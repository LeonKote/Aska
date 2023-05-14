using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyForm : MonoBehaviour
{
	public InputField InputField;
	public Dropdown Dropdown;
	public Quiz[] Quizzes;
	public GameObject menuForm;
	public GameObject settingsForm;
	public GameObject enterLobbyForm;
	public GameObject createLobbyForm;
	public GameObject quizEditorForm;
	public GameObject profileEditorForm;

	public Text nicknameText;
	public RawImage avatarImage;
	public Sprite blankAvatarSprite;

	public GameObject activeForm;

	public void Start()
	{
		activeForm = menuForm;
	}

	public void OnBackButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			activeForm.SetActive(false);
			menuForm.SetActive(true);
			activeForm = menuForm;
		});
	}
	public void OnSettingsButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			menuForm.SetActive(false);
			settingsForm.SetActive(true);
			activeForm = settingsForm;
		});
	}
	public void OnEnterLobbyButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			menuForm.SetActive(false);
			enterLobbyForm.SetActive(true);
			activeForm = enterLobbyForm;
		});
	}

	public void OnQuizEditorButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			menuForm.SetActive(false);
			quizEditorForm.SetActive(true);
			activeForm = quizEditorForm;
		});
	}

	public void OnCreateLobbyButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			menuForm.SetActive(false);
			createLobbyForm.SetActive(true);
			activeForm = createLobbyForm;
		});
	}

	public void OnProfileEditorButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			menuForm.SetActive(false);
			profileEditorForm.SetActive(true);
			activeForm = profileEditorForm;
		});
	}

	public void OnJoinRoom()
	{
		LocalClient.Send("join", int.Parse(InputField.text));
	}

	public void UpdateProfileUI(string nickname, string avatar = null)
	{
		nicknameText.text = nickname;
		if (avatar != null)
		{
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(Convert.FromBase64String(avatar));
			avatarImage.texture = tex;
		}
		else
			avatarImage.texture = blankAvatarSprite.texture;
	}

	public void AddQuizzes(Quiz[] quizzes)
	{
		this.Quizzes = quizzes;
	}
}
