using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

public class EventStorage
{
    [JsonProperty] private List<GameEvent> _events;
    [JsonProperty] private List<GameEvent> _timedEvents;
    [JsonProperty] private List<GameEvent> _eventQueue;
    [JsonProperty] private List<GameEvent> _failEvents;
    [JsonProperty] private List<GameEvent> _winEvents;

    // Contains checksums of events that have been got by GetNext()
    [JsonProperty] private List<int> _pastEvents;

    [JsonProperty] private int _currentTurn;
    [JsonProperty] private bool _isReady;
    
    [JsonProperty] public GameEvent CurrentEvent;

    private static void LocalizeEvent(GameEvent gameEvent, IReadOnlyDictionary<string, string> localized)
    {
        gameEvent.Title = localized[gameEvent.Title];
        gameEvent.Description = localized[gameEvent.Description];
            
        if (gameEvent.Options != null)
        {
            foreach (Option option in gameEvent.Options)
            {
                option.Title = localized[option.Title];
            }
        }
        
        if (gameEvent.TLDR != null)
        {
            foreach (GameEvent nestedEvent in gameEvent.TLDR)
            {
                LocalizeEvent(nestedEvent, localized);
            }
        }
    }
    
    private static List<GameEvent> LoadFile(string path)
    {
        TextAsset raw = ResourceLoader.GetResource<TextAsset>($"Events/{path}.events");
        TextAsset localRaw =
            ResourceLoader.GetResource<TextAsset>(
                $"Localization/{LocalizationSettings.SelectedLocale.Identifier.Code}/Events/{path}.local");
        
        List<GameEvent> events = JsonConvert.DeserializeObject<List<GameEvent>>(raw.text);
        Dictionary<string, string> localized = JsonConvert.DeserializeObject<Dictionary<string, string>>(localRaw.text);

        foreach (GameEvent gameEvent in events)
        {
            LocalizeEvent(gameEvent, localized);
        }

        return events;
    }
    
    public void Load()
    {
        _events = new();
        _timedEvents = new();
        _eventQueue = new();
        _pastEvents = new();
        
        _events.AddRange(LoadFile("Common"));
        _events.AddRange(LoadFile("StartInfo"));
        _events.AddRange(LoadFile("Interview"));
        _events.AddRange(LoadFile("SouthWar"));
        _events.AddRange(LoadFile("StoryTree"));
        
        _timedEvents = LoadFile("Story");
        
        _failEvents = LoadFile("Fail");
        _winEvents = LoadFile("Win");
        
        ResourceLoader.AddGlossaryLinks(_events);
        ResourceLoader.AddGlossaryLinks(_timedEvents);
        ResourceLoader.AddGlossaryLinks(_failEvents);
        ResourceLoader.AddGlossaryLinks(_winEvents);
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

    public void Save()
    {
        ResourceLoader.SavePersistent("EventStorage", this);
    }
    
    public void Init()
    {
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.Init();
        }
        foreach (GameEvent gameEvent in _timedEvents)
        {
            gameEvent.Init();
        }
        
        foreach (GameEvent gameEvent in _events)
        {
            if (!gameEvent.IsTree || gameEvent.IsTreeRoot)
                gameEvent.EnableDynamicChecking();
            if (gameEvent.IsTree)
                gameEvent.UpdateChildren(_events);
        }
        // No dynamic checks for win/fail events (they use another queue)
        // No dynamic checks for timed events (they use another queue)
        foreach (GameEvent timedEvent in _timedEvents.Where(timedEvent => timedEvent.IsTree))
        {
            timedEvent.UpdateChildren(_events);
        }
        
        if (_isReady) return; // No need to reload event queue on loading saved game
        
        // Add to initial queue only available events
        foreach (GameEvent gameEvent in _events)
        {
            if (!gameEvent.IsTree || gameEvent.IsTreeRoot)
                gameEvent.CheckLimits();
        }
        
        _eventQueue.Shuffle();
        
        //_timedEvents.Sort((a, b) => a.TurnPosition.CompareTo(b.TurnPosition));

        _isReady = true;
    }

    public void Dispose()
    {
        foreach (GameEvent gameEvent in _events)
        {
            gameEvent.DisableDynamicChecking();
        }
        _events.Clear();
        _timedEvents.Clear();
        _failEvents.Clear();
    }

    public void EnqueueEvent(GameEvent gameEvent, bool toEnd = false)
    {
        if (_pastEvents.Contains(gameEvent.GetChecksum()))
            return; // Prevent multiple enqueueing*/
        
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

    public List<GameEvent> GetQueueCopy()
    {
        List<GameEvent> res = new();
        res.AddRange(_eventQueue);
        return res;
    }

    public GameEvent GetTimedByTurn(int turn)
    {
        GameEvent res = _timedEvents.FirstOrDefault(x => x.TurnPosition == turn && x.IsAvailable());
        if (res != null)
        {
            _timedEvents.RemoveRange(0, _timedEvents.FindIndex(e => e == res));
            _timedEvents.Remove(res);
        }

        CurrentEvent = res;
        _currentTurn = turn;
        
        return res;
    }
    
    public GameEvent GetNext()
    {
        GameEvent res = default;
        
        // Return related event if it is chained tree
        if (CurrentEvent is {IsTree: true, TreeChildren: not null})
        {
            foreach (GameEvent child in CurrentEvent.TreeChildren)
            {
                if (child == null) continue;
                if (child.IsAvailable())
                {
                    res = child;
                    // _events.Remove(res);
                    break;
                }
            }

            if (res == null)
            {
                CurrentEvent = null;
                return GetNext();
            }
            CurrentEvent.TreeChildren.Remove(res);
        }
        else
        {
            // Continue timed events if common list is empty
            if (_eventQueue.Count < 1)
            {
                if (_timedEvents.Count > 0)
                {
                    res = _timedEvents[0];
                    _timedEvents.Remove(res);

                    if (!res.IsAvailable())
                        return GetNext();
                }
                else
                {
                    CurrentEvent = null;
                    return null;
                }
            }
            // Return timed events in priority
            else if (_timedEvents.Count > 0 && _timedEvents[0].TurnPosition <= _currentTurn)
            {
                res = _timedEvents[0];
                _timedEvents.Remove(res);

                if (!res.IsAvailable())
                {
                    return GetNext();
                }
            }
            // Normal event queue
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
        }

        // Skip duplicate events in queue
        if (_pastEvents.Contains(res.Checksum))
            return GetNext();
        
        if (!res.SkipTurn)
            _currentTurn++;

        CurrentEvent = res;
        _pastEvents.Add(res.Checksum);
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
