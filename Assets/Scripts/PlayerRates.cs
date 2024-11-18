using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerRates : MonoBehaviour
{
	private static readonly Dictionary<Utils.RateType, float> Rates = new();

	private void Awake()
	{
		TextAsset defaultsRaw = Resources.Load<TextAsset>("DefaultRates");
		Dictionary<string, float> defaults = JsonConvert.DeserializeObject<Dictionary<string, float>>(defaultsRaw.text);

		foreach ((string key, float value) in defaults)
		{
			if (!Enum.TryParse(key, out Utils.RateType rate))
				continue;

			Rates[rate] = value;
		}
	}

	public static float GetRate(Utils.RateType rate)
	{
		return Rates[rate];
	}

	public static void UpdateRate(Utils.RateType rate, float value)
	{
		Rates[rate] = value;
	}
}
