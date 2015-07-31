using UnityEngine;
using System.Collections;

public class MovieTexturePlayer : MonoBehaviour
{
    public MovieTexture movTexture;
    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = movTexture;
        movTexture.loop = true;
        movTexture.Play();
		movTexture.wrapMode = TextureWrapMode.Repeat;
        
    }
}