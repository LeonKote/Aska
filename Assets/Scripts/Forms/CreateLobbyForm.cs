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
	public void OnFormOpened()
	{
		searchInputField.text = string.Empty;
		InstantiateQuizButtons(lobbyForm.Quizzes);
	}
	public void OnInputFieldChanged()
	{
		string query = searchInputField.text.ToLower();
		List<Quiz> foundQuizzes = new List<Quiz>();
		for (int i = 0; i < lobbyForm.Quizzes.Length; ++i)
		{
			if (lobbyForm.Quizzes[i].name.ToLower().Contains(query))
				foundQuizzes.Add(lobbyForm.Quizzes[i]);
		}
		InstantiateQuizButtons(foundQuizzes.ToArray());
	}
	public void InstantiateQuizButtons(Quiz[] quizzes)
	{
		for (int i = 0; i < quizObjectParent.childCount; ++i)
			Destroy(quizObjectParent.GetChild(i).gameObject);
		for (int i = 0; i < quizzes.Length; ++i)
		{
			GameObject obj = Instantiate(quizObject, quizObjectParent);
			if (quizzes[i].image != null)
			{
				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(Convert.FromBase64String(quizzes[i].image));
				obj.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
			}
			else
				obj.transform.GetChild(0).GetComponent<RawImage>().texture = blankSprite.texture;
			obj.transform.GetChild(1).GetComponent<Text>().text =
				quizzes[i].name == string.Empty ? "Без названия" : quizzes[i].name;

			obj.transform.GetChild(2).GetComponent<Text>().text =
				quizzes[i].description == string.Empty ? "Викторина без описания." : quizzes[i].description;
			obj.name = quizzes[i].id;
			obj.GetComponent<Button>().onClick.AddListener(delegate { OnQuizPressed(quizObjectParent.GetChild(obj.transform.GetSiblingIndex()).name); });
		}
	}
	public void OnQuizPressed(string id)
	{;
		LocalClient.Send("create", id);
	}
}
