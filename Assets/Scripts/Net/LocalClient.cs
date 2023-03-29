using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class LocalClient : ClientSocket
{
	[Header("References")]
	public GameObject LoginForm;
	public GameObject LobbyForm;
	public GameObject RoomForm;
	public GameObject GameForm;

	private LobbyForm lobby;
	private RoomForm room;
	private GameForm game;

	// Start is called before the first frame update
	void Start()
	{
		lobby = LobbyForm.GetComponent<LobbyForm>();
		room = RoomForm.GetComponent<RoomForm>();
		game = GameForm.GetComponent<GameForm>();

		StartClient();
	}

	// Update is called once per frame
	void Update()
	{
		ProcessResponses();
	}

	protected override void OnResponse(string message)
	{
		JObject response = JObject.Parse(message);

		switch (response.Properties().First().Name)
		{
			case "auth":
				if ((bool)response["auth"]["result"] == true)
				{
					lobby.AddQuizzes(JsonConvert.DeserializeObject<Quiz[]>(response["auth"]["quizzes"].ToString()));
					Transition.Instance.StartAnimation(Auth);
				}
				break;
			case "roomJoin":
				Transition.Instance.StartAnimation(RoomJoin);
				room.OnLocalClientJoin((int)response["roomJoin"]["code"],
					JsonConvert.DeserializeObject<Client[]>(response["roomJoin"]["clients"].ToString()));
				break;
			case "clientJoin":
				room.OnClientJoin(JsonConvert.DeserializeObject<Client>(response["clientJoin"].ToString()));
				break;
			case "clientLeave":
				room.OnClientLeave(JsonConvert.DeserializeObject<Client>(response["clientLeave"].ToString()));
				break;
			case "gameStarted":
				Transition.Instance.StartAnimation(GameStarted);
				game.OnGameStart();
				break;
			case "startTimer":
				game.OnTimerStart();
				break;
			case "roundStarted":
				game.CountdownForRoundStart(JsonConvert.DeserializeObject<QuizQuestion>(response["roundStarted"].ToString()));
				break;
			case "rightAnswer":
				game.OnRightAnswer((int)response["rightAnswer"]);
				break;
			case "roundEnded":
				game.OnRoundEnded(JsonConvert.DeserializeObject<Dictionary<int, int>>(response["roundEnded"].ToString()));
				break;
			case "gameEnded":
				game.OnGameEnded();
				break;
		}
	}
	public void Auth()
	{
		LoginForm.SetActive(false);
		LobbyForm.SetActive(true);
	}
	public void RoomJoin()
	{
		LobbyForm.SetActive(false);
		RoomForm.SetActive(true);
	}
	public void GameStarted()
	{
		RoomForm.SetActive(false);
		GameForm.SetActive(true);
	}
	public static void Send(string key, string value)
	{
		JObject request = new JObject();
		request[key] = value;
		SendRequest(request.ToString(Formatting.None));
	}

	public static void Send(string key, int value)
	{
		JObject request = new JObject();
		request[key] = value;
		SendRequest(request.ToString(Formatting.None));
	}
}
