using UnityEngine;
using System.Collections;

//Display of progress bars adapted from Unity Answers.
//http://answers.unity3d.com/questions/11892/how-would-you-make-an-energy-bar-loading-progress.html

public class DisplayScoreAlt : MonoBehaviour {

	public Texture2D barContent;
	public Texture2D barEdges;
	public float amt = .7f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//display = (VariableControl.Wealth / 100);
	}
	
	void OnGUI () {
		GUI.DrawTextureWithTexCoords(
			new Rect(15,15, amt*barEdges.width, barEdges.height), 
			barContent, new Rect(0,0,amt, 1) //only draws 70% of the image from left to right
			);

		GUI.DrawTexture(new Rect(15,15, barEdges.width, barEdges.height), barEdges);

	}
}
