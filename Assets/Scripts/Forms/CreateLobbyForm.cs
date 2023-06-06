using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyForm : MonoBehaviour
{
	public LobbyForm lobbyForm;

	public GameObject quizObject;
	public Transform quizObjectParent;
	public Sprite blankSprite;
	public InputField searchInputField;
	public Dictionary<string, Texture> cachedQuizIcons = new Dictionary<string, Texture>();
	public void OnFormOpened()
	{
		searchInputField.text = string.Empty;
		InstantiateQuizButtons(lobbyForm.Quizzes);
		LocalClient.Send("searchQuiz", searchInputField.text);
	}

	public void OnInputFieldChanged()
	{
		LocalClient.Send("searchQuiz", searchInputField.text);
	}

	public void InstantiateQuizButtons(Quiz[] quizzes)
	{
		for (int i = 0; i < quizObjectParent.childCount; ++i)
			Destroy(quizObjectParent.GetChild(i).gameObject);
		foreach (Quiz quiz in quizzes)
		{
			GameObject obj = Instantiate(quizObject, quizObjectParent);

			if (quiz.image != null)
			{
				if (cachedQuizIcons.ContainsKey(quiz.id))
				{
					obj.transform.GetChild(0).GetComponent<RawImage>().texture = cachedQuizIcons[quiz.id];
				}
				else
				{
					GameController.instance.StartCoroutine(Utils.LoadImage((Texture t) =>
					{
						cachedQuizIcons.Add(quiz.id, t);
						obj.transform.GetChild(0).GetComponent<RawImage>().texture = t;
					}, quiz.image));
				}
			}
			else
				obj.transform.GetChild(0).GetComponent<RawImage>().texture = blankSprite.texture;

			obj.transform.GetChild(1).GetComponent<Text>().text =
				string.IsNullOrEmpty(quiz.name) ? "Без названия" : quiz.name;

			obj.transform.GetChild(2).GetComponent<Text>().text =
				string.IsNullOrEmpty(quiz.description) ? "Викторина без описания." : quiz.description;

			obj.name = quiz.id;
			obj.GetComponent<Button>().onClick.AddListener(delegate { OnQuizPressed(quizObjectParent.GetChild(obj.transform.GetSiblingIndex()).name); });
		}
	}

	public void OnQuizPressed(string id)
	{
		LocalClient.Send("create", id);
	}
}
