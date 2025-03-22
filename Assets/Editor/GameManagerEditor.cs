using UnityEditor;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeNullComparison

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
	private GameManager _manager;

	public void OnEnable()
	{
		_manager = (GameManager) target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		_manager.TimedEventTurn = EditorGUILayout.IntField("Timed Event Turn", _manager.TimedEventTurn);
		bool isReady = EditorGUILayout.LinkButton("Run Timed Event");
		if (isReady && _manager.EventUI != null)
		{
			GameEvent gameEvent = GameManager.EventStorage.GetTimedByTurn(_manager.TimedEventTurn);
			if (gameEvent != null)
			{
				_manager.EventUI.Data = gameEvent;
				_manager.EventUI.UpdateCard();
				GameManager.PlayerStats.SetStat("TURN", _manager.TimedEventTurn);
			}
			else
			{
				Debug.Log("TIMED EVENT NOT FOUND");
			}
		}
	}
}