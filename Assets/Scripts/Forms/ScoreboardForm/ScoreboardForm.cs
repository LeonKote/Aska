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
	public Sprite blankAvatarSprite;
	private Dictionary<int, ScoreboardPlayer> scoreboardPlayers = new Dictionary<int, ScoreboardPlayer>();
	public Dictionary<int, ScoreboardPlayer> ScoreboardPlayers { get { return scoreboardPlayers; } }
	public void UpdateScore(Dictionary<int, int> score)
	{
		if (scoreboardPlayers.Count == 0)
		{
			ClearScoreboard();
			InitScoreboard(score);
		}

		foreach (int id in score.Keys)
		{
			scoreboardPlayers[id].Score = score[id];
		}

		var list = scoreboardPlayers.OrderByDescending(x => x.Value.score).ToList();

		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.SetPos(i);
		}

		foreach (var player in scoreboardPlayers)
		{
			if (player.Value.isChanging)
			{
				// SoundController.instance.PlayShortClip("scoreboard");
				break;
			}
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
			if (Room.Clients[id].icon != null)
				scoreboardPlayer.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = Room.Clients[id].icon;
			else
				scoreboardPlayer.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = blankAvatarSprite.texture;

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
