using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
	public InputField InputField;
	public GameObject ErrorText;

	public void OnLogin()
	{
		if (!Regex.IsMatch(InputField.text, "^[A-Za-zР-пр-џ0-9_]{3,18}$"))
		{
			ErrorText.SetActive(true);
			InputField.text = "";
			return;
		}

		LocalClient.Send("auth", InputField.text);
	}
}
