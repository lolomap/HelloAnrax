using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	public static GameManager Instance;
	
	// Options
	public GlobalSettings Global;
	public UISettings UI;

	
	// Runtime utilities
	
	public EventCard EventUI { get; set; }
	public int TimedEventTurn { get; set; }

	//public static FrameRateManager FrameRateManager { get; private set; }
	
	public static EventStorage EventStorage { get; private set; }
	public static PlayerStats PlayerStats { get; private set; }

	private static string _buildNumber = "1";

	public static void OverridePlayerStats(PlayerStats stats)
	{
		PlayerStats = stats;
	}

	public static void OverrideEventStorage(EventStorage eventStorage)
	{
		EventStorage = eventStorage;
	}
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		ResourceLoader.ClearResources();
		Instance = Resources.Load<GameManager>("EditorUtilities/GameManager");

		_buildNumber = ResourceLoader.GetResource<BuildScriptableObject>("EditorUtilities/Build").BuildNumber;

		//FrameRateManager = new();
		Application.targetFrameRate = Instance.Global.FrameRateLocked;

		Dictionary<string, string> adIds = ResourceLoader.GetAdIds();
		BannerAdManager.AdUnitId = adIds["banner"];

		string saved;
		if (PlayerPrefs.HasKey("PlayerStats") && (saved = PlayerPrefs.GetString("PlayerStats")) != "{}")
		{
			PlayerStats = JsonConvert.DeserializeObject<PlayerStats>(saved);
		}
		else
		{
			PlayerStats = new();
			PlayerStats.Init();
		}
		
		if (PlayerPrefs.HasKey("GlobalFlags") && (saved = PlayerPrefs.GetString("GlobalFlags")) != "{}")
		{
			foreach ((string flag, float value) in
			         JsonConvert.DeserializeObject<Dictionary<string, float>>(saved))
			{
				PlayerStats.SetFlag(flag, value);
			}
		}
		
		ResourceLoader.ReloadGlossary();

		if (PlayerPrefs.HasKey("EventStorage") && (saved = PlayerPrefs.GetString("EventStorage")) != "{}")
		{
			EventStorage = JsonConvert.DeserializeObject<EventStorage>(saved);
		}
		else
		{
			EventStorage = new();
			EventStorage.Load();
		}
		
	}

	public static void Restart()
	{
		IEnumerable<KeyValuePair<string, float>> saveGlobal = PlayerStats.GetGlobalFlags();
		PlayerStats = new();
		PlayerStats.Init();
		foreach ((string flag, float value) in saveGlobal)
		{
			PlayerStats.SetFlag(flag, value);
		}
		
		EventStorage = new();
		EventStorage.Load();
	}

	public static string GetVersion()
	{
		return $"v{Application.version}.{_buildNumber}";
	}
}

[Serializable]
public struct UISettings
{
	public float AnimationDurationSec;
	public float AnimationScale;
	public float AnimationShakeStrength;
	public float AnimationDelaySec;
	
	public float PopUpDuration;
}

[Serializable]
public struct GlobalSettings
{
	public int FrameRateLocked;
}