using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
	public static SoundToggleButton instance;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public GameObject[] buttons;

	public void Awake()
	{
		instance = this;
		SetSprite();
	}

	public void OnPressed()
	{
		SettingsForm.instance.soundToggle.isOn = !SettingsForm.instance.soundToggle.isOn;
		SetSprite();
	}

	public void SetSprite()
	{
		foreach(var obj in buttons)
			obj.transform.GetChild(0).GetComponent<Image>().sprite = SettingsForm.instance.soundToggle.isOn ? soundOnSprite : soundOffSprite;
	}
}
