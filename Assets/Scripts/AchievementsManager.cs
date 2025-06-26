using System.Collections.Generic;
using GooglePlayGames;
using Newtonsoft.Json;
using UnityEngine;

public class AchievementsManager
{
	private readonly Dictionary<string, string> _achievements;

	public AchievementsManager()
	{
		TextAsset raw = ResourceLoader.GetResource<TextAsset>("Achievements");
		_achievements = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw.text);
	}

	public void Reward(string achievement, float progress)
	{
		PlayGamesPlatform.Instance.ReportProgress(_achievements[achievement], progress, success =>
		{
			if (success)
			{
				// handle
			}
			else
			{
				// handle
			}
		});
	}

	public static void ShowGPAchievements()
	{
		PlayGamesPlatform.Instance.ShowAchievementsUI();
	}
}