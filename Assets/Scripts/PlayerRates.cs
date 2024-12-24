using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class PlayerRates : MonoBehaviour
{
	private static Dictionary<string, float> _rates;
	private static Dictionary<string, float> _flags;
	private static Dictionary<string, string> _formulas;

	private void Awake()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultRates");
		_rates = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		_flags = new();
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("RatesConfig");
		_formulas = JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);
	}

	private void Start()
	{
		foreach ((string rate, float value) in _rates)
		{
			TaggedValue.UpdateAll(rate, value);
		}
	}

	public static void CalculateFormulas()
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

	public static float GetRate(string rate) => _rates[rate];
	public static void UpdateRate(string rate, float value)
	{
		_rates[rate] = value;
		TaggedValue.UpdateAll(rate, value);
	}

	public static float GetFlag(string flag)
	{
		if (!_flags.ContainsKey(flag)) return 0;
		return _flags[flag];
	}
	public static bool HasFlag(string flag)
	{
		if (!_flags.ContainsKey(flag)) return false;
		return _flags[flag] > 0;
	}
	public static void SetFlag(string flag, float value)
	{
		_flags[flag] = value;
		TaggedValue.UpdateAll(flag, value);
	}
}
