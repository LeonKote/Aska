using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
	private static StreamReader reader;
	private static StreamWriter writer;

	private List<string> responses = new List<string>();

	public static bool IsConnected { get { return socket != null && socket.Connected; } }

	protected void StartClient()
	{
		Thread thread = new Thread(new ThreadStart(ListenForData));
		thread.IsBackground = true;
		thread.Start();
	}

	protected void ProcessResponses()
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

		responses.Add("{\"socket\":true}");
		Debug.Log("Connected");

		reader = new StreamReader(socket.GetStream());
		writer = new StreamWriter(socket.GetStream());
		writer.AutoFlush = true;

		try
		{
			while (!reader.EndOfStream)
			{
				string message = reader.ReadLine();
				Debug.Log("Server: " + message);
				responses.Add(message);
			}
		}
		catch (IOException e)
		{
			Debug.Log("Socket exception: " + e);
		}
	}

	protected virtual void OnResponse(string message)
	{

	}

	protected static void SendRequest(string message)
	{
		try
		{
			writer.WriteLine(message);
			Debug.Log("Client: " + message);
		}
		catch (SocketException e)
		{
			Debug.Log("Socket exception: " + e);
		}
	}
}
