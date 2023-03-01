using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class LocalClient : ClientSocket
{
	private GameObject loginForm;
	private GameObject lobbyForm;
	private GameObject roomForm;
	private Room room;

	// Start is called before the first frame update
	void Start()
	{
		loginForm = transform.GetChild(0).gameObject;
		lobbyForm = transform.GetChild(2).gameObject;
		roomForm = transform.GetChild(3).gameObject;

		loginForm.GetComponent<LoginForm>().Init();
		lobbyForm.GetComponent<LobbyForm>().Init();
		roomForm.GetComponent<Room>().Init();

		room = roomForm.GetComponent<Room>();

		StartClient();
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
	}

	protected override void OnResponse(string message)
	{
		JObject response = JObject.Parse(message);

		switch (response.Properties().First().Name)
		{
			case "authResult":
				if ((string)response["authResult"] == "OK")
				{
					loginForm.SetActive(false);
					lobbyForm.SetActive(true);
				}
				break;
			case "roomJoin":
				lobbyForm.SetActive(false);
				roomForm.SetActive(true);
				room.SetRoomCode((int)response["roomJoin"]["code"]);
				room.SetClients(JsonConvert.DeserializeObject<Client[]>(response["roomJoin"]["clients"].ToString()));
				break;
			case "clientJoin":
				room.AddClient(JsonConvert.DeserializeObject<Client>(response["clientJoin"].ToString()));
				break;
			case "clientLeave":
				room.RemoveClient(JsonConvert.DeserializeObject<Client>(response["clientLeave"].ToString()));
				break;
			case "clientMessage":
				room.ClientMessage((int)response["clientMessage"]["id"], (string)response["clientMessage"]["text"]);
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
