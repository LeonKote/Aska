using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingForm : MonoBehaviour
{
	private enum Stage
	{
		Normal,
		Dispose
	}

	public AnimationCurve AnimCurve;
	public float Duration = 2f;
	public Transform LoadingCircle;
	public InputField InputField;

	private Image circle;
	private Transform edge;
	private Transform edgeConst;

	private Stage stage;

	private bool forward = true;
	private float timeElapsed = 0;

	// Start is called before the first frame update
	void Start()
	{
		circle = LoadingCircle.GetChild(0).GetComponent<Image>();
		edge = LoadingCircle.GetChild(1);
		edgeConst = LoadingCircle.GetChild(2);

		stage = Stage.Normal;
	}

	// Update is called once per frame
	void Update()
	{
		if (stage == Stage.Normal)
		{
			if (timeElapsed < Duration)
			{
				float t = AnimCurve.Evaluate(timeElapsed / Duration);

				timeElapsed += Time.deltaTime;

				if (forward)
					circle.fillAmount = t;
				else
					circle.fillAmount = 1 - t;

				LoadingCircle.localEulerAngles = new Vector3(0, 0, t * -360);
				edge.localEulerAngles = new Vector3(0, 0, t * -360);
			}
			else
			{
				forward = !forward;
				circle.fillClockwise = forward;
				timeElapsed = 0;

				if (LocalClient.IsConnected && forward)
				{
					stage = Stage.Dispose;
					edgeConst.gameObject.SetActive(false);
					edge.localPosition = new Vector3(0, -24, 0);
					edge.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
				}
			}
		}
		else if (stage == Stage.Dispose)
		{
			if (timeElapsed < Duration)
			{
				float t = 1 - AnimCurve.Evaluate(timeElapsed / Duration);

				timeElapsed += Time.deltaTime;

				edge.localScale = new Vector3(t, t, 1);
			}
			else
			{
				Destroy(LoadingCircle.gameObject);
				enabled = false;
			}
		}
	}
}
