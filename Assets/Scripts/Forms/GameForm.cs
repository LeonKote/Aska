using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameForm : MonoBehaviour
{
    public Question Question;
    public Room Room;
    private bool roundStarted;
    private bool canAnswer;

    public GameObject QuizForm;
    public GameObject[] QuizAnswerButtons; // подразумевается 4 варианта ответа, в будущем можно сделать более гибко
    public Image QuizImage;
    public Text QuizTitle;
    public Text QuizTimerText;
    private double timeForAnswer;

    public GameObject LeaderBoardForm;
    private void FixedUpdate()
    {
        if (!roundStarted)
            return;
        if (timeForAnswer > 0)
        {
            timeForAnswer -= Time.deltaTime;
            SetTimerText();
            return;
        }
        canAnswer = false;
    }

    public void OnServerQuestionSent(string data)
    {
        // когда сервер присылает вопрос, тут из данных присланных сервером нужно заполнить Question

        PrepareForNextRound();
    }

    public void SetTimerText()
    {
        QuizTimerText.text = Math.Round(timeForAnswer).ToString();
    }

    public void PrepareForNextRound()
    {
        QuizTitle.text = Question.title;
        QuizImage.sprite = Question.image;
        timeForAnswer = Question.time;
        SetTimerText();
        for (int i = 0; i < QuizAnswerButtons.Length; ++i)
            QuizAnswerButtons[0].transform.GetChild(0).GetComponent<Text>().text = Question.answers[i];
    }

    public void PrepareLeaderboard()
    {
        // TODO: спавнить объекты в лидерборд
    }

    public void OnRoundStarted()
    {
        roundStarted = true;
        canAnswer = true;
        LeaderBoardForm.SetActive(false);
        QuizForm.SetActive(true);
    }

    public void OnRoundEnded()
    {
        roundStarted = false;
        QuizForm.SetActive(false);
        LeaderBoardForm.SetActive(true);
    }

    public void OnAnswerButtonPressed(int answerIndex)
    {
        if (!canAnswer)
            return;

        // отправить серверу выбранный вариант ответа

        canAnswer = false;
    }
}
