using System;
using System.Collections.Generic;
using DG.Tweening;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YaAd;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	public static GameManager Instance;
	
	// Options
	public GlobalSettings Global;
	public UISettings UI;
	public Locale Locale;

	
	// Runtime utilities
	
	public EventCard EventUI { get; set; }
	public int TimedEventTurn { get; set; }
	public bool LockPower { get; set; }

	//public static FrameRateManager FrameRateManager { get; private set; }
	
	public static AchievementsManager AchievementsManager { get; private set; }
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

	public static void ShowError()
	{
		DOTween.Sequence()
			.AppendCallback(() => { Instance.EventUI.ErrorIcon.SetActive(true); })
			.AppendInterval(3)
			.AppendCallback(() => { Instance.EventUI.ErrorIcon.SetActive(false); });
	}
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		PlayGamesPlatform.Instance.Authenticate((status) =>
		{
			switch (status)
			{
				case SignInStatus.Success:
					// Contintue with Play Games Services
					break;
				
				// Show button for manual authenticate
				default:
				case SignInStatus.InternalError:
					break;
				case SignInStatus.Canceled:
					break;
			}
		});
		
		ResourceLoader.ClearResources();
		Instance = Resources.Load<GameManager>("EditorUtilities/GameManager");
		LocalizationSettings.SelectedLocale = Instance.Locale;

		_buildNumber = ResourceLoader.GetResource<BuildScriptableObject>("EditorUtilities/Build").BuildNumber;

		//FrameRateManager = new();
		Application.targetFrameRate = Instance.Global.FrameRateLocked;

		Dictionary<string, string> adIds = ResourceLoader.GetAdIds();
		BannerAdManager.AdUnitId = adIds["banner"];

		AchievementsManager = new();
		
		object saved;
		if ((saved = ResourceLoader.GetPersistentJson<PlayerStats>("PlayerStats")) != null)
		{
			PlayerStats = (PlayerStats) saved;
		}
		else
		{
			PlayerStats = new();
			PlayerStats.Init();
		}
		
		if ((saved = ResourceLoader.GetPersistentJson<Dictionary<string, float>>("GlobalFlags")) != null)
		{
			foreach ((string flag, float value) in (Dictionary<string, float>)saved)
			{
				PlayerStats.SetFlag(flag, value);
			}
		}
		
		ResourceLoader.ReloadGlossary();

		if ((saved = ResourceLoader.GetPersistentJson<EventStorage>("EventStorage")) != null)
		{
			EventStorage = (EventStorage)saved;
		}
		else
		{
			EventStorage = new();
			EventStorage.Load();
		}
		
	}

	public static void Restart()
	{
		LocalizationSettings.SelectedLocale = Instance.Locale;
		
		EventStorage.Dispose();
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