using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public SettingsForm settingsForm;
	public Transition transition;
	public static GameController instance;

	public void Init()
	{
		settingsForm.Init();
		transition.Init();
	}
	public void Awake()
	{
		instance = this;
		Init();
	}

	public void Start()
	{
		SoundController.instance.PlayMusic("lobby", true);
	}

	public void Quit()
	{
		DiscordController.instance.ClearActivity();
		Application.Quit();
	}

	public void OnApplicationQuit()
	{
		DiscordController.instance.ClearActivity();
	}
}
