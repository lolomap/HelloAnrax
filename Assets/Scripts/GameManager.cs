using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorUtilities/GameManager", menuName = "Game/Manager")]
public class GameManager : ScriptableObject
{
	private static GameManager _instance;
	
	// Options
	// public bool exampleOption;
	
	public static EventStorage EventStorage { get; private set; }
	public static PlayerStats PlayerStats { get; private set; }
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ReloadGame()
	{
		_instance = Resources.Load<GameManager>("EditorUtilities/GameManager");
		
		PlayerStats = new();
		PlayerStats.Init();
		
		EventStorage = new();
		EventStorage.Load();
	}
}
