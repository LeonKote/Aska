using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System;

public class ProfileEditorForm : MonoBehaviour
{
	public LobbyForm lobbyForm;
	public string avatarText;
	public string nickname;
	public RawImage avatarImage;
	public Sprite blankSprite;
	public InputField nicknameInputField;
	public void OnFormOpened()
	{
		nicknameInputField.text = nickname;
		if (avatarText != string.Empty)
		{
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(Convert.FromBase64String(avatarText));
			avatarImage.texture = tex;
		}
		else
			avatarImage.texture = blankSprite.texture;
	}
	public void UploadAvatar()
	{
		var path = StandaloneFileBrowser.OpenFilePanel("Выбор аватарки", "", new ExtensionFilter[] { new("Image ", "png", "jpg", "jpeg") }, false);
		if (path.Length > 0)
		{
			// TODO: ограничение по весу файла
			Texture2D texture = new WWW(new Uri(path[0]).AbsoluteUri).texture;
			avatarText = Convert.ToBase64String(texture.EncodeToPNG());
			avatarImage.texture = texture;
		}
		lobbyForm.UpdateProfileUI(nickname, avatarText);
		// TODO: выгрузка аватарки на сервер
	}
	public void ChangeNickname()
	{
		if (!Regex.IsMatch(nicknameInputField.text, "^[A-Za-zА-Яа-я0-9_]{3,18}$"))
		{
			Infobox.instance.ShowInfo("Имя может содержать только буквы/цифры длиной от 3 до 18 символов.", InfoType.red);
			return;
		}
		nickname = nicknameInputField.text;
		lobbyForm.UpdateProfileUI(nickname, avatarText);
		// TODO: выгрузка никнейма на сервер
	}
}
