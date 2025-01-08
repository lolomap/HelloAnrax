using System;
using System.Collections;
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

	public void Init()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultStats");
		_stats = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		_flags = new();
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("StatsConfig");
		_formulas = JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);
	}

	public void UpdateUI()
	{
		foreach ((string rate, float value) in _stats)
		{
			TaggedValue.UpdateAll(rate, value);
		}
		
		foreach ((string flag, float value) in _flags)
		{
			TaggedValue.UpdateAll(flag, value);
		}
	}

	public void CalculateFormulas()
	{
		Dictionary<string, decimal> variables = new();
		foreach ((string rate, float value) in _stats)
		{
			variables[rate] = (decimal) value;
		}
		
		foreach ((string rate, string formula) in _formulas)
		{
			SetStat(rate, Convert.ToSingle(Utils.Evaluator.Evaluate(formula, variables)));
		}
	}

	public float GetStat(string rate) => _stats[rate];
	public void SetStat(string rate, float value)
	{
		_stats[rate] = value;
		TaggedValue.UpdateAll(rate, value);
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
		return _stats.ContainsKey(flag.Type) && Utils.Compare(_stats[flag.Type], flag.CompareTo, flag.Comparasion)
		       ||
		       _flags.ContainsKey(flag.Type) && Utils.Compare(_flags[flag.Type], flag.CompareTo, flag.Comparasion);
	}
	public void SetFlag(string flag, float value)
	{
		_flags[flag] = value;
		TaggedValue.UpdateAll(flag, value);
		Updated?.Invoke();
	}
}
