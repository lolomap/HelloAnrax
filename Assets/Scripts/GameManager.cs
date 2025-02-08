using System;
using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	public static GameManager Instance;
	
	// Options
	public GlobalSettings Global;
	public UISettings UI;
	
	//public static FrameRateManager FrameRateManager { get; private set; }
	
	public static EventStorage EventStorage { get; private set; }
	public static PlayerStats PlayerStats { get; private set; }

	private static string _buildNumber = "1";
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		Instance = Resources.Load<GameManager>("EditorUtilities/GameManager");

		_buildNumber = ResourceLoader.GetResource<BuildScriptableObject>("EditorUtilities/Build").BuildNumber;

		//FrameRateManager = new();
		Application.targetFrameRate = Instance.Global.FrameRateLocked;
		
		PlayerStats = new();
		PlayerStats.Init();
		
		EventStorage = new();
		EventStorage.Load();
		
		ResourceLoader.ReloadGlossary();
	}

	public static string GetVersion()
	{
		return $"v{Application.version}.{_buildNumber}";
	}
}

[Serializable]
public struct UISettings
{
	public float AnimationDuration;
	public float AnimationScale;
	public float AnimationShakeStrength;

	public float PopUpDuration;
}

[Serializable]
public struct GlobalSettings
{
	public int FrameRateLocked;
}