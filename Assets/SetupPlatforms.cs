using UnityEngine;
using System.Collections;

public class SetupPlatforms : MonoBehaviour {

	public GameObject prefab;
	public Transform container;
	public Transform lowerLeftCorner;
	public Transform upperRightCorner;

	public int nRows = 3;
	public int nColumns = 3;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clearPlatforms()
	{
		while(container.childCount > 0)
		{
			DestroyImmediate(container.GetChild(0).gameObject);
		}
	}

	public void setupPlatforms()
	{
		print (nRows);
		print (nColumns);

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


				GameObject nuGuy = GameObject.Instantiate(prefab);
				float ct = (c + offset)/ (nColumns - 1);
				float rt = (r + 0.0f)/ (nRows - 1);
				float x = Mathf.Lerp(lowerLeftCorner.position.x, upperRightCorner.position.x, ct);
				float y = Mathf.Lerp(lowerLeftCorner.position.y, upperRightCorner.position.y, rt);

				Vector3 nuPos = lowerLeftCorner.position;
				nuPos.x = x;
				nuPos.y = y;

				nuGuy.transform.position = nuPos;
				nuGuy.transform.parent = container;

			}
		}
	}
}
