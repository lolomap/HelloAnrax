using System;
using System.Collections.Generic;
using System.Linq;
using static Utils;

[Serializable]
public class JsonValue
{
    public string Type;
    public float Value;
}

[Serializable]
public class Modifier : JsonValue {}

[Serializable]
public class Flag : JsonValue
{
    public Comparasion Comparasion = Comparasion.GtE;
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

    public List<Flag> Limits;
    
    public int TurnPosition = -1;
    public string Soundtrack = "MainTheme";

    private void CheckLimits()
    {
        if (IsAvailable())
            GameManager.EventStorage.EnqueueEvent(this);
    }
    
    public GameEvent()
    {
        GameManager.PlayerRates.Updated += CheckLimits;
    }

    ~GameEvent()
    {
        GameManager.PlayerRates.Updated -= CheckLimits;
    }

    public bool IsAvailable()
    {
        return Limits.Any(limitation => GameManager.PlayerRates.HasFlag(limitation));
    }
}
