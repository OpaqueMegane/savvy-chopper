using UnityEngine;
using System.Collections;

public class CuteJitter : MonoBehaviour {

	public 	float speed;

	public Vector3 startAng = Vector3.zero;

	// Use this for initialization
	void Start () {
		this.startAng = this.transform.eulerAngles;
		this.speed = Random.Range(1,5);
	}
	
	// Update is called once per frame
	void Update () {
		float twitchFactor = Mathf.Sin(Time.time * speed);
		twitchFactor = Mathf.RoundToInt(twitchFactor);
		this.transform.eulerAngles = startAng + new Vector3(0,0,7)*twitchFactor;
	}
}
