using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
	public InputField InputField;
	public LobbyForm LobbyForm;

	public void OnLoginName()
	{
		if (!Regex.IsMatch(InputField.text, "^[A-Za-z�-��-�0-9_]{3,18}$"))
		{
			Infobox.instance.ShowInfo("��� ������ �������� �� ���� ��� ���� ������ �� 3 �� 18 ��������.", InfoType.red);
			InputField.text = "";
			return;
		}
		LocalClient.Send("auth", new JObject()
		{
			{ "type", "name" },
			{ "name", InputField.text }
		});
	}

	public void OnLoginVK()
	{
		LocalClient.Send("auth", new JObject()
		{
			{ "type", "vk" }
		});
	}

	public void OnLoginTelegram()
	{
		LocalClient.Send("auth", new JObject()
		{
			{ "type", "tg" }
		});
	}

	public void OnLocalClientConnected()
	{
		InputField.interactable = true;
	}
}
