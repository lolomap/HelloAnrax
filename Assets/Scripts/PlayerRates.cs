using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using UnityEngine;

public class PlayerRates : MonoBehaviour
{
	private static readonly Dictionary<Utils.RateType, float> _rates = new();
	private static readonly Dictionary<Utils.RateType, string> _formulas = new();

	private void Awake()
	{
		TextAsset defaultsRaw = ResourceLoader.GetResource<TextAsset>("DefaultRates");
		Dictionary<string, float> defaults = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		foreach ((string key, float value) in defaults)
		{
			if (!Enum.TryParse(key, out Utils.RateType rate))
				continue;

			_rates[rate] = value;
		}
		
		TextAsset formulasRaw = ResourceLoader.GetResource<TextAsset>("RatesConfig");
		Dictionary<string, string> formulas =
			JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);

		foreach ((string key, string value) in formulas)
		{
			if (!Enum.TryParse(key, out Utils.RateType rate))
				continue;

			_formulas[rate] = value;
		}
	}

	private void Start()
	{
		foreach ((Utils.RateType rate, float value) in _rates)
		{
			TaggedValue.UpdateAll(rate.ToString(), value);
		}
	}

	public static void CalculateFormulas()
	{
		Dictionary<string, decimal> variables = new();
		foreach ((Utils.RateType rate, float value) in _rates)
		{
			variables[rate.ToString()] = (decimal) value;
		}
		
		foreach ((Utils.RateType rate, string formula) in _formulas)
		{
			UpdateRate(rate,  GetRate(rate) + Convert.ToSingle(Utils.Evaluator.Evaluate(formula, variables)));
		}
	}

	public static float GetRate(Utils.RateType rate)
	{
		return _rates[rate];
	}

	public static void UpdateRate(Utils.RateType rate, float value)
	{
		_rates[rate] = value;
		TaggedValue.UpdateAll(rate.ToString(), value);
	}
}
