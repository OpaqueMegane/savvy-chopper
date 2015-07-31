using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Copter : MonoBehaviour {
    static  string[]
        fruitNames = 
        {
"apple",
"banana",
"cherry",
"durian",
"hotdog",
"lemon",
"orange",
"pineapple",
"plum",
"strawberry",
"tomato",
"watermelon"
        };

    static string[] teams = { 
                "ap'ple",
"banana",
"cherry",
"durian",
"hotdog",
"le'mon",
"orange",
"pinapl",
"p_lu'm",
"strwby",
"tomato",
"wtrmln" };

    //public GameObject verticalPlayer = null;
    //public GameObject horizontalPlayer = null;
    GameObject prop;
    GameObject hProp;
    float upSpeed = 4f;//7; //acceleration
    float topSpeed = 19;//35;
    float propSpeed = 0;
    float hPropSpeed = 0;

	AudioSource bumpSource;
	GameObject fruitGfx;
	Vector3 fruitGfxScale;

    static bool fruitsShuffled = false;

    public int score = 0;
    static int someoneWon = -1;
    static bool alreadySpeaking = false;

    //public static int N_PLAYERS = 0;
    public Grabbable hat = null;
    float initialYRot;
        AudioClip releaseSound;
        public AudioClip winGameClip;
        public AudioClip bumpClip;
        public static AudioClip[] announcerClips;
    
    public static string getFruitNameByIndex(int index)
        {
            return fruitNames[index];
        }

    public static void shuffleFruits()
    {
        int[] nuIndices = new int[fruitNames.Length];
        for (int i = 0; i < nuIndices.Length; i++)
        {
            nuIndices[i] = i;
        }

        // Knuth shuffle algorithm
        for (int t = 0; t < nuIndices.Length; t++)
        {
            int tmp = nuIndices[t];
            int r = Random.Range(t, nuIndices.Length);
            nuIndices[t] = nuIndices[r];
            nuIndices[r] = tmp;
        }

        string[] newNames = new string[fruitNames.Length];
        string[] newTeamNames = new string[teams.Length];
        for (int i = 0; i < nuIndices.Length; i++)
        {
            newNames[i] = fruitNames[nuIndices[i]];
            newTeamNames[i] = teams[nuIndices[i]];
        }

        fruitNames = newNames;
        teams = newTeamNames;
        fruitsShuffled = true;
    
    }

	// Use this for initialization
	void Start () {



        if (!fruitsShuffled)
        {
            shuffleFruits();
        }


		

        someoneWon = -1;
        prop = transform.FindChild("prop").gameObject;
        hProp = transform.FindChild("h_prop").gameObject;

        // set up the gfx

		string fruitKind = "";

		fruitKind = fruitNames[copterNumber];
		
	
        this.transform.FindChild(fruitKind).gameObject.SetActive(true);

		fruitGfx =  this.transform.FindChild(fruitKind).gameObject;
		fruitGfxScale = fruitGfx.transform.localScale;
            
        
        if (hProp.transform.FindChild(fruitKind) != null)
        {
            foreach (Transform child in hProp.transform)
            {
          			
		
                    child.gameObject.SetActive(child.name == fruitKind);

			
                
            }
        }

        if (prop.transform.FindChild(fruitKind) != null)
        {
            foreach (Transform child in prop.transform)
            {

                child.gameObject.SetActive(child.name == fruitKind);

            }
        }
      

        //
        
        
        
        //N_PLAYERS++;
        initialYRot = this.transform.eulerAngles.y;
        releaseSound = Resources.Load("steal") as AudioClip;
        winGameClip = Resources.Load("hawk") as AudioClip;
        bumpClip = Resources.Load("bump") as AudioClip;

		bumpSource = this.gameObject.AddComponent<AudioSource>();
		bumpSource.loop = false;
		bumpSource.playOnAwake = false;
		bumpSource.clip = bumpClip;

        if (announcerClips == null)
        {
            announcerClips = Resources.LoadAll<AudioClip>("announcer");
        }


		//this.gameObject.SetActive(false); //[!!!]
	}
	
	// Update is called once per frame
	void Update () {

	

        if (this.name == "Copter1" && Input.GetKeyDown(KeyCode.U))
        {
            this.scored();
        }

		prop.transform.localRotation *= Quaternion.AngleAxis(propSpeed * Time.deltaTime, Vector3.up);//localEulerAngles += new Vector3(0, propSpeed, 0) * Time.deltaTime;

		//propSpeed = Mathf.Lerp(propSpeed, 0, 0.6f*Time.deltaTime);

        hProp.transform.localRotation *= Quaternion.AngleAxis(hPropSpeed * Time.deltaTime, Vector3.up);//localEulerAngles += new Vector3(hPropSpeed, 0, 0) * Time.deltaTime;
        hPropSpeed = Mathf.Lerp(hPropSpeed, 0, 0.6f * Time.deltaTime);

        KeyCode[][] playerKeys
            =
            {
                new KeyCode[] {KeyCode.W, KeyCode.D, KeyCode.A},
				new KeyCode[] {KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.LeftArrow},
                new KeyCode[] {KeyCode.Y, KeyCode.J, KeyCode.G},
                new KeyCode[] {KeyCode.O, KeyCode.Semicolon, KeyCode.K},
            };

		if (Input.GetKeyDown(KeyCode.M))
		{
			this.scored();
		}

		if(this.copterNumber < playerKeys.Length )
		{
			if (Input.GetKeyDown(playerKeys[this.copterNumber][0]))
        {
            thrustUp();
        }

			if (Input.GetKeyDown(playerKeys[this.copterNumber ][1]))
        {
            thrustHorizontal(1);
        }

			if (Input.GetKeyDown(playerKeys[this.copterNumber][2]))
        {
            thrustHorizontal(-1);
        }
		}

        if (GetComponent<Rigidbody>().velocity.x > 1)
        {
            float yRot = transform.eulerAngles.y;
            yRot = Mathf.Lerp(yRot, initialYRot, Time.deltaTime * 2);
            this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
            
        }
        else  if (GetComponent<Rigidbody>().velocity.x < -1)
        {
            float yRot = transform.eulerAngles.y;
            yRot = Mathf.Lerp(yRot, initialYRot + 180, Time.deltaTime * 2);
            this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
        }

        
        if (score == 6 && someoneWon == -1)
        {
            someoneWon = copterNumber;
            StartCoroutine(StartGameOver());
            AudioSource.PlayClipAtPoint(winGameClip, Vector3.zero);
        }

	}

	public int copterNumber
	{
		get
		{
		string[] split = this.name.Split('r');

		int coptNumber = int.Parse(split[1]) - 1;//this.name[this.name.Length - 1] - '0' - 1;
		return coptNumber;
		}
	}

    public void scored()
    {
        score++;
        StartCoroutine(announcerSay());
    }

    IEnumerator announcerSay()
    {
        while(alreadySpeaking)
        {
            yield return new WaitForEndOfFrame();
        }
        alreadySpeaking = true;
        yield return new WaitForSeconds(1);
        AudioSource.PlayClipAtPoint(announcerClips[Random.Range(0, announcerClips.Length)], Vector3.zero);
        yield return new WaitForSeconds(3);
        alreadySpeaking = false;
    }

    public void thrustUp()
    {
        Vector3 targetVelo = this.GetComponent<Rigidbody>().velocity;
        targetVelo.y += upSpeed;
        targetVelo.y = Mathf.Min(topSpeed, targetVelo.y);
        this.GetComponent<Rigidbody>().velocity = targetVelo;
        propSpeed += 750;
        propSpeed = Mathf.Min(propSpeed, 2500);
    }


    public void thrustDown ()
    {
		print ("dddd");
        Vector3 targetVelo = this.GetComponent<Rigidbody>().velocity;
        targetVelo.y -= upSpeed;
        targetVelo.y = Mathf.Max(-1.5f*topSpeed, targetVelo.y);
        this.GetComponent<Rigidbody>().velocity = targetVelo;
        propSpeed -= 750;
//
        propSpeed = Mathf.Max(propSpeed, -2500);
    }


    public void thrustHorizontal(float coeff)
    {
        

        Vector3 targetVelo = this.GetComponent<Rigidbody>().velocity;
        targetVelo.x += coeff*.5f*upSpeed;
        targetVelo.x = Mathf.Min(topSpeed, targetVelo.x);
        targetVelo.x = Mathf.Max(-topSpeed, targetVelo.x);
        this.GetComponent<Rigidbody>().velocity = targetVelo;

        hPropSpeed += coeff * 500;
        hPropSpeed = Mathf.Min(hPropSpeed, 1500);
        hPropSpeed = Mathf.Max(hPropSpeed, -1500);
    }

	void resetFruitScale()
	{
		fruitGfx.transform.localScale = fruitGfxScale;
	}

    void OnCollisionEnter(Collision col)
    {
        if (this.GetComponent<Rigidbody>().velocity.magnitude > 3.5f)
        {
            //AudioSource.PlayClipAtPoint(bumpClip, Vector3.zero, .75f);
			bumpSource.pitch = Random.Range(.75f, 1.25f);
			bumpSource.Play();
			Vector3 diff  =  col.contacts[0].point - this.transform.position;//(col.contacts[0]. - this.transform.position);

			float squashAmt = fruitGfx.transform.localScale.x / 1.5f;


			if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
			{
				iTween.PunchScale(this.fruitGfx, new Vector3(-squashAmt, squashAmt,0), 1f);
			}
			else
			{
				iTween.PunchScale(this.fruitGfx, new Vector3(squashAmt, -squashAmt, 0), 1f);
			}
			Invoke("resetFruitScale", 1.1f);
        }

        if (col.gameObject.GetComponent<Copter>() != null && col.gameObject.transform.position.y > this.transform.position.y)
        {
         

            print("drop hat!");
            if (hat != null)
            {
				AudioSource.PlayClipAtPoint(releaseSound, Vector3.zero);
                hat.grabber = col.gameObject;
                this.hat = null;
            }
        }
    }

    IEnumerator StartGameOver()
    {
        yield return new WaitForSeconds(5);
        HappyFunTimesExample.ExampleSimplePlayer.AllCopters = null;
        Application.LoadLevel(0);
    }

    void OnGUI()
    {
		string word = "";
		word = teams[copterNumber];//"HORSE";
		try
		{
        //word = teams[copterNumber];//"HORSE";
		}
		catch (Exception e)
		{
			print ("couldn't get name " + copterNumber);
		}

        string scoreString = "letters:";
        for (int i = 0; i < word.Length; i++)
        {
            if (i < score)
            {
                scoreString += word[i];
            }
            else
            {
                scoreString += "-";
            }
        }
        //word.Length 

        if (someoneWon != -1 && someoneWon == copterNumber)
        {
            AlexUtil.DrawCenteredText(new Vector3(0, 0, 0), teams[copterNumber] + " , U WIN! !!", 100, Color.black, "buxton");
            AlexUtil.DrawCenteredText(new Vector3(3, 4, 0), teams[copterNumber] + " , U WIN! !!", 100, Color.white, "buxton");
        }
		

		int xOffset = copterNumber % 6;
		int yOffset = copterNumber < 6 ? Screen.height - 35 : 0;

		float horizScoreSpacing  = 235;
		int scoreFontSize = 22;

		AlexUtil.DrawText(new Vector2(10 + (xOffset) * horizScoreSpacing, 5 + yOffset), fruitNames[copterNumber] + " " + scoreString, scoreFontSize, Color.black, "buxton");
		//AlexUtil.DrawText(new Vector2(120 + 75 + (xOffset) * horizScoreSpacing, 25 + yOffset), scoreString, scoreFontSize, Color.black, "buxton");

		AlexUtil.DrawText(new Vector2(10 + (xOffset) * horizScoreSpacing, 5 + yOffset) + new Vector2(2,2), fruitNames[copterNumber] + " " + scoreString, scoreFontSize, Color.white, "buxton");
		//AlexUtil.DrawText(new Vector2(120 + 75 + (xOffset) * horizScoreSpacing, 25 + yOffset) + new Vector2(2, 2), scoreString, scoreFontSize, Color.white, "buxton");
    }
}
