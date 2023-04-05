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

	private LoginForm login;
	private LobbyForm lobby;
	private RoomForm room;
	private GameForm game;

	// Start is called before the first frame update
	void Start()
	{
		login = LoginForm.GetComponent<LoginForm>();
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
			case "socket":
				login.OnLocalClientConnected();
				break;
			case "auth":
				if ((bool)response["auth"]["result"] == true)
				{
					lobby.AddQuizzes(JsonConvert.DeserializeObject<Quiz[]>(response["auth"]["quizzes"].ToString()));
					Transition.Instance.StartAnimation(() =>
					{
						LoginForm.SetActive(false);
						LobbyForm.SetActive(true);
					});
				}
				break;
			case "roomJoin":
				room.OnLocalClientJoin((int)response["roomJoin"]["code"],
					JsonConvert.DeserializeObject<Client[]>(response["roomJoin"]["clients"].ToString()));
				Transition.Instance.StartAnimation(() =>
				{
					LobbyForm.SetActive(false);
					RoomForm.SetActive(true);
				});
				break;
			case "clientJoin":
				room.OnClientJoin(JsonConvert.DeserializeObject<Client>(response["clientJoin"].ToString()));
				break;
			case "clientLeave":
				room.OnClientLeave(JsonConvert.DeserializeObject<Client>(response["clientLeave"].ToString()));
				break;
			case "gameStarted":
				game.OnGameStart((string)response["gameStarted"]["name"]);
				Transition.Instance.StartAnimation(() =>
				{
					RoomForm.SetActive(false);
					GameForm.SetActive(true);
				});
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
