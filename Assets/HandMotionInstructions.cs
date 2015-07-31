using UnityEngine;
using System.Collections;

public class HandMotionInstructions : MonoBehaviour {

	float direction = 1;
	GameObject swirler;

	// Use this for initialization
	void Start () {
		swirler = this.transform.FindChild("rotater").gameObject;
		StartCoroutine(animRoutine());
		//StartCoroutine(hideAndShow());
	}
	
	// Update is called once per frame
	void Update () {
		swirler.transform.eulerAngles += new Vector3(0,0, -direction * Time.deltaTime * 80);

	}

	IEnumerator hideAndShow()
	{
		Vector3 startPos = this.transform.position;

		yield return new WaitForSeconds(8);

		iTween.MoveTo(this.gameObject, startPos + new Vector3(0,-150,0), 7);
		yield return new WaitForSeconds(8);
		iTween.MoveTo(this.gameObject, startPos, 7);
		yield return new WaitForSeconds(15);
    }

	IEnumerator animRoutine()
	{


		GameObject[] frames = new GameObject[4];

		frames[0] = this.transform.FindChild("cw1").gameObject;
		frames[1] = this.transform.FindChild("cw2").gameObject;
		frames[2] = this.transform.FindChild("cw3").gameObject;
		frames[3] = this.transform.FindChild("cw4").gameObject;

		int currentFrame = 0;

		for(;;)
		{
			float swapTime = Time.time + 5;

			while (Time.time <= swapTime)
			{
				yield return new WaitForSeconds(.125f);

				currentFrame = (currentFrame + ((int) direction) + frames.Length) % frames.Length;
				for (int i = 0; i < frames.Length; i++)
				{
					frames[i].SetActive(false);
				}

				frames[currentFrame].SetActive(true);

			


			}
			float oldDir = direction;
			float newDir = direction * -1;

			float t = 0;
			while (t < 1)
			{
				t += Time.fixedDeltaTime * .5f;
				direction = Mathf.Lerp(oldDir, newDir, t);

				swirler.transform.localScale = new Vector3(direction, swirler.transform.localScale.y, swirler.transform.localScale.z);

				yield return new WaitForFixedUpdate();
			}
			direction = (int) newDir;

			yield return new WaitForSeconds(0.5f);


		}
	}
}
