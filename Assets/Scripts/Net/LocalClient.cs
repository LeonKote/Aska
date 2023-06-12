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
	public GameObject CreateLobbyForm;
	public GameObject ProfileEditorForm;

	private LoginForm login;
	private LobbyForm lobby;
	private RoomForm room;
	private GameForm game;
	private CreateLobbyForm createLobby;
	private ProfileEditorForm profileEditorForm;

	private int id;
	public bool isAuthorizedBySocial;
	public int Id { get { return id; }}

	// Start is called before the first frame update
	void Start()
	{
		login = LoginForm.GetComponent<LoginForm>();
		lobby = LobbyForm.GetComponent<LobbyForm>();
		room = RoomForm.GetComponent<RoomForm>();
		game = GameForm.GetComponent<GameForm>();
		createLobby = CreateLobbyForm.GetComponent<CreateLobbyForm>();
		profileEditorForm = ProfileEditorForm.GetComponent<ProfileEditorForm>();

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
				if (PlayerPrefs.HasKey("token"))
				{
					isAuthorizedBySocial = true;
					Send("auth", new JObject()
					{
						{ "type", "token" },
						{ "token", PlayerPrefs.GetString("token") }
					});
				}
				break;
			case "auth":
				switch ((string)response["auth"]["type"])
				{
					case "url":
						System.Diagnostics.Process.Start((string)response["auth"]["url"]);
						break;
					case "success":
						string name = (string)response["auth"]["profile"]["name"];
						lobby.nicknameText.text = name;
						profileEditorForm.nicknameInputField.text = name;

						if (response["auth"]["token"] != null)
						{
							PlayerPrefs.SetString("token", (string)response["auth"]["token"]);
							isAuthorizedBySocial = true;
						}

						if (response["auth"]["profile"]["image"] != null)
							StartCoroutine(Utils.LoadImage((Texture t) =>
							{
								lobby.avatarImage.texture = t;
								profileEditorForm.avatarImage.texture = t;
							}, (string)response["auth"]["profile"]["image"]));
						else
							lobby.avatarImage.texture = lobby.blankAvatarSprite.texture;
						id = (int)response["auth"]["profile"]["id"];
						Transition.Instance.StartAnimation(() =>
						{
							LoginForm.SetActive(false);
							LobbyForm.SetActive(true);
						});
						break;
				}
				break;
			case "searchQuiz":
				createLobby.InstantiateQuizButtons(response["searchQuiz"].ToObject<Quiz[]>());
				break;
			case "roomJoin":
				room.OnLocalClientJoin((int)response["roomJoin"]["code"], response["roomJoin"]["clients"].ToObject<Client[]>(),
					response["roomJoin"]["quiz"].ToObject<Quiz>());
				Transition.Instance.StartAnimation(() =>
				{
					LobbyForm.SetActive(false);
					RoomForm.SetActive(true);
				});
				break;
			case "clientJoin":
				room.OnClientJoin(response["clientJoin"].ToObject<Client>());
				break;
			case "clientLeave":
				room.OnClientLeave(room.Clients[(int)response["clientLeave"]]);
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
				game.CountdownForRoundStart(response["roundStarted"].ToObject<QuizQuestion>());
				break;
			case "rightAnswer":
				game.OnRightAnswer((int)response["rightAnswer"]);
				break;
			case "roundEnded":
				game.OnRoundEnded(response["roundEnded"].ToObject<Dictionary<int, int>>());
				break;
			case "gameEnded":
				game.OnGameEnded();
				break;
			case "join":
				switch ((string)response["join"]["error"])
				{
					case "doesntExistsErr":
						Infobox.instance.ShowInfo("Комната не существует.", InfoType.red);
						break;
					case "startedErr":
						Infobox.instance.ShowInfo("Игра в этой комнате уже началась.", InfoType.red);
						break;
				}
				break;
		}
	}

	public static void Send(string key, JToken? value = null)
	{
		SendRequest(JsonConvert.SerializeObject(new JObject { { key, value } }, Formatting.None));
	}
}
