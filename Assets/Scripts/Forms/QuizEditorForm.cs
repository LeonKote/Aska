using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

public class QuizEditorForm : MonoBehaviour
{
	public GameObject editorForm;
	public GameObject mainForm;
	public GameObject exitConfirmPanel;
	public GameObject quizSelectionForm;
	public GameObject quizObject;

	public LocalClient localClient;

	public Transform quizObjectParent;

	public Text questionsCounterText;
	public InputField quizNameInputField;
	public InputField quizDescriptionInputField;
	public RawImage quizImage;
	public Sprite blankImageSprite;
	public Sprite blankQuizSprite;

	public InputField questionTextInputField;
	public InputField[] questionAnswerInputFields;
	public InputField countDownToStartInputField;
	public InputField questionTimeInputField;
	public Toggle[] questionAnswerToggles;
	public RawImage questionImage;

	private string quizName;
	private string quizDescription;
	private string quizImageText;

	private int selectedQuestionIndex = 0;
	private bool newQuiz = true;
	private int selectedQuizIndex = 0;
	private bool callBlocker = false;

	public List<Quiz> quizzes = new();
	public List<QuizQuestion> questions = new();

	public void Start()
	{
		// TODO: �������� ����������� �������� �� �����
	}

	public void OnQuizSelectionButtonPressed()
	{
		InstantiateQuizButtons();
		Transition.Instance.StartAnimation(() =>
		{
			mainForm.SetActive(false);
			quizSelectionForm.SetActive(true);
		});
	}

	public void InstantiateQuizButtons()
	{
		for (int i = 0; i < quizObjectParent.childCount; ++i)
			Destroy(quizObjectParent.GetChild(i).gameObject);
		for (int i = 0; i < quizzes.Count; ++i)
		{
			GameObject obj = Instantiate(quizObject, quizObjectParent);
			if (!string.IsNullOrEmpty(quizzes[i].image))
			{
				Texture2D texture = new Texture2D(2, 2);
				texture.LoadImage(Convert.FromBase64String(quizzes[i].image));
				obj.transform.GetChild(0).GetComponent<RawImage>().texture = texture;
			}
			else
				obj.transform.GetChild(0).GetComponent<RawImage>().texture = blankQuizSprite.texture;
			obj.transform.GetChild(0).GetComponent<ResizeRawImage>().AdjustSize();
			obj.transform.GetChild(1).GetComponent<Text>().text =  string.IsNullOrEmpty(quizzes[i].name) ? "��� ��������" : quizzes[i].name;
			obj.transform.GetChild(2).GetComponent<Text>().text = string.IsNullOrEmpty(quizzes[i].description) ? "��������� ��� ��������." : quizzes[i].description;
			obj.GetComponent<Button>().onClick.AddListener(delegate { LoadExistingQuiz(obj.transform.GetSiblingIndex()); });
		}
	}

	public void UpdateToolbarUI()
	{
		if (quizImageText != string.Empty)
		{
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(Convert.FromBase64String(quizImageText));
			quizImage.texture = texture;
		}
		else
			quizImage.texture = blankImageSprite.texture;
		quizImage.GetComponent<ResizeRawImage>().AdjustSize();
		
		quizNameInputField.text = quizName;
		quizDescriptionInputField.text = quizDescription;
	}

	public void OnQuizNameInputField()
	{
		quizName = quizNameInputField.text;
	}

	public void OnQuizDescriptionInputField()
	{
		quizDescription = quizDescriptionInputField.text;
	}

	public void OnQuestionTextInputField()
	{
		questions[selectedQuestionIndex].question = questionTextInputField.text;
	}

	public void OnCountdownInputField()
	{
		int result;
		if (int.TryParse(countDownToStartInputField.text, out result))
			questions[selectedQuestionIndex].countdown = result;
		else
			questions[selectedQuestionIndex].countdown = 0;
	}

	public void OnQuestionTimeInputField()
	{
		int result;
		if (int.TryParse(questionTimeInputField.text, out result))
			questions[selectedQuestionIndex].time = result;
		else
			questions[selectedQuestionIndex].time = 0;
	}

