using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameForm : MonoBehaviour
{
	private enum State
	{
		None,
		Countdown,
		InGame
	}

	[Header("References")]
	public GameObject QuizThemeForm;
	public GameObject QuizQuestionForm;
	public GameObject QuizForm;
	public GameObject ScoreboardForm;

	[Header("Quiz Theme Form")]
	public Text QuizThemeText;

	[Header("Quiz Question Form")]
	public Text CountdownText;
	public Text QuestionText;

	[Header("Quiz Form")]
	public Text QuizTitle;
	public GameObject[] QuizAnswerButtons; // подразумевается 4 варианта ответа, в будущем можно сделать более гибко
	public RawImage QuizImage;
	public Text QuizTimerText;

	public Color32 rightAnswerColor;
	public Color32 wrongAnswerColor;
	public Color32[] defaultButtonColors;
	public string[] rightAnswerMessages;
	public string[] wrongAnswerMessages;

	private QuizQuestion roundQuestion;
	private State state;
	private float timerTime;
	private int answer;

	private ScoreboardForm scoreboard;

	void Start()
	{
		scoreboard = ScoreboardForm.GetComponent<ScoreboardForm>();
	}

	void Update()
	{
		switch (state)
		{
			case State.Countdown:
				timerTime -= Time.deltaTime;
				SetCountdownText();
				if (timerTime <= 0)
				{
					OnRoundStarted();
					state = State.InGame;
				}
				break;
			case State.InGame:
				timerTime -= Time.deltaTime;
				SetTimerText();
				if (timerTime <= 0)
				{
					SetAnswerButtonsInteractable(false);
					state = State.None;
				}
				break;
		}
	}

	public void OnTimerStart()
	{

	}

	public void OnGameStart(string name)
	{
		QuizThemeText.text = name;
		QuizThemeForm.SetActive(true);
	}

	public void CountdownForRoundStart(QuizQuestion question)
	{
		roundQuestion = question;
		QuestionText.text = question.question;
		timerTime = question.countdown;

		QuizTitle.text = question.question;

		for (int i = 0; i < QuizAnswerButtons.Length; i++)
		{
			QuizAnswerButtons[i].transform.GetChild(0).GetComponent<Text>().text = question.answers[i];
			QuizAnswerButtons[i].transform.GetComponent<Image>().color = defaultButtonColors[i];
		}

		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(Convert.FromBase64String(question.image));
		QuizImage.texture = tex;

		QuizThemeForm.SetActive(false);
		ScoreboardForm.SetActive(false);
		QuizQuestionForm.SetActive(true);

		state = State.Countdown;
	}

	public void SetCountdownText()
	{
		CountdownText.text = Mathf.Round(timerTime).ToString();
	}

	public void OnRoundStarted()
	{
		timerTime = roundQuestion.time;
		SetAnswerButtonsInteractable(true);

		QuizQuestionForm.SetActive(false);
		QuizForm.SetActive(true);

		state = State.InGame;
	}

	public void OnRightAnswer(int id)
	{
		state = State.None;
		SetAnswerButtonsInteractable(false);

		if (answer == -1)
			return;
		for (int i = 0; i < QuizAnswerButtons.Length; ++i)
		{
			if (i == id)
				QuizAnswerButtons[i].GetComponent<Image>().color = rightAnswerColor;
			else
				QuizAnswerButtons[i].GetComponent<Image>().color = wrongAnswerColor;
		}
		if (answer == id)
		{
			Infobox.instance.ShowInfo(rightAnswerMessages[UnityEngine.Random.Range(0, rightAnswerMessages.Length)], InfoType.green);
		}
		else
		{
			Infobox.instance.ShowInfo(wrongAnswerMessages[UnityEngine.Random.Range(0, wrongAnswerMessages.Length)], InfoType.red);
		}
	}

	public void OnRoundEnded(Dictionary<int, int> score)
	{
		answer = -1;
		QuizForm.SetActive(false);
		ScoreboardForm.SetActive(true);

		scoreboard.UpdateScore(score);
	}

	public void OnGameEnded()
	{

	}

	public void SetTimerText()
	{
		QuizTimerText.text = Mathf.Round(timerTime).ToString();
	}

	public void OnAnswerButtonPressed(int answerIndex)
	{
		LocalClient.Send("answer", answerIndex);
		answer = answerIndex;

		SetAnswerButtonsInteractable(false);
	}

	public void SetAnswerButtonsInteractable(bool enable)
	{
		foreach (GameObject quizAnswerButton in QuizAnswerButtons)
			quizAnswerButton.GetComponent<Button>().interactable = enable;
	}
}
