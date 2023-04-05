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
