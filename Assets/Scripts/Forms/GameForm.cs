using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameForm : MonoBehaviour
{
	[Header("References")]
	public GameObject QuizForm;
	public GameObject ScoreboardForm;
	public GameObject ScoreboardPlayerPrefab;
	public RoomForm Room;

	[Header("Quiz form")]
	public Text QuizTitle;
	public GameObject[] QuizAnswerButtons; // подразумевается 4 варианта ответа, в будущем можно сделать более гибко
	public RawImage QuizImage;
	public Text QuizTimerText;

	private bool roundStarted;
	private float timeForAnswer;
	private int answer;

	private Dictionary<int, int> scoreboard = new Dictionary<int, int>();

	// Update is called once per frame
	void Update()
	{
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

	public void OnRoundStarted(QuizQuestion question)
	{
		roundStarted = true;
		SetAnswerButtonsInteractable(true);
		ScoreboardForm.SetActive(false);
		QuizForm.SetActive(true);

		QuizTitle.text = question.question;

		for (int i = 0; i < QuizAnswerButtons.Length; i++)
			QuizAnswerButtons[i].transform.GetChild(0).GetComponent<Text>().text = question.answers[i];

		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(Convert.FromBase64String(question.image));
		QuizImage.texture = tex;

		timeForAnswer = question.time;
	}

	public void OnRightAnswer(int id)
	{
		roundStarted = false;
		SetAnswerButtonsInteractable(false);

		if (answer == -1)
			return;

		if (answer == id)
		{

		}
		else
		{

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
