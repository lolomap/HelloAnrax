using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class PlayerRates
{
	private Dictionary<string, float> _rates;
	private Dictionary<string, float> _flags;
	private Dictionary<string, string> _formulas;

	public event Action Updated;

	public void Init()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultRates");
		_rates = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		_flags = new();
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("RatesConfig");
		_formulas = JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);
	}

	public void UpdateUI()
	{
		foreach ((string rate, float value) in _rates)
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
		foreach ((string rate, float value) in _rates)
		{
			variables[rate] = (decimal) value;
		}
		
		foreach ((string rate, string formula) in _formulas)
		{
			UpdateRate(rate,  GetRate(rate) + Convert.ToSingle(Utils.Evaluator.Evaluate(formula, variables)));
		}
	}

	public float GetRate(string rate) => _rates[rate];
	public void UpdateRate(string rate, float value)
	{
		_rates[rate] = value;
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
		return _flags.ContainsKey(flag.Type) && Utils.Compare(_flags[flag.Type], flag.CompareTo, flag.Comparasion);
	}
	public void SetFlag(string flag, float value)
	{
		_flags[flag] = value;
		TaggedValue.UpdateAll(flag, value);
		Updated?.Invoke();
	}
}
