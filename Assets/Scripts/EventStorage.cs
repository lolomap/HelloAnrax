using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class EventStorage
{
    private List<GameEvent> _common;
    private List<GameEvent> _partyTemplate;
    private List<GameEvent> _partySpecial;
    private List<GameEvent> _timed;

    private Queue<int> _eventQueue = new();
    private Queue<int> _timedQueue = new();
    private int _eventsCounter = 0;

    private const float PartyTemplateChance = 30.0f;
    private const float PartySpecialChance = 30.0f;
    private const float OptionParamMin = 0.3f;

    public void Load()
    {
        TextAsset commonRaw = ResourceLoader.GetResource<TextAsset>("CommonEvents");
        _common = JsonConvert.DeserializeObject<List<GameEvent>>(commonRaw.text);

        int queueSize = _common.Count; //max(common, template, special)
        List<int> indexes = new(new int[queueSize]);
        for (int i = 0; i < queueSize; i++)
        {
            indexes[i] = i;
        }
        
        TextAsset timedRaw = ResourceLoader.GetResource<TextAsset>("TimedEvents");
        _timed = JsonConvert.DeserializeObject<List<GameEvent>>(timedRaw.text);
        
        foreach (GameEvent e in _timed)
        {
            _timedQueue.Enqueue(e.TurnPosition);
        }

        while (indexes.Count > 0)
        {
            int pos = Random.Range(0, indexes.Count);
            _eventQueue.Enqueue(indexes[pos]);
            indexes.RemoveAt(pos);
        }
    }
    
    public GameEvent GetNext()
    {
        _eventsCounter++;
        GameEvent res = null;

        if (_timedQueue.Count > 0 && _eventsCounter == _timedQueue.Peek())
        {
            int timedIndex = _timedQueue.Dequeue();

            res = _timed[--timedIndex];
            
            return res;
        }
        
        int eventIndex = _eventQueue.Dequeue();

        // List<Party> possibleParties = new();
        // if (IsNpEventsUnlocked) possibleParties.Add(Party.Nationalists);
        // if (IsUpEventsUnlocked) possibleParties.Add(Party.Unionists);
        // if (IsWpEventsUnlocked) possibleParties.Add(Party.Westernists);
        //
        // if (possibleParties.Count > 0)
        // {
        //     float chance = Random.value;
        //     switch (chance)
        //     {
        //         case < PartyTemplateChance:
        //             res = _partyTemplate[eventIndex];
        //             res.PartyTemplate = possibleParties[Random.Range(0, possibleParties.Count)];
        //             break;
        //         case < PartyTemplateChance + PartySpecialChance:
        //             res = _partySpecial[eventIndex];
        //             break;
        //     }
        // }
        // else
        {
            res = _common[eventIndex];
        }

        if (res == null)
            throw new("Failed to get next event");
        
        if (!res.IsDisposable)
            _eventQueue.Enqueue(eventIndex);
        
        return res;
    }
}
