using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameForm : MonoBehaviour
{
	[Header("References")]
	public GameObject QuizForm;
	public GameObject QuizThemeForm;
	public GameObject QuizQuestionForm;
	public GameObject ScoreboardForm;
	public GameObject ScoreboardPlayerPrefab;
	public RoomForm Room;

	[Header("Quiz Theme Form")]
	public Text QuizThemeText;

	[Header("Quiz Question Form")]
	public Text CountdownText;
	public Text QuestionText;

	[Header("Quiz form")]
	public Text QuizTitle;
	public GameObject[] QuizAnswerButtons; // подразумевается 4 варианта ответа, в будущем можно сделать более гибко
	public RawImage QuizImage;
	public Text QuizTimerText;

	private QuizQuestion roundQuestion;
	private bool roundStarted;
	private float timeForAnswer;
	private float timeForRoundStart;
	private bool countdownForRoundStart;
	private int answer;
	public Color32 rightAnswerColor;
	public Color32 wrongAnswerColor;
	public Color32[] defaultButtonColors;

	private Dictionary<int, int> scoreboard = new Dictionary<int, int>();

	void Update()
	{
		if (countdownForRoundStart)
		{
			timeForRoundStart -= Time.deltaTime;
			SetCountdownText();
			if (timeForRoundStart <= 0)
			{
				OnRoundStarted();
				countdownForRoundStart = false;
			}

		}
		if (!roundStarted)
			return;

		if (timeForAnswer > 0)
		{
			timeForAnswer -= Time.deltaTime;
			SetTimerText();
		}
		else
		{
			SetAnswerButtonsInteractable(false);
			roundStarted = false;
		}
	}

	public void OnTimerStart()
	{

	}

	public void OnGameStart()
	{
		// QuizTheme.text = ...
		QuizThemeForm.SetActive(true);
	}

	public void CountdownForRoundStart(QuizQuestion question)
	{
		roundQuestion = question;
		QuestionText.text = roundQuestion.question;
		timeForRoundStart = 3;
		QuizThemeForm.SetActive(false);
		ScoreboardForm.SetActive(false);
		QuizQuestionForm.SetActive(true);
		countdownForRoundStart = true;
	}

	public void SetCountdownText()
	{
		CountdownText.text = Math.Round(timeForRoundStart).ToString();
	}

	public void OnRoundStarted()
	{
		roundStarted = true;
		SetAnswerButtonsInteractable(true);
		ScoreboardForm.SetActive(false);
		QuizQuestionForm.SetActive(false);
		QuizForm.SetActive(true);

		QuizTitle.text = roundQuestion.question;

		for (int i = 0; i < QuizAnswerButtons.Length; i++)
		{
			QuizAnswerButtons[i].transform.GetChild(0).GetComponent<Text>().text = roundQuestion.answers[i];
			QuizAnswerButtons[i].transform.GetComponent<Image>().color = defaultButtonColors[i];
		}

		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(Convert.FromBase64String(roundQuestion.image));
		QuizImage.texture = tex;

		timeForAnswer = roundQuestion.time;
	}

	public void OnRightAnswer(int id)
	{
		roundStarted = false;
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
			// выдать какое-нибудь сообщение о том что игрок крутой
		}
		else
		{
			// выдать какое-нибудь сообщение о том что игрок полный лузер
		}
	}

	public void OnRoundEnded(Dictionary<int, int> scoreboard)
	{
		answer = -1;
		QuizForm.SetActive(false);
		ScoreboardForm.SetActive(true);

		ClearScoreboard();
		this.scoreboard = scoreboard;

		foreach (int id in scoreboard.Keys)
		{
			GameObject scoreboardPlayer = Instantiate(ScoreboardPlayerPrefab, ScoreboardForm.transform.GetChild(1));
			scoreboardPlayer.transform.GetChild(0).GetComponent<Text>().text = Room.Clients[id].name;
			scoreboardPlayer.transform.GetChild(2).GetComponent<Text>().text = scoreboard[id].ToString();
		}
	}

	public void OnGameEnded()
	{
		
	}

	public void SetTimerText()
	{
		QuizTimerText.text = Math.Round(timeForAnswer).ToString();
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

	public void ClearScoreboard()
	{
		foreach (Transform scoreboardPlayer in ScoreboardForm.transform.GetChild(1))
			Destroy(scoreboardPlayer.gameObject);
	}
}
