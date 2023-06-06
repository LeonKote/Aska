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
		GameCountdown,
		RoundCountdown,
		InGame
	}

	[Header("References")]
	public GameObject QuizThemeForm;
	public GameObject QuizQuestionForm;
	public GameObject QuizForm;
	public GameObject ScoreboardForm;
	public GameObject LobbyForm;
	public GameObject GameEndForm;
	public LobbyForm LobbyFormScript;
	public GameEndForm gameEndScript;
	private ScoreboardForm scoreboard;

	[Header("Quiz Theme Form")]
	public Text QuizThemeText;

	[Header("Quiz Question Form")]
	public Animator quizQuestionFormAnimator;
	public Text CountdownText;
	public Text GameCountdownText;
	public Text QuestionText;
	public Text QuestionCounterText;

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

	private int questionCounter;
	private QuizQuestion roundQuestion;
	private State state;
	private float timerTime;
	private float tickingSoundTime;
	private bool tickWithAnotherSound;
	private int answer;

	void Start()
	{
		scoreboard = ScoreboardForm.GetComponent<ScoreboardForm>();
	}

	void Update()
	{
		if (state != State.None)
			timerTime -= Time.deltaTime;
		switch (state)
		{
			case State.GameCountdown:
				SetGameCountdownText();
				if (timerTime <= 0)
					state = State.None;
				break;
			case State.RoundCountdown:
				SetRoundCountdownText();
				if (timerTime <= 0.5f)
					quizQuestionFormAnimator.Play("QuizQuestionDisappearing");
				if (timerTime <= 0)
				{
					OnRoundStarted();
					state = State.InGame;
				}
				break;
			case State.InGame:
				SetTimerText();
				if (timerTime <= 0)
				{
					SetAnswerButtonsInteractable(false);
					state = State.None;
				}
				tickingSoundTime += Time.deltaTime;
				if (tickingSoundTime >= 1)
				{
					SoundController.instance.PlayShortClip(tickWithAnotherSound ? "timertick2" : "timertick1");
					tickWithAnotherSound = !tickWithAnotherSound;
					tickingSoundTime = 0;
				}
				break;
		}
	}

	public void OnTimerStart()
	{
		state = State.GameCountdown;
		timerTime = 3.5f;
	}

	public void OnGameStart(string name)
	{
		questionCounter = 0;
		QuizThemeText.text = name;
		QuizThemeForm.SetActive(true);
		ScoreboardForm.SetActive(false);
		QuizQuestionForm.SetActive(false);
		GameCountdownText.text = "Игра скоро начнётся...";
	}

	public void CountdownForRoundStart(QuizQuestion question)
	{
		++questionCounter;
		QuestionCounterText.text = $"{questionCounter} из 1"; // TODO: принимать с сервера количество вопросов и обновлять этот текст соответствующе
		roundQuestion = question;
		QuestionText.text = question.question;
		timerTime = question.countdown;

		QuizTitle.text = question.question;

		for (int i = 0; i < QuizAnswerButtons.Length; i++)
		{
			QuizAnswerButtons[i].transform.GetChild(0).GetComponent<Text>().text = question.answers[i];
			QuizAnswerButtons[i].transform.GetComponent<Image>().color = defaultButtonColors[i];
		}

		StartCoroutine(Utils.LoadImage((Texture t) => QuizImage.texture = t, question.image));
		QuizImage.GetComponent<ResizeRawImage>().AdjustSize();
		QuizThemeForm.SetActive(false);
		ScoreboardForm.SetActive(false);
		GameEndForm.SetActive(false);
		QuizQuestionForm.SetActive(true);

		state = State.RoundCountdown;
	}

	public void SetRoundCountdownText()
	{
		CountdownText.text = Mathf.Round(timerTime).ToString();
	}

	public void SetGameCountdownText()
	{
		GameCountdownText.text = $"Игра начнётся через {Math.Round(timerTime)}...";
	}

	public void OnRoundStarted()
	{
		timerTime = roundQuestion.time;
		SetAnswerButtonsInteractable(true);

		QuizForm.SetActive(true);
		tickingSoundTime = 1;
		tickWithAnotherSound = false;
		state = State.InGame;
	}

	public void OnRightAnswer(int id)
	{
		state = State.None;
		SetAnswerButtonsInteractable(false);

		if (answer == -1)
			return;

		for (int i = 0; i < QuizAnswerButtons.Length; ++i)
			QuizAnswerButtons[i].GetComponent<Image>().color = i == id ? rightAnswerColor : wrongAnswerColor;

		if (answer == id)
			Infobox.instance.ShowInfo(rightAnswerMessages[UnityEngine.Random.Range(0, rightAnswerMessages.Length)], InfoType.green);
		else
			Infobox.instance.ShowInfo(wrongAnswerMessages[UnityEngine.Random.Range(0, wrongAnswerMessages.Length)], InfoType.red);
	}

	public void OnRoundEnded(Dictionary<int, int> score)
	{
		answer = -1;
		QuizForm.SetActive(false);
		ScoreboardForm.SetActive(true);

		scoreboard.UpdateScore(score);
	}

	public void OnBackButtonPressed()
	{
		Transition.Instance.StartAnimation(() =>
		{
			gameObject.SetActive(false);
			LobbyForm.SetActive(true);
			LobbyFormScript.enterLobbyForm.SetActive(false);
			LobbyFormScript.createLobbyForm.SetActive(false);
			LobbyFormScript.menuForm.SetActive(true);
			LobbyFormScript.activeForm = LobbyFormScript.menuForm;
		});
	}

	public void OnGameEnded()
	{
		ScoreboardForm.SetActive(false);
		gameEndScript.SetUpForm();
		GameEndForm.SetActive(true);
	}

	public void SetTimerText()
	{
		QuizTimerText.text = Mathf.Floor(timerTime).ToString();
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
