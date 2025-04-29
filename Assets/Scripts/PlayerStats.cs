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
	[JsonProperty]
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
			
			variables[stat] = (decimal) SetStat(stat, value);
			Debug.Log($"{stat}: {variables[stat]}");
		}
		
		OnUpdated();
	}

	public IEnumerable<KeyValuePair<string,float>> GetGlobalFlags()
	{
		return _flags.Where(x => x.Key.StartsWith("GLOBAL_"));
	}
	
	public float GetStat(string stat) => _stats[stat].Value;
	public float SetStat(string stat, float value)
	{
		string uiTag = stat;
		if (uiTag.StartsWith("HIGHLIGHT_"))
			uiTag = uiTag.Split("HIGHLIGHT_")[1];

		if (!_stats.ContainsKey(uiTag))
			_stats[uiTag] = new();

		if (value < _stats[uiTag].Min)
			_stats[uiTag].Value = _stats[uiTag].Min;
		else if (value > _stats[uiTag].Max)
			_stats[uiTag].Value = _stats[uiTag].Max;
		else
			_stats[uiTag].Value = value;
		
		TaggedValue.UpdateAll(stat, value);
		
		OnUpdated();

		return _stats[uiTag].Value;
	}

	public float GetFlag(string flag)
	{
		if (!_flags.ContainsKey(flag)) return 0;
		return _flags[flag];
	}
	public bool HasFlag(string flag)
	{
		// If player has no flag, it was 0 and we need to set it for Lt/LtE checks
		_flags.TryGetValue(flag, out float flagValue);

		return Utils.Compare(_stats.TryGetValue(flag, out Stat stat) ? stat.Value : flagValue,
			1, Utils.Comparison.GtE);
	}
	public bool HasFlag(Flag flag)
	{
		// If player has no flag, it was 0 and we need to set it for Lt/LtE checks
		_flags.TryGetValue(flag.Type, out float flagValue);

		return Utils.Compare(_stats.TryGetValue(flag.Type, out Stat stat) ? stat.Value : flagValue,
			flag.CompareTo, flag.Comparison);
	}
	public void SetFlag(string flag, float value)
	{
		if (flag == null) return;
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
