using System.Collections.Generic;

[System.Serializable]
public class Quiz
{
	public string id;
	public string name;
	public string image;
	public string description;
	public List<QuizQuestion> questions;

	public Quiz(string name, string description, string image, List<QuizQuestion> questions)
	{
		this.name = name;
		this.description = description;
		this.image = image;
		this.questions = questions;
	}
}
