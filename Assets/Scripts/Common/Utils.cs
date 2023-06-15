using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public static class Utils
{
	public static IEnumerator LoadImage(Action<Texture> texture, string url, Texture defaultTexture = null)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError
			|| request.result == UnityWebRequest.Result.ProtocolError)
		{
			texture.Invoke(defaultTexture);
			Debug.Log(request.error);
		}
		else
			texture.Invoke(((DownloadHandlerTexture)request.downloadHandler).texture);
	}
}