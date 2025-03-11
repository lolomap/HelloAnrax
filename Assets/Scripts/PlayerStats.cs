using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class PlayerStats
{
	[Serializable]
	private class Stat
	{
		public float Min;
		public float Max = 9999f;
		public float Value;
	}
	
	[JsonProperty] private Dictionary<string, Stat> _stats;
	[JsonProperty] private Dictionary<string, float> _flags;
	[JsonProperty] private Dictionary<string, string> _formulas;

	[JsonProperty] private Dictionary<string, float> _globalFlags;

	public event Action Updated;
	private bool _isReady;

	public void Init()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultStats");
		_stats = JsonConvert.DeserializeObject<Dictionary<string, Stat>>(defaultsRaw.text);

		_flags = new();
		_globalFlags = new();
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("StatsConfig");
		_formulas = JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);

		_isReady = true;
	}

	public void SaveGlobal()
	{
		PlayerPrefs.SetString("GlobalFlags", JsonConvert.SerializeObject(_globalFlags));
	}

	public void Save()
	{
		PlayerPrefs.SetString("PlayerStats", JsonConvert.SerializeObject(this));
	}

	public void UpdateUI()
	{
		foreach ((string id, Stat stat) in _stats)
		{
			TaggedValue.UpdateAll(id, stat.Value);
		}
		
		foreach ((string flag, float value) in _flags)
		{
			TaggedValue.UpdateAll(flag, value);
		}
	}

	public void CalculateFormulas()
	{
		Dictionary<string, decimal> variables = new();
		foreach ((string id, Stat stat) in _stats)
		{
			variables[id] = (decimal) stat.Value;
		}
		
		foreach ((string stat, string formula) in _formulas)
		{
			float value = Convert.ToSingle(Utils.Evaluator.Evaluate(formula, variables));
			
			SetStat(stat, value);
			
			if (!variables.ContainsKey(stat))
			{
				variables[stat] = (decimal) value;
			}
		}
		
		OnUpdated();
	}

	public IEnumerable<KeyValuePair<string,float>> GetGlobalFlags()
	{
		return _flags.Where(x => x.Key.StartsWith("GLOBAL_"));
	}
	
	public float GetStat(string stat) => _stats[stat].Value;
	public void SetStat(string stat, float value)
	{
		if (!_stats.ContainsKey(stat))
			_stats[stat] = new();

		if (value < _stats[stat].Min)
			_stats[stat].Value = _stats[stat].Min;
		else if (value > _stats[stat].Max)
			_stats[stat].Value = _stats[stat].Max;
		else
			_stats[stat].Value = value;
		
		TaggedValue.UpdateAll(stat, value);
		
		OnUpdated();
	}

	public float GetFlag(string flag)
	{
		if (!_flags.ContainsKey(flag)) return 0;
		return _flags[flag];
	}
	public bool HasFlag(string flag)
	{
		if (_flags.TryGetValue(flag, out float flagValue))
			return flagValue > 0;
		if (_stats.TryGetValue(flag, out Stat stat))
			return stat.Value > 0;

		return false;
	}
	public bool HasFlag(Flag flag)
	{
		if (flag.Type == null) return true;
		
		float flagValue = 0;
		if (_flags.ContainsKey(flag.Type))
			flagValue = _flags[flag.Type];
		
		return _stats.ContainsKey(flag.Type) && Utils.Compare(_stats[flag.Type].Value, flag.CompareTo, flag.Comparison)
		       ||
		       Utils.Compare(flagValue, flag.CompareTo, flag.Comparison);
	}
	public void SetFlag(string flag, float value)
	{
		if (flag.StartsWith("GLOBAL_"))
			_globalFlags[flag] = value;
			
		_flags[flag] = value;
		TaggedValue.UpdateAll(flag, value);

		OnUpdated();
	}

	private void OnUpdated()
	{
		if (_isReady)
			Updated?.Invoke();
	}
}
