using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class ResourceLoader
{
	private static readonly Dictionary<string, Object> _resources = new();

	private static Dictionary<string, string> _glossary = new();

	public static T GetResource<T>(string path, bool force = false) where T : Object
	{
		if (!force && _resources.TryGetValue(path, out Object result)) return (T)result;

		result = Resources.Load<T>(path);
		_resources[path] = result;
		return (T)result;
	}

	public static void ReloadGlossary()
	{
		_glossary =
			JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.Load<TextAsset>("Glossary").text);
	}

	public static string GetGlossaryText(string id)
	{
		return _glossary.GetValueOrDefault(id);
	}
}
