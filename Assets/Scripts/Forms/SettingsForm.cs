using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsForm : MonoBehaviour
{
    public AudioListener listener;
	public Toggle soundToggle;
	public Dropdown screenModeDropdown;
	public void Init()
	{
		if (PlayerPrefs.HasKey("settings_sound"))
		{
			bool enabled = Convert.ToBoolean(PlayerPrefs.GetInt("settings_sound"));
			listener.enabled = enabled;
			soundToggle.isOn = enabled;
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
		listener.enabled = soundToggle.isOn;
		PlayerPrefs.SetInt("settings_sound", Convert.ToInt32(soundToggle.isOn));
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
