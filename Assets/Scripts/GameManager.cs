using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	public enum ResourcesLoadOption
	{
		Default,
		Unload,
		Reload
	}

	private static GameManager _instance;
	
	// Options
	// public bool exampleOption;
	
	public static EventStorage EventStorage { get; private set; }
	public static PlayerRates PlayerRates { get; private set; }
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		_instance = Resources.Load<GameManager>("EditorUtilities/GameManager");
		
		EventStorage = new();
		EventStorage.Load();

		PlayerRates = new();
		PlayerRates.Init();
	}
}
