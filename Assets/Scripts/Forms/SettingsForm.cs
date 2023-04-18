using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsForm : MonoBehaviour
{
    public AudioListener listener;
	public Toggle soundToggle;
    public void OnSoundTogglePressed()
	{
		listener.enabled = soundToggle.isOn;
	}
}
