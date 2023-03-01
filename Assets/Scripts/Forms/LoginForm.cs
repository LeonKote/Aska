using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
    private InputField inputField;
    private GameObject errorText;

    public void Init()
    {
        inputField = transform.GetChild(0).GetComponent<InputField>();
        errorText = transform.GetChild(2).gameObject;
    }

    public void OnLogin()
    {
		if (!Regex.IsMatch(inputField.text, "^[A-Za-zР-пр-џ0-9_]{3,18}$"))
		{
			errorText.SetActive(true);
			inputField.text = "";
			return;
		}

		LocalClient.Send("auth", inputField.text);
	}
}
