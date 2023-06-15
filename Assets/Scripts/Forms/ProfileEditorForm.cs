using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System;
using System.IO;

public class ProfileEditorForm : MonoBehaviour
{
	public LobbyForm lobbyForm;
	public LocalClient localClient;
	public string avatarText;
	public string nickname;
	public RawImage avatarImage;
	public Sprite blankSprite;
	public InputField nicknameInputField;
	public void UploadAvatar()
	{
		if (!localClient.isAuthorizedBySocial)
		{
			Infobox.instance.ShowInfo("Вы не авторизованы.", InfoType.red);
			return;
		}
		var path = StandaloneFileBrowser.OpenFilePanel("Выбор аватарки", "", new ExtensionFilter[] { new("Image ", "png", "jpg", "jpeg") }, false);
		if (path.Length == 0) return;
		// TODO: ограничение по весу файла
		byte[] bytes = File.ReadAllBytes(path[0]);

		Texture2D texture = new Texture2D(2, 2);
		texture.LoadImage(bytes);
		avatarImage.texture = texture;
		lobbyForm.avatarImage.texture = texture;

		LocalClient.Send("avatar", Convert.ToBase64String(bytes));
	}
	public void ChangeNickname()
	{
		if (!localClient.isAuthorizedBySocial)
		{
			Infobox.instance.ShowInfo("Вы не авторизованы.", InfoType.red);
			return;
		}
		if (!Regex.IsMatch(nicknameInputField.text, "^[A-Za-zА-Яа-я0-9_]{3,18}$"))
		{
			Infobox.instance.ShowInfo("Ник должен состоять из букв или цифр длиной от 3 до 18 символов.", InfoType.red);
			return;
		}
		lobbyForm.nicknameText.text = nicknameInputField.text;
		LocalClient.Send("name", nicknameInputField.text);
	}
}
