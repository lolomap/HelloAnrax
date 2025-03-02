using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class EventStorage
{
    private List<GameEvent> _events;
    private List<GameEvent> _timedEvents;
    private List<GameEvent> _eventQueue;
    private List<GameEvent> _failEvents;
    private List<GameEvent> _winEvents;

    private int _currentTurn;

    private static List<GameEvent> LoadFile(string path)
    {
        TextAsset raw = ResourceLoader.GetResource<TextAsset>(path);
        return JsonConvert.DeserializeObject<List<GameEvent>>(raw.text);
    }
    
    public void Load()
    {
        _events = new();
        _timedEvents = new();
        _eventQueue = new();
        
        _events.AddRange(LoadFile("CommonEvents"));
        _timedEvents = LoadFile("TimedEvents");

        _failEvents = LoadFile("FailEvents");
        _winEvents = LoadFile("WinEvents");
    }

    public void Load(List<GameEvent> events, List<GameEvent> timedEvents,
        List<GameEvent> failEvents, List<GameEvent> winEvents)
    {
        _eventQueue = new();
        
        _events = events;
        _timedEvents = timedEvents;
        _failEvents = failEvents;
        _winEvents = winEvents;
    }

    public void Init()
    {
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.EnableDynamicChecking();
        }
        // No dynamic checks for win/fail events (they use another queue)
        // No dynamic checks for timed events (they use another queue)
        
        // Add to initial queue only available events
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.CheckLimits();
        }
        
        _eventQueue.Shuffle();
    }

    public void EnqueueEvent(GameEvent gameEvent, bool toEnd = false)
    {
        if (_eventQueue.Contains(gameEvent)) return; // Prevent multiple enqueueing
        
        if (!_events.Contains(gameEvent)) throw new ArgumentException("Try to enqueue unknown event");

        int pos;
        if (gameEvent.IsTrigger)
            pos = 0;
        else if (toEnd)
            pos = _eventQueue.Count;
        else 
            pos = Random.Range(0, _eventQueue.Count + 1);

        _eventQueue.Insert(pos, gameEvent);
    }

    public void DequeueEvent(GameEvent gameEvent)
    {
        if (!_events.Contains(gameEvent)) throw new ArgumentException("Try to dequeue unknown event");

        _eventQueue.Remove(gameEvent);
    }

    public List<GameEvent> GetQueue()
    {
        List<GameEvent> res = new();
        res.AddRange(_eventQueue);
        return res;
    }
    
    public GameEvent GetNext()
    {
        if (_eventQueue.Count < 1) return null;

        GameEvent res;
        
        _currentTurn++;
        if (_timedEvents.Count > 0 && _timedEvents[0].TurnPosition <= _currentTurn)
        {
            res = _timedEvents[0];

            _timedEvents.Remove(res);
        }
        else
        {
            res = _eventQueue[0];

            _eventQueue.Remove(res);
            if (!res.IsDisposable)
            {
                EnqueueEvent(res, true);
            }
            else
            {
                res.DisableDynamicChecking();
                _events.Remove(res);
            }
        }
        
        return res;
    }

    public GameEvent GetFail()
    {
        return _failEvents.Count < 1 ? null : _failEvents.FirstOrDefault(gameEvent => gameEvent.IsAvailable());
    }

    public GameEvent GetWin()
    {
        return _winEvents.Count < 1 ? null : _winEvents.FirstOrDefault(gameEvent => gameEvent.IsAvailable());
    }
}
