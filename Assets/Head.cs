using UnityEngine;
using System.Collections;

public class Head : MonoBehaviour {

    bool hatOk = false;
    public bool underground = true;
    GameObject colliders;
    Vector3 collidersStartOffset;
    float maxTexOffset;
    public GameObject hat = null;
    static GameObject explosionPrefab;

        AudioClip upClip;
        AudioClip downClip;
        AudioClip blastClip;
    
	AudioClip sparklySound;


	// Use this for initialization
	void Start () {
        maxTexOffset = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").y;
	this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, 1));
    underground = true;
    colliders = transform.FindChild("head").gameObject;
    collidersStartOffset = colliders.transform.localPosition;
    explosionPrefab = Resources.Load("explode") as GameObject;
    upClip = Resources.Load("slideUp") as AudioClip;
    downClip = Resources.Load("slideDown") as AudioClip;
    blastClip = Resources.Load("blast") as AudioClip;
		sparklySound = Resources.Load("sparkly") as AudioClip;
   
	}
	
	// Update is called once per frame
	void Update () {
        float offset = Mathf.Sin(Time.time);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(sprout());
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(desprout(null));
        }

        
        float texOff = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").y;
        this.colliders.transform.localPosition = collidersStartOffset;
        this.colliders.transform.position += new Vector3(0, (maxTexOffset - texOff) * this.transform.lossyScale.y, 0);
        
	}

    public void giveHat(GameObject hat, GameObject scorer)
    {
        //if (hat == null)
        
              this.hat = hat;
        StartCoroutine(desprout(scorer));
        
      
    }

    public void unburrow()
    {
        StartCoroutine(sprout());
    }

    IEnumerator sprout ()
    {
        AudioSource.PlayClipAtPoint(upClip, Vector3.zero);
        underground = false;
        float startTime = Time.time;
        while (Time.time < startTime + 1 )
        {
            float t = (Time.time - startTime) / 1;

            float off = (1 - t) + t * (maxTexOffset);
            this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, off));
            yield return new WaitForFixedUpdate();
        }

        hatOk = true;
        

    }

    IEnumerator desprout(GameObject scorer)
    {
        //AudioSource.PlayClipAtPoint(downClip, Vector3.zero);
        yield return new WaitForSeconds(2.5f);
        AudioSource.PlayClipAtPoint(downClip, Vector3.zero);
        hatOk = false;
        float startTime = Time.time;
        float off0 = GetComponent<Renderer>().material.GetTextureOffset("_MainTex").y;
        while (Time.time < startTime + 1)
        {
            float t = (Time.time - startTime) / 1;
            this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, (1-t) * off0 + t));
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);
        if (hat != null)
        {
            Destroy(hat);
            GameObject obj = GameObject.Instantiate(explosionPrefab, hat.transform.position, Quaternion.identity) as GameObject;
            AudioSource.PlayClipAtPoint(blastClip, Vector3.zero);
            if (scorer != null)
            {
                scorer.GetComponent<Copter>().scored();
            }
			yield return new WaitForSeconds(0.25f);
			AudioSource.PlayClipAtPoint(sparklySound, Vector3.zero);
            yield return new WaitForSeconds(1.75f);
            Destroy(obj);
        }
        underground = true;
        this.hat = null;




    }

}
