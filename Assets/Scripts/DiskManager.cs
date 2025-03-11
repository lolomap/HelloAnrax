using UnityEngine;

public class DiskManager : MonoBehaviour
{
	private void SaveAllData()
	{
		GameManager.PlayerStats.SaveGlobal();
		GameManager.PlayerStats.Save();
		
		GameManager.EventStorage.Save();
		
		PlayerPrefs.Save();
	}
	
	private void OnApplicationQuit()
	{
		SaveAllData();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		SaveAllData();
	}
}