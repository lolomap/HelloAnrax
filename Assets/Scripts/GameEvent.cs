using System;
using System.Collections.Generic;
using static Utils;

[Serializable]
public struct Modifier
{
    public RateType Type;
    public float Value;
}

[Serializable]
public class Option
{
    public string Title;
    public List<Modifier> Modifiers;
    public List<RateType> ParamLimits;

}

[Serializable]
public class GameEvent
{
    public GameEventType Type;
    public string Title;
    public string Description;
    public Party PartyTemplate;
    public List<Option> Options;
    public bool IsDisposable;
    
    public int TurnPosition = -1;
    public string Soundtrack = "MainTheme";
}
