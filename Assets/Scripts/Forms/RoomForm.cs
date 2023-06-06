using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomForm : MonoBehaviour
{
	public Text NameText;
	public Text ClientListText;
	public GameObject StartButton;

	private Dictionary<int, Client> clients = new Dictionary<int, Client>();

	public Dictionary<int, Client> Clients { get { return clients; } }

	public void OnLocalClientJoin(int code, Client[] clients)
	{
		if (clients.Length == 1)
			StartButton.SetActive(true);
		NameText.text = "Комната #" + code;
		foreach (Client client in clients)
			GameController.instance.StartCoroutine(Utils.LoadImage((Texture t) => client.icon = t, client.image));
		this.clients = clients.ToDictionary(x => x.id, x => x);
		UpdateClientList();
	}

	public void OnClientJoin(Client client)
	{
		clients.Add(client.id, client);
		UpdateClientList();
	}

	public void OnClientLeave(Client client)
	{
		clients.Remove(client.id);
		UpdateClientList();
	}

	public void UpdateClientList()
	{
		ClientListText.text = string.Join(", ", clients.Select(x => x.Value.name));
	}

	public void OnLocalClientStart()
	{
		LocalClient.Send("start", null);
	}
}
