using System;
using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	public static GameManager Instance;
	
	// Options
	public UISettings UI;
	
	public static EventStorage EventStorage { get; private set; }
	public static PlayerStats PlayerStats { get; private set; }

	private static string _buildNumber = "1";
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		Instance = Resources.Load<GameManager>("EditorUtilities/GameManager");

		_buildNumber = ResourceLoader.GetResource<BuildScriptableObject>("EditorUtilities/Build").BuildNumber;
		
		PlayerStats = new();
		PlayerStats.Init();
		
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
	public float AnimationDuration;
	public float AnimationScale;
}