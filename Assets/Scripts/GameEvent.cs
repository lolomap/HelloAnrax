using System;
using System.Collections.Generic;
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
public class Flag : JsonValue {}

[Serializable]
public class Option
{
    public string Title;
    public string Category = "Default"; 
    public List<Modifier> Modifiers;
    public List<Flag> Flags;

    public List<Flag> Limitations;
}

[Serializable]
public class GameEvent
{
    public GameEventType Type;
    
    public string Title;
    public string Description;

    public string Category = "Default";
    
    public string PartyTemplate;
    public List<Option> Options;
    public bool IsDisposable;
    
    public int TurnPosition = -1;
    public string Soundtrack = "MainTheme";
}
