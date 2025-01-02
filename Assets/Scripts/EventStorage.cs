using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class EventStorage
{
    private List<GameEvent> _events;
    private List<GameEvent> _eventQueue;

    private static List<GameEvent> LoadFile(string path)
    {
        TextAsset raw = ResourceLoader.GetResource<TextAsset>(path);
        return JsonConvert.DeserializeObject<List<GameEvent>>(raw.text);
    }
    
    public void Load()
    {
        _events = new();
        _eventQueue = new();
        
        _events.AddRange(LoadFile("CommonEvents"));

        List<GameEvent> timed = LoadFile("TimedEvents");
        _events.AddRange(timed);

        foreach (GameEvent gameEvent in _events.Where(gameEvent => gameEvent.IsAvailable()))
        {
            _eventQueue.Add(gameEvent);
        }
        
        _eventQueue.Shuffle();
        
        foreach (GameEvent gameEvent in timed)
        {
            _eventQueue.Insert(gameEvent.TurnPosition - 1, gameEvent);
        }
    }

    public void EnqueueEvent(GameEvent gameEvent)
    {
        if (!_events.Contains(gameEvent)) throw new ArgumentException("Try to enqueue unknown event");

        int pos = gameEvent.TurnPosition > 0 ? gameEvent.TurnPosition : Random.Range(0, _eventQueue.Count + 1);
        
        _eventQueue.Insert(pos, gameEvent);
    }
    
    public GameEvent GetNext()
    {
        if (_eventQueue.Count < 1) return null;
        
        GameEvent res = _eventQueue[0];
        
        _eventQueue.Remove(res);
        return res;
    }
}
