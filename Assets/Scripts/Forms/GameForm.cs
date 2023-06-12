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
	public RoomForm roomFormScript;
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
	private int answer = -1;
	private bool riserPlayed;
	private bool countdownSoundPlayed;

	void Start()
	{
		scoreboard = ScoreboardForm.GetComponent<ScoreboardForm>();
	}
	// TODO: дубилурю с scoreboardform - у хоста жесткие проблемы с отображением скорборда (скорее всего список клиентов не обновляется)
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
				if (timerTime <= 1f && !riserPlayed)
				{
					SoundController.instance.PlayShortClip("riser");
					riserPlayed = true;
				}
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
				if (timerTime <= 3f && !countdownSoundPlayed)
				{
					SoundController.instance.StartCountdown(3);
					countdownSoundPlayed = true;
				}
				if (timerTime <= 0)
				{
					QuizTimerText.text = "0";
					state = State.None;
				}
				break;
		}
	}

	public void OnTimerStart()
	{
		state = State.GameCountdown;
		timerTime = 3f;
	}

	public void OnGameStart(string name)
	{
		questionCounter = 0;
		QuizThemeText.text = name;
		QuizThemeForm.SetActive(true);
		countdownSoundPlayed = false;
		ScoreboardForm.SetActive(false);
		QuizForm.SetActive(false);
		QuizQuestionForm.SetActive(false);
		GameEndForm.SetActive(false);
		SoundController.instance.SetLowPassFilter(true, 10f);
		GameCountdownText.text = "Игра скоро начнётся...";
		DiscordController.instance.UpdateActivity($"В игре #{roomFormScript.roomCode}", "Ожидание начала игры", roomFormScript.quiz.name);
	}

	public void CountdownForRoundStart(QuizQuestion question)
	{
		SoundController.instance.StopMusic();
		++questionCounter;
		QuestionCounterText.text = $"{questionCounter} из {roomFormScript.quiz.questionsCount}";
		roundQuestion = question;
		QuestionText.text = question.question;
		timerTime = question.countdown;
		DiscordController.instance.UpdateActivity($"В игре #{roomFormScript.roomCode}", 
			$"Вопрос {questionCounter} из {roomFormScript.quiz.questionsCount}", 
			roomFormScript.quiz.name);

		QuizTitle.text = question.question;

		for (int i = 0; i < QuizAnswerButtons.Length; i++)
		{
			QuizAnswerButtons[i].transform.GetChild(0).GetComponent<Text>().text = question.answers[i];
			QuizAnswerButtons[i].transform.GetComponent<Image>().color = defaultButtonColors[i];
		}

		StartCoroutine(Utils.LoadImage((Texture t) => 
		{ 
			QuizImage.texture = t; 
			QuizImage.GetComponent<ResizeRawImage>().AdjustSize(); 
		}, question.image));

		QuizThemeForm.SetActive(false);
		ScoreboardForm.SetActive(false);
		QuizQuestionForm.SetActive(true);
		riserPlayed = false;
		state = State.RoundCountdown;
		SoundController.instance.StartCountdown(question.countdown);
	}

	public void SetRoundCountdownText()
	{
		CountdownText.text = Mathf.Ceil(timerTime).ToString();
	}

	public void SetGameCountdownText()
	{
		GameCountdownText.text = $"Игра начнётся через {Mathf.Ceil(timerTime)}...";
	}

	public void OnRoundStarted()
	{
		timerTime = roundQuestion.time;
		SetAnswerButtonsInteractable(true);
		SoundController.instance.SetLowPassFilter(false, 0f, false);
		SoundController.instance.PlayMusic("ingame");
		QuizForm.SetActive(true);
		countdownSoundPlayed = false;
		state = State.InGame;
	}

	public void OnRightAnswer(int id)
	{
		state = State.None;
		SoundController.instance.SetLowPassFilter(true, 5f);
		SetAnswerButtonsInteractable(false, answer);
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
			LocalClient.Send("leave");
			SoundController.instance.SetLowPassFilter(false, 5, false);
			SoundController.instance.PlayMusic("lobby", true);
			SoundController.instance.ForceCountdownStop();
			gameObject.SetActive(false);
			LobbyForm.SetActive(true);
			state = State.None;
			LobbyFormScript.enterLobbyForm.SetActive(false);
			LobbyFormScript.createLobbyForm.SetActive(false);
			LobbyFormScript.menuForm.SetActive(true);
			LobbyFormScript.activeForm = LobbyFormScript.menuForm;
			DiscordController.instance.UpdateActivity($"В главном меню");
		});
	}

	public void OnGameEnded()
	{
		SoundController.instance.SetLowPassFilter(false, 0, false);
		SoundController.instance.PlayMusic("lobby");
		ScoreboardForm.SetActive(false);
		DiscordController.instance.UpdateActivity("В игре", "Экран конца игры");
		gameEndScript.SetUpForm();
		GameEndForm.SetActive(true);
	}

	public void SetTimerText()
	{
		QuizTimerText.text = Mathf.Ceil(timerTime).ToString();
	}

	public void OnAnswerButtonPressed(int answerIndex)
	{
		LocalClient.Send("answer", answerIndex);
		answer = answerIndex;
		SoundController.instance.ForceCountdownStop();
		SetAnswerButtonsInteractable(false, answerIndex);
	}

	public void SetAnswerButtonsInteractable(bool enable, int indexForSkip = -1)
	{
		for (int i = 0; i < QuizAnswerButtons.Length; ++i)
				QuizAnswerButtons[i].GetComponent<Button>().interactable = i == indexForSkip ? !enable : enable;
	}
}
