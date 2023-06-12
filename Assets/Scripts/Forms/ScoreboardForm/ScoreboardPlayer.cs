using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardPlayer : MonoBehaviour
{
	public static readonly Vector2 startVec = new Vector2(0, 350.2279f);
	public static readonly Vector2 offsetVec = new Vector2(0, -140);

	public float scoreDuration = 1f;
	public float moveDuration = 0.5f;

	[HideInInspector]
	public int score;
	[HideInInspector]
	public Vector2 vecPos;

	private int prevScore;
	private Vector2 prevVecPos;

	private Text scoreText;
	private float timeElapsed = 0;
	public bool isChanging;

	public int Score
	{
		get { return score; }
		set
		{
			prevScore = score;
			score = value;
		}
	}

	void Start()
	{
		scoreText = transform.GetChild(2).GetComponent<Text>();
	}

	void Update()
	{
		if (!isChanging) return;

		if (timeElapsed < scoreDuration)
		{
			transform.localPosition = Vector2.Lerp(prevVecPos, vecPos, timeElapsed / moveDuration);
			scoreText.text = ((int)Mathf.Lerp(prevScore, score, timeElapsed / scoreDuration)).ToString();
			timeElapsed += Time.deltaTime;
		}
		else
		{
			scoreText.text = score.ToString();
			timeElapsed = 0;
			isChanging = false;
		}
	}

	public void SetPos(int pos)
	{
		prevVecPos = vecPos;
		vecPos = startVec + pos * offsetVec;
		isChanging = true;
	}
}
