using UnityEngine;
using System.Collections;

public class PositionGameStuff : MonoBehaviour {

	Vector3 startPosition;
	public GameObject platformPrefab;
	Transform platformContainer;
	Transform hatPtContainer;

	// Use this for initialization
	void Start () {
		platformContainer = GameObject.Find("_PLATFORMS").transform;
		hatPtContainer = GameObject.Find("_HAT_POINTS").transform;

		platformPrefab = (GameObject) Resources.Load("platform");
		//platformPrefab

		startPosition = this.transform.position;

		GameObject leftWall = this.transform.Find("left").gameObject;
		GameObject topWall = this.transform.Find("top").gameObject;
		GameObject rightWall = this.transform.Find("right").gameObject;
		GameObject bottomWall = this.transform.Find("bottom").gameObject;



		Vector3 left = Camera.main.ViewportToWorldPoint( new Vector3(0,.5f, 0)) + new Vector3(0, 0,0);
		left.z = leftWall.transform.position.z;

		Vector3 right = Camera.main.ViewportToWorldPoint( new Vector3(1,.5f, 0))+ new Vector3(10, 0,0);
		right.z = leftWall.transform.position.z;

		Vector3 top = Camera.main.ViewportToWorldPoint( new Vector3(.5f,1, 0)) + new Vector3(0, 25,0);
		top.z = leftWall.transform.position.z;

		Vector3 bottom = Camera.main.ViewportToWorldPoint( new Vector3(.5f,0, 0))+ new Vector3(0, -2,0);
		bottom.z = leftWall.transform.position.z;

		leftWall.transform.position = left;
		rightWall.transform.position = right;
		topWall.transform.position = top;
		bottomWall.transform.position = bottom;

		Vector3 upperLeft = Vector3.zero;
		upperLeft.x = left.x;
		upperLeft.y = top.y;
		upperLeft.z = left.z + 50;

		Vector3 lowerRight = Vector3.zero;
		lowerRight.x = right.x;
		lowerRight.y = bottom.y;
		lowerRight.z = upperLeft.z;

		float topInset = -65;
		float bottomInset = 30;
		float horizontalInset = 18;
		setupPlatforms(upperLeft + new Vector3(horizontalInset, topInset, 0), lowerRight  + new Vector3(-horizontalInset, bottomInset, 0));

	}


	void setupPlatforms(Vector3 upperRightCorner, Vector3 lowerLeftCorner)
	{
		int nRows = 3;
		int nColumns = 4;

		for (int r = 0; r < nRows; r++)
		{		
			float offset = 0;
			if (r%2 == 1)
			{
				offset = .5f;
			}
			
			for (int c = 0; c < nColumns; c++)
			{
				//if (offset!= 0 && c == nColumns-1)
				//{
				//		continue;
				//}
				




				float ct = (c + offset)/ (nColumns - 1);
				float rt = (r + 0.0f)/ (nRows - 1);

				if (ct <= 1)
				{
					//create platform
					GameObject nuGuy = GameObject.Instantiate(platformPrefab);
					float x = Mathf.Lerp(lowerLeftCorner.x, upperRightCorner.x, ct);
					float y = Mathf.Lerp(lowerLeftCorner.y, upperRightCorner.y, rt);
				
					Vector3 nuPos = lowerLeftCorner;
					nuPos.x = x;
					nuPos.y = y;
				
					nuGuy.transform.position = nuPos;
					nuGuy.transform.parent = platformContainer;
				}
				
			}
		}


		//spawn hat points
		for (float c = .25f; c < (nColumns - 1); c += .5f)
		{
			float ct = (c)/ (nColumns - 1);
			GameObject newHatPt = new GameObject();

			float x = Mathf.Lerp(lowerLeftCorner.x, upperRightCorner.x, ct);
			float y = upperRightCorner.y;
			
			Vector3 nuPos = upperRightCorner;
			nuPos.x = x;
			nuPos.y = y - 10;
			
			newHatPt.transform.position = nuPos;
			newHatPt.transform.parent = hatPtContainer;

		}
		positionCopters(upperRightCorner, lowerLeftCorner);
	}

	void positionCopters(Vector3 upperRightCorner, Vector3 lowerLeftCorner)
	{
		Transform copterContainer;
		copterContainer = GameObject.Find("_ALL_COPTERS").transform;
		for(int i = 0; i < copterContainer.childCount; i++)
		{
			float x = Mathf.Lerp(lowerLeftCorner.x, upperRightCorner.x, i / (copterContainer.childCount - 1.0f));

			copterContainer.GetChild(i).transform.position = new Vector3(
				x,
				lowerLeftCorner.y + 10,
				lowerLeftCorner.z);
		}

	
		//int nCopters 
	}

	// Update is called once per frame
	void Update () {
	
	}
	
}
