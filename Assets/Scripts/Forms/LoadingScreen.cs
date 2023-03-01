using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	private enum Stage
	{
		Normal,
		End,
		Dispose
	}

	public AnimationCurve AnimCurve;
    public float Duration = 1.5f;

	private Image background;
	private Image circle;
	private Transform edge;
	private Transform edgeConst;

	private Stage stage;

	private bool forward = true;
	private float timeElapsed = 0;

	// Start is called before the first frame update
	void Start()
    {
		Transform circleTransform = transform.GetChild(0);

		background = transform.GetComponent<Image>();
		circle = circleTransform.GetChild(0).GetComponent<Image>();
        edge = circleTransform.GetChild(1);
		edgeConst = circleTransform.GetChild(2);

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

				edge.localEulerAngles = new Vector3(0, 0, t * -360);
			}
			else
			{
				forward = !forward;
				circle.fillClockwise = forward;
				timeElapsed = 0;

				if (LocalClient.Socket != null && LocalClient.Socket.Connected && forward)
				{
					stage = Stage.End;
					edgeConst.gameObject.SetActive(false);
					edge.localPosition = new Vector3(0, -224, 0);
					edge.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
				}
			}
		}
		else if (stage == Stage.End)
		{
			if (timeElapsed < Duration)
			{
				float t = 1 - AnimCurve.Evaluate(timeElapsed / Duration);

				timeElapsed += Time.deltaTime;

				edge.localScale = new Vector3(t, t, 1);
			}
			else
			{
				timeElapsed = 0;
				stage = Stage.Dispose;
			}
		}
		else if (stage == Stage.Dispose)
		{
			if (timeElapsed < Duration)
			{
				float t = 1 - AnimCurve.Evaluate(timeElapsed / Duration);

				timeElapsed += Time.deltaTime;

				background.color = new Color(1, 1, 1, t);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}
