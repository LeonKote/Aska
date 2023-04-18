using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAnimEnded : MonoBehaviour
{
	public void OnAnimationEnded()
	{
		gameObject.SetActive(false);
	}
}
