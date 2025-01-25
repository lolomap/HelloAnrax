using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class PlayerStats
{
	private Dictionary<string, float> _stats;
	private Dictionary<string, float> _flags;
	private Dictionary<string, string> _formulas;

	public event Action Updated;
	private bool _isReady;

	public void Init()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultStats");
		_stats = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		_flags = new();
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("StatsConfig");
		_formulas = JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);

		_isReady = true;
	}

	public void UpdateUI()
	{
		foreach ((string stat, float value) in _stats)
		{
			TaggedValue.UpdateAll(stat, value);
		}
		
		foreach ((string flag, float value) in _flags)
		{
			TaggedValue.UpdateAll(flag, value);
		}
	}

	public void CalculateFormulas()
	{
		Dictionary<string, decimal> variables = new();
		foreach ((string stat, float value) in _stats)
		{
			variables[stat] = (decimal) value;
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

	public float GetStat(string stat) => _stats[stat];
	public void SetStat(string stat, float value)
	{
		_stats[stat] = value;
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
		if (!_flags.ContainsKey(flag)) return false;
		return _flags[flag] > 0;
	}
	public bool HasFlag(Flag flag)
	{
		return _stats.ContainsKey(flag.Type) && Utils.Compare(_stats[flag.Type], flag.CompareTo, flag.Comparison)
		       ||
		       _flags.ContainsKey(flag.Type) && Utils.Compare(_flags[flag.Type], flag.CompareTo, flag.Comparison);
	}
	public void SetFlag(string flag, float value)
	{
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
