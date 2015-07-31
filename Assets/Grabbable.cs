using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour {

    public GameObject grabber = null;
    GameObject head = null;
    float grabRadius;
    LineRenderer line;
    int nLineVertices = 2;

    AudioClip grabSound;
    AudioClip releaseSound;
	static int lastHatIdx = -1;


	// Use this for initialization
	void Start () {

		Transform images = this.transform.FindChild("images");
		if (lastHatIdx == -1)
		{

			lastHatIdx = Random.Range(0, images.childCount);
		}

        grabRadius = 15f;// this.GetComponent<SphereCollider>().radius * 2;
        line = this.transform.FindChild("line").GetComponent<LineRenderer>();
        line.SetVertexCount(nLineVertices);
        line.enabled = false;

		//prevent subsequent duplicates
		int newHatIdx = Random.Range(0, images.childCount);
		if (newHatIdx == lastHatIdx)
		{
			newHatIdx = (newHatIdx + 1) % images.childCount;
		}
		lastHatIdx = newHatIdx;

		images.GetChild(newHatIdx).gameObject.SetActive(true);


        grabSound = Resources.Load("can") as AudioClip;
        releaseSound = Resources.Load("click") as AudioClip;

	}
	
	// Update is called once per frame
	void Update () 
    {
        this.GetComponent<Rigidbody>().AddForce(Vector3.up * -25, ForceMode.Acceleration);
        if (grabber != null)
        {
            Vector3 diff = grabber.transform.position - this.transform.position;
            diff = Mathf.Min(0,grabRadius - diff.magnitude) * diff.normalized;
            this.GetComponent<Rigidbody>().AddForce(-diff * 5, ForceMode.Acceleration);
            line.enabled = true;
          
            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, grabber.transform.position);

			grabber.GetComponent<Copter>().hat = this;

        }
        else
        {
            line.enabled = false;
        }
	
	}

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("head") 
		    && 
		    this.transform.position.y > c.gameObject.transform.position.y + 4f // sufficiently above head
		    &&
		    Mathf.Abs(this.transform.position.x - c.gameObject.transform.position.x) < 3 //sufficiently lined up

		    && head == null)
        {

            AudioSource.PlayClipAtPoint(releaseSound, Vector3.zero);
            StartCoroutine(delayedCrowdSound());
            
            head = c.transform.parent.gameObject;
            Head h = head.transform.GetComponent<Head>();
            h.giveHat(this.gameObject, grabber);
            grabber = null;
           
        }
    }
    IEnumerator delayedCrowdSound()
    {
        yield return new WaitForSeconds(1.0f);
        
            AudioSource.PlayClipAtPoint(Copter.announcerClips[1], Vector3.zero);
        

    }

    void OnTriggerEnter(Collider c)
    {

        if (c.GetComponent<Copter>() != null && grabber == null && (head == null || head.GetComponent<Head>().underground))
        {
            
            grabber = c.gameObject;
            grabber.GetComponent<Copter>().hat = this;

            AudioSource.PlayClipAtPoint(grabSound, Vector3.zero);
        }
        
    }
}
