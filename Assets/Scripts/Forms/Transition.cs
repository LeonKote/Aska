using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
	public static Transition Instance { get; private set; }

	public Color32[] colorPalette;
	// TODO: исправить проблему с недоступностью объектов при анимации (анимация не стартует заново, а должна)
	public Animator animator;
	private Action action;
	private Vector3 startPosition;

	public void Init()
	{
		gameObject.SetActive(false);
		Instance = this;
		startPosition = transform.localPosition;
	}

	public void StartAnimation(Action action)
	{
		animator.Rebind();
		this.action = action;
		gameObject.GetComponent<Image>().color = colorPalette[UnityEngine.Random.Range(0, colorPalette.Length)];
		gameObject.SetActive(true);
		animator.Play("Transition");
	}

	public void OnMiddleTrigger()
	{
		action.Invoke();
	}

	public void OnEndTrigger()
	{
		gameObject.SetActive(false);
		transform.localPosition = startPosition;
	}
}
