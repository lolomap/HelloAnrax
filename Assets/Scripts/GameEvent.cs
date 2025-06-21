using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using static Utils;

[Serializable]
public class JsonValue
{
    public string Type;
    public float Value;
}

[Serializable]
public class Modifier : JsonValue
{
    public Flag Limit;
}

/// <summary>
/// Flags are used for checking availability.
/// Set Type same as stat name for stat check, otherwise special flag will be used
/// </summary>
[Serializable]
public class Flag : JsonValue
{
    public Comparison Comparison = Comparison.GtE;
    public float CompareTo = 1f;
}

[Serializable]
public class Option
{
    public string Title;
    public string Category = "Default"; 
    public List<Modifier> Modifiers;
    public List<Flag> Flags;

    public List<Flag> Limits;

    public bool IsAvailable()
    {
        bool result = true;
        List<Flag> limitations = Limits;
        
        if (limitations == null) return true;
        
        foreach (Flag limitation in limitations)
        {
            result = result && GameManager.PlayerStats.HasFlag(limitation);
        }

        return result;
    }
    
    public bool IsAvailable(out List<Flag> blockedFlags)
    {
        bool result = true;
        List<Flag> limitations = Limits;
        blockedFlags = new();
        
        if (limitations == null) return true;
        
        foreach (Flag limitation in limitations)
        {
            if (!GameManager.PlayerStats.HasFlag(limitation))
            {
                blockedFlags.Add(limitation);
                result = false;
            }
        }

        return result;
    }
}

[Serializable]
public class GameEvent
{
    public string Id = "";
    
    public string Title = "";
    public string Description = "";

    public string Category = "Default";
    
    public List<Option> Options;
    public bool IsDisposable = true;

    public bool IsTrigger;
    public bool IsTreeRoot;
    public bool IsTree;
    public List<Flag> Limits;
    
    public int TurnPosition = -1;
    public bool SkipTurn;
    public string Soundtrack = "MainTheme";

    public List<GameEvent> TLDR;

    public int Checksum;
    
    [NonSerialized] public List<GameEvent> TreeChildren;
    [NonSerialized] private bool _isDynamicChecking;
    
    [JsonProperty] private bool _isDestroyed;
    [JsonProperty] private bool _isEnqueued;
    
    public void EnableDynamicChecking()
    {
        if (!_isDynamicChecking && !_isDestroyed)
        {
            GameManager.PlayerStats.Updated += CheckLimits;
            _isDynamicChecking = true;
        }
    }

    public void DisableDynamicChecking()
    {
        GameManager.PlayerStats.Updated -= CheckLimits;
        _isDestroyed = true;
        _isDynamicChecking = false;
    }

    // DO NOT USE CONSTRUCTOR WITH SERIALIZER! 
    public void Init()
    {
        Checksum = GetChecksum();
        if (IsTreeRoot)
            IsTree = true;
    }
    
    ~GameEvent()
    {
        if (GameManager.PlayerStats != null)
            DisableDynamicChecking();
    }

    public bool IsAvailable()
    {
        return Limits == null || Limits.All(limitation => GameManager.PlayerStats.HasFlag(limitation));
    }
    
    public void CheckLimits()
    {
        if (IsAvailable())
        {
            if (_isEnqueued) return;
            GameManager.EventStorage.EnqueueEvent(this);
            _isEnqueued = true;
        }
        else
        {
            GameManager.EventStorage.DequeueEvent(this);
            _isEnqueued = false;
        }
    }

    public int GetChecksum()
    {
        return Id.GetHashCode() + Category.GetHashCode();
    }

    public void UpdateChildren(List<GameEvent> events)
    {
        TreeChildren ??= new();
        if (Options == null) return;
        foreach (Option option in Options)
        {
            if (option.Flags == null) continue;
            foreach (Flag flag in option.Flags)
            {
                List<GameEvent> child = events.FindAll(x =>
                    x.IsTree && x.Limits != null &&
                    x.Limits.Any(y => y.Type == flag.Type));
                TreeChildren.AddRange(child);
            }
        }
    }
}
