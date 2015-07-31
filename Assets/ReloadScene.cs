using UnityEngine;
using System.Collections;

public class ReloadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Delete))
        {
            Application.LoadLevel(0);
        }
	}
}
