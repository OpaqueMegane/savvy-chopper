using UnityEngine;
using System.Collections;

public class BackAndForth : MonoBehaviour {

	Vector3 startPosition;

	// Use this for initialization
	void Start () {
		startPosition = this.transform.position;
		StartCoroutine(moveRoutine());
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator moveRoutine()
	{
			Vector3 left = Camera.main.ViewportToWorldPoint( new Vector3(0,.5f, startPosition.z)) + new Vector3(-250, 0,0);;
			Vector3 right = Camera.main.ViewportToWorldPoint( new Vector3(1,.5f, startPosition.z))+ new Vector3(250, 0,0);;
			left.y = startPosition.y;
			left.z = startPosition.z;
			right.y = startPosition.y;
			right.z = startPosition.z;
			for (;;)
			{
				float dur = 40f;
				float tim = 0;
				while (tim < 1)
				{
					tim += Time.fixedDeltaTime / dur;
					
					this.transform.position = Vector3.Lerp(left, right, tim);
                        
                    yield return new WaitForFixedUpdate();
                }

			if (HatHeadManager.instance.gameInProgress ())
			{
				yield return new WaitForSeconds (Random.Range(20, 120));
			}
			else
			{
				yield return new WaitForSeconds (.5f);
			}

				tim = 0;
				while (tim < 1)
				{
					tim += Time.fixedDeltaTime / dur;
					
				this.transform.position = Vector3.Lerp(right, left, tim);
					
					yield return new WaitForFixedUpdate();
                }

			if (HatHeadManager.instance.gameInProgress ())
			{
				yield return new WaitForSeconds (Random.Range(20, 120));
			}
			else
			{
				yield return new WaitForSeconds (.5f);
			}

			}
		


	}
}
