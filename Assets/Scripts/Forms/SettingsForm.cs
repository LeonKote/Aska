using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsForm : MonoBehaviour
{
	public static SettingsForm instance;
	public SoundController soundController;
	public Toggle soundToggle;
	public Dropdown screenModeDropdown;
	public void Init()
	{
		instance = this;
		if (PlayerPrefs.HasKey("settings_sound"))
		{
			bool soundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("settings_sound"));
			soundController.SetSound(soundEnabled);
			soundToggle.isOn = !soundEnabled;
		}
		else
		{
			soundController.SetSound(true);
			soundToggle.isOn = true;
		}
		if (PlayerPrefs.HasKey("settings_screenmode"))
		{
			int value = PlayerPrefs.GetInt("settings_screenmode");
			ChangeScreenMode(value);
			screenModeDropdown.value = value;
		}
	}
    public void OnSoundTogglePressed()
	{
		soundController.SetSound(!soundToggle.isOn);
		if (SoundToggleButton.instance != null)
			SoundToggleButton.instance.SetSprite();
		PlayerPrefs.SetInt("settings_sound", Convert.ToInt32(!soundToggle.isOn));
	}
	public void OnDropdownValueChanged()
	{
		int value = screenModeDropdown.value;
		PlayerPrefs.SetInt("settings_screenmode", value);
		ChangeScreenMode(value);
	}
	public void ChangeScreenMode(int value)
	{
		switch (value)
		{
			case 0:
				Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
				break;
			case 1:
				Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.ExclusiveFullScreen);
				break;
			case 2:
				Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.Windowed);
				break;
		}
	}
}