	public void OnQuizImagePressed()
	{
		var path = StandaloneFileBrowser.OpenFilePanel("�������� ���������", "", new ExtensionFilter[] { new ("Image ", "png", "jpg", "jpeg") }, false);
		if (path.Length > 0)
		{
			// TODO: ����������� �� ������� �����
			byte[] bytes = File.ReadAllBytes(path[0]);
			quizImageText = Convert.ToBase64String(bytes);

			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(bytes);
			quizImage.texture = texture;
			quizImage.GetComponent<ResizeRawImage>().AdjustSize();
		}
	}

	public void OnQuestionImagePressed()
	{
		var path = StandaloneFileBrowser.OpenFilePanel("�������� �������", "", new ExtensionFilter[] { new("Image ", "png", "jpg", "jpeg") }, false);
		if (path.Length > 0)
		{
			// TODO: ����������� �� ������� �����
			byte[] bytes = File.ReadAllBytes(path[0]);
			questions[selectedQuestionIndex].image = Convert.ToBase64String(bytes);

			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(bytes);
			questions[selectedQuestionIndex].icon = texture;
			questionImage.texture = texture;
			questionImage.GetComponent<ResizeRawImage>().AdjustSize();
		}
	}

	public void ReturnToMainMenu()
	{
		Transition.Instance.StartAnimation(() =>
		{
			exitConfirmPanel.SetActive(false);
			quizSelectionForm.SetActive(false);
			editorForm.SetActive(false);
			mainForm.SetActive(true);
		});
	}

	public void OnQuestionAnswerInputField(string index)
	{
		int _index = int.Parse(index);
		questions[selectedQuestionIndex].answers[_index] = questionAnswerInputFields[_index].text;
	}

	public void OnQuestionAnswerToggle(string index)
	{
		if (callBlocker)
			return;
		int _index = int.Parse(index);
		for (int i = 0; i < questionAnswerToggles.Length; ++i)
		{
			if (i != _index)
			{
				var temp = questionAnswerToggles[i].onValueChanged; // �������, ����� ��������� ���� ����� �� ������ ����� ������ ���
				questionAnswerToggles[i].onValueChanged = new Toggle.ToggleEvent();
				questionAnswerToggles[i].isOn = false;
				questionAnswerToggles[i].onValueChanged = temp;
			}
				
		}
		if (!questionAnswerToggles[_index].isOn)
		{
			questions[selectedQuestionIndex].rightAnswerIndex = -1;
			return;
		}
		
		questions[selectedQuestionIndex].rightAnswerIndex = _index;
	}

	public void CreateNewQuiz()
	{
		newQuiz = true;
		questions.Clear();
		quizName = string.Empty;
		quizDescription = string.Empty;
		quizImageText = string.Empty;
		quizImage.texture = blankImageSprite.texture;
		selectedQuestionIndex = 0;
		UpdateToolbarUI();
		CreateNewQuestion();
		Transition.Instance.StartAnimation(() =>
		{
			mainForm.SetActive(false);
			editorForm.SetActive(true);
		});
	}

	public void SaveQuiz()
	{
		// TODO: ���������� � ����
		if (newQuiz)
		{
			quizzes.Add(new Quiz(quizName, quizDescription, quizImageText, questions));
			newQuiz = false;
			selectedQuizIndex = quizzes.Count - 1;
			return;
		}
		quizzes[selectedQuizIndex].name = quizName;
		quizzes[selectedQuizIndex].description = quizDescription;
		quizzes[selectedQuizIndex].image = quizImageText;
		quizzes[selectedQuizIndex].questions = questions; 
	}

	public void SendToModeration()
	{
		SaveQuiz();
		if (!localClient.isAuthorizedBySocial)
		{
			Infobox.instance.ShowInfo("�� �� ������������.", InfoType.red);
			return;
		}
		if (IsQuizCorrectCheck(quizzes[selectedQuizIndex]))
		{
			// TODO: ��������� �� ���������, �������� ����� �������� ������� ��������� � �������?
			LocalClient.Send("userQuiz", JObject.FromObject(quizzes[selectedQuizIndex]));
			Infobox.instance.ShowInfo("��������� ���������� �� ���������.", InfoType.green);
		}
	}

