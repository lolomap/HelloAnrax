using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Utils;

[Serializable]
public struct Modifier
{
    public ParamType Type;
    public float Value;
}

[Serializable]
public class Option
{
    public string Title;
    public List<Modifier> Modifiers;
    public List<ParamType> ParamLimits;

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
}
