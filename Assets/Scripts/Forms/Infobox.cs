using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum InfoType
{
	green, 
	red
}
public class Info
{
	public Sprite sprite;
	public Color32 color;
	public string info;
}
public class Infobox : MonoBehaviour
{
    public static Infobox instance;

	public GameObject prefab;
	public Transform parent;
	public Sprite[] sprites;
	public Color32[] colors;
	public float duration;

	public void Start()
	{
		instance = this;
	}

	public void ShowInfo(string message, InfoType type)
	{
		Sprite sprite = sprites[(int)type];
		Color32 color = colors[(int)type];

		StartCoroutine(InfoCoroutine(sprite, color, message));
		SoundController.instance.PlayShortClip(type == InfoType.green ? "success" : "fail");
	}
	private IEnumerator InfoCoroutine(Sprite sprite, Color32 color, string message)
	{
		GameObject obj = Instantiate(prefab, parent);
		obj.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
		obj.transform.GetChild(0).GetComponent<Image>().color = color;
		obj.transform.GetChild(1).GetComponent<Text>().text = message;
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(duration);
		obj.GetComponent<Animator>().Play("InfoboxDisappearing");
		yield return new WaitForSeconds(0.5f);
		Destroy(obj);
	}
}
