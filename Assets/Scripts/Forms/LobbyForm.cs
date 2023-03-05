using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyForm : MonoBehaviour
{
	public InputField inputField;

	public void OnJoinRoom()
	{
		LocalClient.Send("join", int.Parse(inputField.text));
	}

	public void OnCreateRoom()
	{
		LocalClient.Send("create", null);
	}
}
