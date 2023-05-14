using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public SettingsForm settingsForm;
	public Transition transition;

	public void Init()
	{
		settingsForm.Init();
		transition.Init();
	}
	public void Awake()
	{
		Init();
	}
	public void Quit()
	{
		Application.Quit();
	}
}
