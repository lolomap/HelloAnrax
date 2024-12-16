using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader
{
	private static readonly Dictionary<string, Object> _resources = new();

	public static T GetResource<T>(string path, bool force = false) where T : Object
	{
		if (!force && _resources.TryGetValue(path, out Object result)) return (T)result;

		result = Resources.Load<T>(path);
		_resources[path] = result;
		return (T)result;
	}
}
