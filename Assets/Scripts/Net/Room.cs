using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
	public InputField inputField;
	public Text chatText;
	public Text nameText;
	public Text clientListText;

	private Dictionary<int, Client> clients = new Dictionary<int, Client>();


	public void SetRoomCode(int code)
	{
		nameText.text = "Room #" + code;
	}

	public void AddClient(Client client)
	{
		clients.Add(client.id, client);
		UpdateClientList();
	}

	public void RemoveClient(Client client)
	{
		clients.Remove(client.id);
		UpdateClientList();
	}

	public void SetClients(Client[] clients)
	{
		this.clients = clients.ToDictionary(x => x.id, x => x);
		UpdateClientList();
	}

	public void ClientMessage(int id, string message)
	{
		if (id == 0)
			chatText.text += "\n" + message;
		else
			chatText.text += "\n" + clients[id].name + ": " + message;
	}

	public void UpdateClientList()
	{
		clientListText.text = string.Join(", ", clients.Select(x => x.Value.name));
	}

	public void OnLocalClientMessage()
	{
		LocalClient.Send("message", inputField.text);
		inputField.text = "";
	}
}
