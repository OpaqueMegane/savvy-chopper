using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HatHeadManager : MonoBehaviour {
    int targetNHats = 6;
    int targetNHeads = 4;

	int attractState = 0; //0 attract, 1 instructions 2 game

    List<GameObject> hats = new List<GameObject>();
	GameObject motionInstructions;

	int lastHatIdx;

    Transform hatSpawnPoints;
    List<GameObject> allHeads = new List<GameObject>();
    GameObject hatPrefab;

    AudioClip startGameClip;

    bool prepping = true;
    public static HatHeadManager instance;
	float spawnOkTime = float.NegativeInfinity;
	// Use this for initialization
	void Start () {

		motionInstructions = GameObject.Find("motion_example");

		lastHatIdx = Random.Range(0,hats.Count);

		HappyFunTimesExample.ExampleSimplePlayer.setUpAllCopters();

        prepping = true;
        if (instance == null)
        {
            instance = this;
        }
        hatSpawnPoints = GameObject.Find("_HAT_POINTS").transform;
        hatPrefab = Resources.Load("hat") as GameObject;
 

        startGameClip = Resources.Load<AudioClip>("ddr");
        //AudioSource.PlayClipAtPoint(startGameClip, Vector3.zero);

        StartCoroutine(Preamble());
	}

    delegate void GuiFunc();

    GuiFunc gfunc = () => { };

    public bool gameInProgress()
    {
        return !prepping;
    }
    IEnumerator Preamble()
    {
		attractState = 0;
		GameObject.Find("logo").GetComponent<SpriteRenderer>().enabled = true;
		GameObject.Find("instructions").GetComponent<SpriteRenderer>().enabled = false;
        
		bool enoughPlayers = false;// true;
        while (!enoughPlayers)
        {
            enoughPlayers = Input.GetKey(KeyCode.Return);
            yield return new WaitForEndOfFrame();
        }

		//GameObject.Find("logo").GetComponent<SpriteRenderer>().enabled = false;
		GameObject.Find("instructions").GetComponent<SpriteRenderer>().enabled = true;
        //yield return new WaitForSeconds(2);

		yield return new WaitForSeconds(.5f);

		attractState = 1;

		while(attractState == 1)
		{
			if (Input.GetKey(KeyCode.Return))
			{
				attractState = 2;
			}
			yield return new WaitForEndOfFrame();
		}

		//GameObject.Find("logo").GetComponent<SpriteRenderer>().enabled = false;
		GameObject.Find("instructions").GetComponent<SpriteRenderer>().enabled = false;
		motionInstructions.SetActive(false);


        AudioSource.PlayClipAtPoint(Copter.announcerClips[0], Vector3.zero);
        gfunc = () => {
            string msg = "R_U-Readyy???!";
            AlexUtil.DrawCenteredText(new Vector3(0, 0, 0), msg, 100, Color.black, "buxton");
            AlexUtil.DrawCenteredText(new Vector3(3, 4, 0), msg, 100, Color.white, "buxton");
        };
        yield return new WaitForSeconds(3);


        AudioSource.PlayClipAtPoint(Copter.announcerClips[2], Vector3.zero);

        gfunc = () =>
        {
            string msg = "GO!";
            AlexUtil.DrawCenteredText(new Vector3(0, 0, 0), msg, 100, Color.black, "buxton");
            AlexUtil.DrawCenteredText(new Vector3(3, 4, 0), msg, 100, Color.white, "buxton");
        };
        yield return new WaitForSeconds(1);
        
        
        
        
        prepping = false;

        gfunc = () => { };


    }

	void lazyHeadsSetup()
	{
		if (allHeads.Count == 0 && GameObject.Find("_PLATFORMS").transform.childCount > 0)
		{
			foreach (Transform t in GameObject.Find("_PLATFORMS").transform)
			{
				allHeads.Add(t.FindChild("head").gameObject);
			}
		}
	}

	// Update is called once per frame
	void Update () {

		lazyHeadsSetup();

        if (Input.GetKeyDown(KeyCode.R))
        {
			HappyFunTimesExample.ExampleSimplePlayer.AllCopters = null;
            Application.LoadLevel(0);
        }

        if (prepping)
        {
            return;
        }


		spawnNewHatUpdate();
        int nHeadsUp = 0;
        List<GameObject> underGroundHeads = new List<GameObject>();
        foreach (GameObject head in allHeads)
        {
            if (! head.GetComponent<Head>().underground)
            {
                nHeadsUp++;
            }
            else
            {
                underGroundHeads.Add(head);
            }
        }

        if (nHeadsUp < targetNHeads)
        {
            underGroundHeads[Random.Range(0, underGroundHeads.Count - 1)].GetComponent<Head>().unburrow();
        }
	}


	void spawnNewHatUpdate()
	{
		hats.RemoveAll((x) => x == null);
		
		if (hats.Count < targetNHats && Time.time > spawnOkTime)
		{
			
			Transform randomPlace = hatSpawnPoints.GetChild(Random.Range(0, hatSpawnPoints.childCount));

			bool placeOK = true;
			foreach(GameObject otherHats in hats)
			{
				if (Mathf.Abs(otherHats.transform.position.x - randomPlace.position.x) < .1f)
				{
					placeOK = false;
					break;
				}
			}

			if (placeOK)
			{
				GameObject nuHat = GameObject.Instantiate(hatPrefab, randomPlace.position, Quaternion.identity) as GameObject;
				spawnOkTime = Time.time + 4;
				hats.Add(nuHat);
			}
		}
	}


    void OnGUI()
    {
        this.gfunc();

		if (!gameInProgress())
		{
			if (attractState == 0)
			{
			AlexUtil.DrawText(new Vector2(Screen.width - 400 -2 , 15 + 2), "Select a partner via your phone\n" +
			                  "and twirl your arm as pictured\nbelow to pilot your copter", 32, Color.white, "buxton");
            
			AlexUtil.DrawText(new Vector2(Screen.width - 400, 15), "Select a partner via your phone\n" +
			                  "and twirl your arm as pictured\nbelow to pilot your copter", 32, Color.black, "buxton");
			}

			if ((.5f * Time.time) - Mathf.Floor(.5f * Time.time) > .45f)
			{
				AlexUtil.DrawText(new Vector2(19, Screen.height - 40), "PRESS 'RETURN' TO START GAME", 24, Color.white, "buxton");
				AlexUtil.DrawText(new Vector2(17, Screen.height - 42), "PRESS 'RETURN' TO START GAME", 24, Color.black, "buxton");
			}
		}
    }
}
