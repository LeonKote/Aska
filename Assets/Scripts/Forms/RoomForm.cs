using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomSettings
{
	public int maxPlayers = 8;
	public int maxQuestionsCount = 20;
	public bool roomClosed = false;
}
public class RoomForm : MonoBehaviour
{
	public GameObject clientPrefab;
	public Transform clientParent;
	public Transform quizInfoPanelTransform;
	public Text NameText;
	public GameObject StartButton;
	public GameObject EditRoomSettingsButton;
	public Sprite blankAvatarSprite;
	public LobbyForm lobbyForm;
	public RoomSettings roomSettings;
	public Quiz quiz;
	private bool isLocalClientHosted;
	public int roomCode;
	public GameObject roomForm;
	public GameObject roomSettingsForm;
	public InputField maxPlayersInputField;
	public InputField maxQuestionsInputField;
	public Toggle roomClosedToggle;

	private Dictionary<int, Client> clients = new Dictionary<int, Client>();

	public Dictionary<int, Client> Clients { get { return clients; } }

	public void OnLocalClientJoin(int code, Client[] clients, Quiz quiz)
	{
		if (clients.Length == 1)
		{
			isLocalClientHosted = true;
			StartButton.SetActive(true);
			EditRoomSettingsButton.SetActive(true);
			roomSettings = new RoomSettings();
		}
		else
		{
			isLocalClientHosted = false;
			StartButton.SetActive(false);
			EditRoomSettingsButton.SetActive(false);
		}
		roomCode = code;
		NameText.text = "Комната #" + code;
		this.quiz = quiz;
		UpdateQuizInfoPanel();
		this.clients = clients.ToDictionary(x => x.id, x => x);
		foreach (Client client in clients)
			GameController.instance.StartCoroutine(Utils.LoadImage((Texture t) => 
			{
				client.icon = t;
				UpdateClientList();
			}, client.image, blankAvatarSprite.texture));
		DiscordController.instance.UpdateActivity($"В комнате #{code}", $"Игроков в комнате: {clients.Length}", quiz.name, roomCode.ToString());
	}

	public void OnClientJoin(Client client)
	{
		clients.Add(client.id, client);
		var temp = clients.ToList()[clients.Count - 1];
		GameController.instance.StartCoroutine(Utils.LoadImage((Texture t) => 
		{
			temp.Value.icon = t;
			UpdateClientList();
		}, client.image, blankAvatarSprite.texture));
		DiscordController.instance.UpdateActivity($"В комнате #{roomCode}", $"Игроков в комнате: {clients.Count}", quiz.name, roomCode.ToString());
	}

	public void UpdateQuizInfoPanel()
	{
		GameController.instance.StartCoroutine(Utils.LoadImage((Texture t) => 
			quizInfoPanelTransform.GetChild(0).GetComponent<RawImage>().texture = t, quiz.image, blankAvatarSprite.texture));
		quizInfoPanelTransform.GetChild(0).GetComponent<ResizeRawImage>().AdjustSize();
		quizInfoPanelTransform.GetChild(1).GetComponent<Text>().text = quiz.name;
		quizInfoPanelTransform.GetChild(2).GetComponent<Text>().text = quiz.description;
	}

	public void OnBackButtonPressed()
	{
		clients.Clear();
		LocalClient.Send("leave");
		Transition.Instance.StartAnimation(() =>
		{
			gameObject.SetActive(false);
			lobbyForm.gameObject.SetActive(true);
			lobbyForm.enterLobbyForm.SetActive(false);
			lobbyForm.createLobbyForm.SetActive(false);
			lobbyForm.menuForm.SetActive(true);
			lobbyForm.activeForm = lobbyForm.menuForm;
			DiscordController.instance.UpdateActivity($"В главном меню");
		});
	}

	public void OnClientLeave(Client client)
	{
		clients.Remove(client.id);
		UpdateClientList();
		DiscordController.instance.UpdateActivity($"В комнате #{roomCode}", $"Игроков в комнате: {clients.Count}", quiz.name, roomCode.ToString());
	}

	public void UpdateClientList()
	{
		ClearClientList();
		var list = clients.ToList();
		for (int i = 0; i < list.Count; ++i)
			InstantiateClient(list[i].Value);
	}

	public void InstantiateClient(Client client)
	{
		GameObject obj = Instantiate(clientPrefab, clientParent);
		obj.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = client.icon;
		obj.transform.GetChild(1).GetComponent<Text>().text = client.name;
	}

	public void OpenRoomSettingsForm()
	{
		if (!isLocalClientHosted)
			return;

		Transition.Instance.StartAnimation(() =>
		{
			roomForm.SetActive(false);
			UpdateRoomSettingsFields();
			roomSettingsForm.SetActive(true);
		});
	}

	public void CloseRoomSettingsForm()
	{
		Transition.Instance.StartAnimation(() =>
		{
			roomSettingsForm.SetActive(false);
			roomForm.SetActive(true);
		});
	}

	public void UpdateRoomSettingsFields()
	{
		maxQuestionsInputField.text = roomSettings.maxQuestionsCount.ToString();
		maxPlayersInputField.text = roomSettings.maxPlayers.ToString();
		roomClosedToggle.isOn = roomSettings.roomClosed;
	}

	public void ApplyRoomSettings()
	{
		if (Convert.ToInt32(maxQuestionsInputField.text) > quiz.questionsCount)
			maxQuestionsInputField.text = quiz.questionsCount.ToString();
		roomSettings.maxQuestionsCount = Convert.ToInt32(maxQuestionsInputField.text);
		roomSettings.maxPlayers = Convert.ToInt32(maxPlayersInputField.text);
		roomSettings.roomClosed = roomClosedToggle.isOn;

		// TODO: Отправить настройки комнаты на сервер
		Infobox.instance.ShowInfo("Настройки применены.", InfoType.green);
	}

	public void ClearClientList()
	{
		for (int i = 0; i < clientParent.childCount; ++i)
			Destroy(clientParent.GetChild(i).gameObject);
	}

	public void OnLocalClientStart()
	{
		LocalClient.Send("start");
	}
}
