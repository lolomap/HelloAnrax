using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static Utils;

[Serializable]
public class JsonValue
{
    public string Type;
    public float Value;
}

[Serializable]
public class Modifier : JsonValue {}

/// <summary>
/// Flags are used for checking availability.
/// Set Type same as stat name for stat check, otherwise special flag will be used
/// </summary>
[Serializable]
public class Flag : JsonValue
{
    public Comparison Comparison = Comparison.GtE;
    public float CompareTo;
}

[Serializable]
public class Option
{
    public string Title;
    public string Category = "Default"; 
    public List<Modifier> Modifiers;
    public List<Flag> Flags;

    public List<Flag> Limits;
}

[Serializable]
public class GameEvent
{
    public string Title;
    public string Description;

    public string Category = "Default";
    
    public string PartyTemplate;
    public List<Option> Options;
    public bool IsDisposable = true;

    public bool IsTrigger;
    public List<Flag> Limits;
    
    public int TurnPosition = -1;
    public string Soundtrack = "MainTheme";

    
    
    public void EnableDynamicChecking()
    {
        GameManager.PlayerStats.Updated += CheckLimits;
    }

    ~GameEvent()
    {
        if (GameManager.PlayerStats != null)
            GameManager.PlayerStats.Updated -= CheckLimits;
    }

    public bool IsAvailable()
    {
        return Limits == null || Limits.Any(limitation => GameManager.PlayerStats.HasFlag(limitation));
    }
    
    public void CheckLimits()
     {
         if (IsAvailable())
             GameManager.EventStorage.EnqueueEvent(this);
         else GameManager.EventStorage.DequeueEvent(this);
     }
}
