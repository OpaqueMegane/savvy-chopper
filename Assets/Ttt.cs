using UnityEngine;
using System.Collections;

public class Ttt : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			//Randomly change the pitch
			this.GetComponent<AudioSource>().pitch = Random.Range(.5f, 2f);
			
			//Make it half as loud
			this.GetComponent<AudioSource>().volume = .5f;
			
			this.GetComponent<AudioSource>().Play();
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
        {
            //pause the sound
            this.GetComponent<AudioSource>().Pause();
        }
        
    }
	
	// Update is called once per frame

}
