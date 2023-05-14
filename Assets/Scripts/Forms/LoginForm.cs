using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
	public InputField InputField;
	public LobbyForm LobbyForm;

	public void OnLogin()
	{
		if (!Regex.IsMatch(InputField.text, "^[A-Za-z�-��-�0-9_]{3,18}$"))
		{
			Infobox.instance.ShowInfo("��� ������ �������� �� ���� ��� ���� ������ �� 3 �� 18 ��������.", InfoType.red);
			InputField.text = "";
			return;
		}
		LobbyForm.UpdateProfileUI(InputField.text);
		LocalClient.Send("auth", InputField.text);
	}

	public void OnLocalClientConnected()
	{
		InputField.interactable = true;
	}
}
