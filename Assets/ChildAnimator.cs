using UnityEngine;
using System.Collections;

public class ChildAnimator : MonoBehaviour {

	int lastKeyPress = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//turn off all the children sprites
		for (int i = 0; i < this.transform.childCount; i++)
		{
			this.transform.GetChild(i).gameObject.SetActive(false);
		}

		//turn on only the one you care about
		if (Input.GetKey(KeyCode.RightArrow))
		{
			lastKeyPress = 0;
			this.transform.FindChild("name of right child sprite").gameObject.SetActive(true);	
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			lastKeyPress = 1;
			this.transform.FindChild("name of left child sprite").gameObject.SetActive(true);	
		}
		else
		{

			if (lastKeyPress == 0)
			{
				this.transform.FindChild("name of right child sprite").gameObject.SetActive(true);	
			}
			else if (lastKeyPress == 1)
			{
				this.transform.FindChild("name of left child sprite").gameObject.SetActive(true);	
			}
		}

	}
}
