using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardForm : MonoBehaviour
{
	[Header("References")]
	public RoomForm Room;
	public GameObject ScoreboardPlayerPrefab;

	private Dictionary<int, ScoreboardPlayer> scoreboardPlayers = new Dictionary<int, ScoreboardPlayer>();

	public void UpdateScore(Dictionary<int, int> score)
	{
		if (scoreboardPlayers.Count == 0)
		{
			ClearScoreboard();
			InitScoreboard(score);
		}

		var list = scoreboardPlayers.OrderByDescending(x => x.Value.score).ToList();

		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.SetScore(score[list[i].Key], i);
		}
	}

	private void InitScoreboard(Dictionary<int, int> score)
	{
		int idx = 0;
		foreach (int id in score.Keys)
		{
			Vector2 pos = ScoreboardPlayer.startVec + idx * ScoreboardPlayer.offsetVec;

			GameObject scoreboardPlayer = Instantiate(ScoreboardPlayerPrefab, transform.GetChild(1));
			scoreboardPlayer.transform.localPosition = pos;
			scoreboardPlayer.transform.GetChild(0).GetComponent<Text>().text = Room.Clients[id].name;

			ScoreboardPlayer player = scoreboardPlayer.GetComponent<ScoreboardPlayer>();
			player.vecPos = pos;

			scoreboardPlayers.Add(id, player);
			idx++;
		}
	}

	private void ClearScoreboard()
	{
		foreach (Transform scoreboardPlayer in transform.GetChild(1))
			Destroy(scoreboardPlayer.gameObject);
	}
}
