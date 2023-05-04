using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System;

public class ProfileEditorForm : MonoBehaviour
{
	public string avatarText;
	public string nickname;
	public RawImage avatarImage;

	public InputField nicknameInputField;
	public void OnFormOpened()
	{
		nicknameInputField.text = nickname;
	}
	public void UploadAvatar()
	{
		var path = StandaloneFileBrowser.OpenFilePanel("Аватар", "", new ExtensionFilter[] { new("Image ", "png", "jpg", "jpeg") }, false);
		if (path.Length > 0)
		{
			// TODO: ограничение по размеру файла
			Texture2D texture = new WWW(new Uri(path[0]).AbsoluteUri).texture;
			avatarText = Convert.ToBase64String(texture.EncodeToPNG());
			avatarImage.texture = texture;
		}
	}
	public void ChangeNickname()
	{
		nickname = nicknameInputField.text;
		// TODO: отправка на сервер нового ника
	}
}
