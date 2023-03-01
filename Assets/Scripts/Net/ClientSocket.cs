using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientSocket : MonoBehaviour
{
	public string Hostname = "localhost";
	public int Port = 8887;
	public int MaxAttempts = 3;

	private static TcpClient socket;
	private Thread thread;

	private List<string> responses = new List<string>();

	public static TcpClient Socket { get { return socket; } }

	protected void StartClient()
	{
		thread = new Thread(new ThreadStart(ListenForData));
		thread.IsBackground = true;
		thread.Start();
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (responses.Count == 0) return;
		for (int i = 0; i < responses.Count; i++)
			OnResponse(responses[i]);
		responses.Clear();
	}

	private void ListenForData()
	{
		int attempts = 0;

		do
		{
			try
			{
				socket = new TcpClient(Hostname, Port);
				attempts = 0;
			}
			catch (SocketException e)
			{
				attempts++;
				Debug.Log("Connection attempt: " + attempts);

				if (attempts == MaxAttempts)
				{
					Debug.Log(e);
					return;
				}
			}
		}
		while (attempts > 0);

		Debug.Log("Connected");

		byte[] bytes = new byte[1024];
		while (true)
		{
			using (NetworkStream stream = socket.GetStream())
			{
				int length;
				while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
				{
					byte[] data = new byte[length];
					Array.Copy(bytes, 0, data, 0, length);
					string message = Encoding.UTF8.GetString(data);
					Debug.Log("Server: " + message);
					responses.Add(message);
				}
			}
		}
	}

	protected virtual void OnResponse(string message)
	{

	}

	protected static void SendRequest(string message)
	{
		try
		{
			NetworkStream stream = socket.GetStream();
			byte[] data = Encoding.UTF8.GetBytes(message + "\n");
			stream.Write(data, 0, data.Length);
			Debug.Log("Client: " + message);
		}
		catch (SocketException e)
		{
			Debug.Log("Socket exception: " + e);
		}
	}
}
