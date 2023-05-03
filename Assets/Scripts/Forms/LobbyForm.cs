using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyForm : MonoBehaviour
{
	public InputField InputField;
	public Dropdown Dropdown;
	private Quiz[] quizzes;
	public GameObject menuForm;
	public GameObject settingsForm;
	public GameObject enterLobbyForm;
	public GameObject createLobbyForm;
	public GameObject quizEditorForm;

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
	public void OnJoinRoom()
	{
		LocalClient.Send("join", int.Parse(InputField.text));
	}

	public void OnCreateRoom()
	{
		if (Dropdown.value == 0) return;
		LocalClient.Send("create", quizzes[Dropdown.value - 1].id);
	}

	public void AddQuizzes(Quiz[] quizzes)
	{
		this.quizzes = quizzes;
		Dropdown.AddOptions(quizzes.Select(x => x.name.Substring(0, Mathf.Min(x.name.Length, 25))).ToList());
	}
}
