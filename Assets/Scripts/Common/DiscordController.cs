using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
	public static DiscordController instance;
	private const long CLIENT_ID = 1117791881468842035;

	public Discord.Discord discord;
	public ActivityManager activityManager;

	public void Awake()
	{
		instance = this;
		discord = new Discord.Discord(CLIENT_ID, (UInt64)CreateFlags.NoRequireDiscord);
		activityManager = discord.GetActivityManager();
		UpdateActivity($"В главном меню");
	}

	public void UpdateActivity(string details, string state = "", string largeText = "", string secret = "")
	{
		var assets = new ActivityAssets();
		assets.LargeText = largeText;
		assets.LargeImage = "logo";


		var activity = new Activity
		{
			State = state,
			Details = details,
			Type = ActivityType.Playing,
			Assets = assets
		};

		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Result.Ok)
			{
				Debug.Log("Discord activity updated");
			}
			else
			{
				Debug.LogError($"Discord activity update error {res}");
			}
		});
	}

	public void ClearActivity()
	{
		activityManager.ClearActivity((res) =>
		{
			if (res == Result.Ok)
			{
				Debug.Log("Discord activity cleared");
			}
			else
			{
				Debug.LogError($"Discord activity clear error {res}");
			}
		});
	}

	public void Update()
	{
		discord.RunCallbacks();
	}

}
