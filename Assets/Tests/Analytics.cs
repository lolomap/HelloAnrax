using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class Analytics
	{
		[Test]
		public void CalculateBalance()
		{
			EventStorage storage = new();
			GameManager.OverrideEventStorage(storage);
			storage.Load();
			storage.Init();
			

			Dictionary<string, float> statsBalances = new();

			while (storage.GetNext() is { } data)
			{
				if (data.Options != null)
				{
					foreach (Option option in data.Options)
					{
						if (option.Modifiers != null)
						{
							foreach (Modifier modifier in option.Modifiers)
							{
								// Divide modifier because it can be not selected
								statsBalances.TryAdd(modifier.Type, 0);
								statsBalances[modifier.Type] += modifier.Value / data.Options.Count;
							}
						}
					}
				}
			}
			
			// Build log output
			string log = "";
			List<KeyValuePair<string, float>> list = statsBalances.ToList();
			list.Sort((x, y) =>
				string.Compare(x.Key, y.Key, StringComparison.Ordinal));
			foreach ((string stat, float balance) in list)
			{
				log += $"{stat}:\t\t\t\t\t{balance}\n";
			}
			File.WriteAllText($"{Path.GetDirectoryName(Application.dataPath)}/Logs/ModifiersBalance.log", log);
		}
	}
}