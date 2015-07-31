/*
An assortment of random useful functions/classes/stuff

Also includes some Debug developer settings

TODO convert some of these to extension methods!

*/
using UnityEngine;

using System.Collections;

//TODO try and put camera stuff in here

public static class Extensions
{        
	//public static string toSti(this ResourceType grade)
	public static void SetLayerRecursively (this GameObject obj, int newLayer)
	{

		if (null == obj)
		{
			return;
		}
			
		obj.layer = newLayer;
			
		foreach (Transform child in obj.transform)
		{
			if (null == child)
			{
				continue;
			}
			SetLayerRecursively (child.gameObject, newLayer);
		}
	}

    public static void DgRPC(this GameObject obj, string functionName, RPCMode target, params object[] args)
    {

        if (obj.GetComponent<NetworkView>() != null)
        {
            obj.GetComponent<NetworkView>().RPC(functionName, target, args);
          
        }
        else
        {
            obj.SendMessage(functionName, args);
        }     
    }
}

public class AlexUtil : MonoBehaviour
{	
	public static float ConvertRange (float originalMin, float originalMax, float targetMin, float targetMax, float number)
	{
		float ret = (number - originalMin) / (originalMax - originalMin) * (targetMax - targetMin) + targetMin;

		ret = Mathf.Clamp (ret, targetMin, targetMax);
		return ret;
	}

    public static void DrawCenteredText(Vector2 offset, string text, int fontSize, Color fontColor, string font)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontSize = fontSize;
        style.font = (Font)Resources.Load("Fonts/" + font);
        Vector2 size = style.CalcSize(new GUIContent(text));
        Vector2 position = new Vector2(Screen.width, Screen.height) / 2 - size / 2;
        position += offset;
        GUI.Label(new Rect(position.x, position.y, size.x, size.y), text, style);
    }

    public static void DrawText(Vector2 position, string text, int fontSize, Color fontColor, string font)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = fontColor;
        style.fontSize = fontSize;
        style.font = (Font)Resources.Load("Fonts/" + font);
        Vector2 size = style.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(position.x, position.y, size.x, size.y), text, style);
    }


	                              

	public class StoredTransform
	{
		public Vector3 position;
		public Quaternion rotation;


        public Vector3 forward
        {
            get { return rotation * Vector3.forward; }
            set { rotation = Quaternion.LookRotation(value); }
        }

   
		
		public StoredTransform (Transform t)
		{
			this.position = t.position;
			this.rotation = t.rotation;
		}

		public static implicit operator UnityEngine.Transform(StoredTransform t)  // implicit digit to byte conversion operator
		{
			System.Console.WriteLine("conversion occurred");
			return new StoredTransform(t);
		}
	}
}
