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
    }

    public void Init()
    {
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.Init();
        }
        
        // Add to initial queue only available events
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.CheckLimits();
        }
        
        _eventQueue.Shuffle();
        
        foreach (GameEvent gameEvent in _timedEvents)
        {
            _eventQueue.Insert(gameEvent.TurnPosition - 1, gameEvent);
        }
    }

    public void EnqueueEvent(GameEvent gameEvent, bool toEnd = false)
    {
        if (!_events.Contains(gameEvent)) throw new ArgumentException("Try to enqueue unknown event");

        int pos;
        if (toEnd)
            pos = _eventQueue.Count;
        else if (gameEvent.IsTrigger)
            pos = 0;
        else if (gameEvent.TurnPosition > 0)
            pos = gameEvent.TurnPosition;
        else
            pos = Random.Range(0, _eventQueue.Count + 1);

        _eventQueue.Insert(pos, gameEvent);
    }

    public void DequeueEvent(GameEvent gameEvent)
    {
        if (!_events.Contains(gameEvent)) throw new ArgumentException("Try to dequeue unknown event");

        _eventQueue.Remove(gameEvent);
    }
    
    public GameEvent GetNext()
    {
        if (_eventQueue.Count < 1) return null;
        
        GameEvent res = _eventQueue[0];
        
        _eventQueue.Remove(res);
        if (!res.IsDisposable)
        {
            EnqueueEvent(res, true);
        }
        
        return res;
    }
}
