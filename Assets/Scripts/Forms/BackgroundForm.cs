using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundForm : MonoBehaviour
{
	private RawImage image;
	private Vector2 offset = new Vector2(0.01f, 0.01f);

	// Start is called before the first frame update
	void Start()
	{
		image = GetComponent<RawImage>();
	}

	// Update is called once per frame
	void Update()
	{
		image.uvRect = new Rect(image.uvRect.position + offset * Time.deltaTime, image.uvRect.size);
	}
}
