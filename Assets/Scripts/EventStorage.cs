using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static Utils;
using Random = UnityEngine.Random;

public class EventStorage
{
    private static List<GameEvent> _common;
    private static List<GameEvent> _partyTemplate;
    private static List<GameEvent> _partySpecial;

    private static Queue<int> _eventQueue = new();

    private const float PartyTemplateChance = 30.0f;
    private const float PartySpecialChance = 30.0f;
    private const float OptionParamMin = 0.3f;

    public static bool IsNpEventsUnlocked, IsUpEventsUnlocked, IsWpEventsUnlocked;

    public static void Load()
    {
        TextAsset commonRaw = Resources.Load<TextAsset>("CommonEvents");
        _common = JsonConvert.DeserializeObject<List<GameEvent>>(commonRaw.text);

        int queueSize = _common.Count; //max(common, template, special)
        List<int> indexes = new(new int[queueSize]);
        for (int i = 0; i < queueSize; i++)
        {
            indexes[i] = i;
        }

        while (indexes.Count > 0)
        {
            int pos = Random.Range(0, indexes.Count);
            _eventQueue.Enqueue(indexes[pos]);
            indexes.RemoveAt(pos);
        }
    }
    
    public static GameEvent GetNext()
    {
        GameEvent res = null;
        
        int eventIndex = _eventQueue.Dequeue();

        List<Party> possibleParties = new();
        if (IsNpEventsUnlocked) possibleParties.Add(Party.Nationalists);
        if (IsUpEventsUnlocked) possibleParties.Add(Party.Unionists);
        if (IsWpEventsUnlocked) possibleParties.Add(Party.Westernists);
        
        if (possibleParties.Count > 0)
        {
            float chance = Random.value;
            switch (chance)
            {
                case < PartyTemplateChance:
                    res = _partyTemplate[eventIndex];
                    res.PartyTemplate = possibleParties[Random.Range(0, possibleParties.Count)];
                    break;
                case < PartyTemplateChance + PartySpecialChance:
                    res = _partySpecial[eventIndex];
                    break;
            }
        }
        else
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
