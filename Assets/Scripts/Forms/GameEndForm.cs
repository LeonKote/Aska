using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEndForm : MonoBehaviour
{
    public RoomForm room;
    public ScoreboardForm scoreboard;
    public LocalClient localClient;

    public GameObject[] leaderboardObjects;
    public Text clientScoreboardPosition;
    public Sprite blankAvatarSprite;

    public void ClearForm()
	{
        foreach (GameObject obj in leaderboardObjects)
            obj.SetActive(false);

        clientScoreboardPosition.text = null;
	}

    public void SetUpForm()
	{
        ClearForm();
        var list = scoreboard.ScoreboardPlayers.OrderByDescending(x => x.Value.score).ToList();
        for (int i = 0; i < leaderboardObjects.Length; ++i)
		{
            if (i > list.Count - 1)
                break;

            leaderboardObjects[i].SetActive(true);
            Transform clientFields = leaderboardObjects[i].transform.GetChild(1);
            if (room.Clients[list[i].Key].icon != null)
                clientFields.GetChild(0).GetComponent<RawImage>().texture = room.Clients[list[i].Key].icon;
            else
                clientFields.GetChild(0).GetComponent<RawImage>().texture = blankAvatarSprite.texture;
            clientFields.GetChild(1).GetComponent<Text>().text = room.Clients[list[i].Key].name;
		}
        clientScoreboardPosition.text = $"Вы заняли {list.IndexOf(list.Where(x => x.Key == localClient.Id).FirstOrDefault()) + 1} место.";
    }
}
