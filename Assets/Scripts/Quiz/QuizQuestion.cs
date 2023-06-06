using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class QuizQuestion
{
	public string question;
	public string[] answers;
	public string image;
	[JsonIgnore] public Texture icon;
	public int time;
	public int countdown;
	public int rightAnswerIndex;

	public QuizQuestion(string question, string[] answers, string image, int time, int countdown, int rightAnswerIndex)
	{
		this.question = question;
		this.answers = answers;
		this.image = image;
		this.time = time;
		this.countdown = countdown;
		this.rightAnswerIndex = rightAnswerIndex;
	}
}