	public bool IsQuizCorrectCheck(Quiz quiz)
	{
		if (quizName == string.Empty)
		{
			Infobox.instance.ShowInfo("�� ������� �������� ���������.", InfoType.red);
			return false;
		}
		if (quizDescription == string.Empty)
		{
			Infobox.instance.ShowInfo("�� ������� �������� ���������.", InfoType.red);
			return false;
		}
		if (quizImageText == string.Empty)
		{
			Infobox.instance.ShowInfo("�� ��������� �������� ���������.", InfoType.red);
			return false;
		}
		for (int i = 0; i < quiz.questions.Count; ++i)
		{
			if (quiz.questions[i].question == string.Empty)
			{
				Infobox.instance.ShowInfo($"������ #{i + 1}: ��� ������ �������.", InfoType.red);
				return false;
			}
			if (quiz.questions[i].image == string.Empty)
			{
				Infobox.instance.ShowInfo($"������ #{i + 1}: �� ��������� ��������.", InfoType.red);
				return false;
			}
			if (quiz.questions[i].rightAnswerIndex == -1)
			{
				Infobox.instance.ShowInfo($"������ #{i + 1}: �� ������ ������ �����.", InfoType.red);
				return false;
			}
			if (quiz.questions[i].countdown <= 0)
			{
				Infobox.instance.ShowInfo($"������ #{i + 1}: ������� ������� ����� ������� �� ������.", InfoType.red);
				return false;
			}
			if (quiz.questions[i].countdown <= 0)
			{
				Infobox.instance.ShowInfo($"������ #{i + 1}: ������� ������� ����� �� �����.", InfoType.red);
				return false;
			}
			foreach (var answer in quiz.questions[i].answers)
				if (answer == string.Empty)
				{
					Infobox.instance.ShowInfo($"������ #{i + 1}: �� ��� �������� ������ ���������.", InfoType.red);
					return false;
				}
		}

		return true;
	}
	
	public void LoadExistingQuiz(int index)
	{
		newQuiz = false;
		selectedQuizIndex = index;
		quizName = quizzes[index].name;
		quizDescription = quizzes[index].description;
		quizImageText = quizzes[index].image;
		questions = quizzes[index].questions;
		selectedQuestionIndex = 0;
		UpdateToolbarUI();
		UpdateQuestionCounter();
		UpdateQuestionEditorUI();
		Transition.Instance.StartAnimation(() =>
		{
			mainForm.SetActive(false);
			quizSelectionForm.SetActive(false);
			editorForm.SetActive(true);
		});
	}

	public void CreateNewQuestion()
	{
		questions.Add(new QuizQuestion(string.Empty, new string[4], string.Empty, 60, 3, -1));
		selectedQuestionIndex = questions.Count - 1;
		UpdateQuestionEditorUI();
		UpdateQuestionCounter();
	}

	public void DeleteSelectedQuestion()
	{
		if (questions.Count <= 1)
			return;
		questions.RemoveAt(selectedQuestionIndex);
		if (selectedQuestionIndex > questions.Count - 1)
			selectedQuestionIndex--;
		UpdateQuestionCounter();
		UpdateQuestionEditorUI();
	}

	public void ChangeSelectedQuestion(bool increment)
	{
		if (increment && selectedQuestionIndex == questions.Count - 1)
			return;
		if (!increment && selectedQuestionIndex == 0)
			return;
		selectedQuestionIndex = selectedQuestionIndex + (increment ? 1 : -1);

		UpdateQuestionEditorUI();
		UpdateQuestionCounter();
	}
	public void UpdateQuestionEditorUI()
	{
		questionTextInputField.text = questions[selectedQuestionIndex].question;
		questionTimeInputField.text = questions[selectedQuestionIndex].time.ToString();
		countDownToStartInputField.text = questions[selectedQuestionIndex].countdown.ToString();
		if (questions[selectedQuestionIndex].icon != null)
			questionImage.texture = questions[selectedQuestionIndex].icon;
		else
			questionImage.texture = blankImageSprite.texture;
		questionImage.GetComponent<ResizeRawImage>().AdjustSize();

		for (int i = 0; i < questionAnswerInputFields.Length; ++i)
			questionAnswerInputFields[i].text = questions[selectedQuestionIndex].answers[i];
		for (int i = 0; i < questionAnswerToggles.Length; ++i)
		{
			callBlocker = true;
			questionAnswerToggles[i].isOn = false;
			if (i == questions[selectedQuestionIndex].rightAnswerIndex)
				questionAnswerToggles[i].isOn = true;
			callBlocker = false;
		}
	}
	public void UpdateQuestionCounter()
	{
		questionsCounterText.text = $"{selectedQuestionIndex + 1} �� {questions.Count}";
	}
}
