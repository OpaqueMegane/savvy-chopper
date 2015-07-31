using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimesExample;

public class PlayerIDManager : MonoBehaviour 
{

	public static List<PlayerInfo> playerInfos;
	public static int SessionID;
    static Dictionary<int, int> playerPairRequests;
    public const int MAX_N_PLAYERS = 24;
	public GameObject[] AllCopters = null;
	public static PlayerIDManager instance = null;

	// Use this for initialization
	void Awake () 
	{
		ExampleSimplePlayer.setUpAllCopters();
		if (instance == null) 
		{
			instance = this;
		}
	}

	public static bool checkID (string id)
	{
		string[] idStrings = id.Split ('|');
		int session = int.Parse (idStrings [0]);
		int pId = int.Parse (idStrings [1]);
		return (session == SessionID && pId < playerInfos.Count);
	
	}

	public static int NewPlayer(ExampleSimplePlayer playerInterface)
	{
		PlayerInfo newPlayer = new PlayerInfo ();
		newPlayer.score = 0;
        newPlayer.gameObject = playerInterface.gameObject;
		//newPlayer.color = playerInterface.m_color; [!!!]
		playerInfos.Add (newPlayer);
		return playerInfos.Count - 1;

	}

	// Update is called once per frame
	void Update () 
	{



        if (playerInfos == null)
        {
            
            playerInfos = new List<PlayerInfo>();
            if (PlayerPrefs.HasKey("sessionID"))
            {
                PlayerPrefs.SetInt("sessionID", PlayerPrefs.GetInt("sessionID") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("sessionID", 0);
            }

            SessionID = PlayerPrefs.GetInt("sessionID");
            playerPairRequests = new Dictionary<int, int>();
        }

	    if (HatHeadManager.instance.gameInProgress())
        {
            //assignAllUnassignedPlayers();
        }
        //Attempt to pair players,
        // pairings freeze for duration of round

        //player states:
        // made request, hasn't made requested

        //while waiting for game to start....
        //1. players request each other
        //2. player requested other, other specified no preference
        //3. both players requested no preference

        //once game starts, pair any unpaired players
     
        for (int i = 0; i < playerInfos.Count; i++) //playerInfos, i probably equals id
        {
            int requestingPlayer = int.Parse(playerInfos[i].id.Split('|')[1]); //get their id TODO: replace id with a plain int?
            
            if (i != requestingPlayer)
            {
                Debug.LogError("id and array idx don't match! ");
                Debug.Break();
            }



            bool playerAlreadyAssigned =  playerInfos[requestingPlayer].gameObject != null && playerInfos[requestingPlayer].gameObject.GetComponent<ExampleSimplePlayer>().isAssignedCopterSlot();

            if (!playerAlreadyAssigned
                &&
                playerPairRequests.ContainsKey(requestingPlayer) 
                && 
			    (playerPairRequests.ContainsKey(playerPairRequests[requestingPlayer]) || playerPairRequests[requestingPlayer] == -1)
                )
            {
                
				int requestedPlayerNumber = playerPairRequests[requestingPlayer];
				int requestsRequest = -9999;//not set!

				if (requestedPlayerNumber == -1)
				{
					print ("searching for other uncaring");
					for (int j = 0; j < playerInfos.Count; j++)
					{
						if (i != j && playerPairRequests.ContainsKey(j) && playerPairRequests[j] == -1)
						{
							//otherNotCaringPlayer = j;
							requestedPlayerNumber = j;
                            requestsRequest = i;
                            print ("don't kare");
                            break;
                        }
                    }
                }
				else
				{
					requestsRequest = playerPairRequests[requestedPlayerNumber];
				}
				
				bool requestedSelf = requestingPlayer == playerPairRequests[requestingPlayer];

				if (requestedSelf)
				{
					print ("you can't choose yourself, player " + requestingPlayer + "!");
					playerPairRequests.Remove(requestingPlayer);
				}


				//handle case if both don't care
	
				//


				if (
					(!requestedSelf && (requestingPlayer == requestsRequest || requestsRequest == -1))//if they requested you, or they don't care
					) 
                {
					print ("req playr " + requestingPlayer + " ");
                    ExampleSimplePlayer requester = playerInfos[requestingPlayer].gameObject.GetComponent<ExampleSimplePlayer>();
					ExampleSimplePlayer requestee = playerInfos[requestedPlayerNumber].gameObject.GetComponent<ExampleSimplePlayer>();

					while (playerPairRequests.ContainsKey(requestingPlayer))
					{
						playerPairRequests.Remove(requestingPlayer);
					}

					while (playerPairRequests.ContainsKey(requestedPlayerNumber))
					{
						playerPairRequests.Remove(requestedPlayerNumber);
					}

                    int copterSlot = getFreeCopter();
                    requester.assignCopterSlot(2 * copterSlot);
                    requestee.assignCopterSlot(2 * copterSlot + 1);
                    //match!
                    //Give their copters
                    
                }
                
            }
        }
		hideUnoccupiedCopters ();
	}



	void hideUnoccupiedCopters()
	{
		if (ExampleSimplePlayer.AllCopters == null || this.gameObject == null)
		{
			return;
		}

		for (int i = 0; ExampleSimplePlayer.AllCopters != null && i < ExampleSimplePlayer.AllCopters.Length; i++) 
		{
			if (PlayerIDManager.instance.copterUnoccupied (i)) 
			{
				//ExampleSimplePlayer.AllCopters[i].SetActive(false); //[!!!]
			}
			else
			{
				ExampleSimplePlayer.AllCopters[i].SetActive(true);
			}
		}

		//AudioSource.PlayClipAtPoint(Resources.Load("name_of_sound_without_extension") as AudioClip, new Vector3(0,0,0));
	}

	public bool copterUnoccupied(int i)
	{
		bool horizOccupied = false;
		bool vertOccupied = false;
		for (int j = 0; playerInfos != null && j < playerInfos.Count; j++)//see if any players have that slot
		{
			int copterIdx = playerInfos[j].copterSlot;
			if (copterIdx != -1 && copterIdx/2 == i)
			{
				horizOccupied = playerInfos[j].copterSlot % 2 == 1;
				vertOccupied = playerInfos[j].copterSlot % 2 == 0;
			}
			
		}

		return !horizOccupied && !vertOccupied;
	}


	
	int getFreeCopter()
	{
		
		for (int i = 0; i < MAX_N_PLAYERS/2; i++) //for each copter
        {
			if (copterUnoccupied(i))
			{
				return i;
            }
        }
        return -1;
    }
    

    bool copterSlotFree(int slot)
    {
        for (int i = 0; i < playerInfos.Count; i++) //playerInfos, i probably equals id
        {
            if (playerInfos[i].copterSlot == slot)
            {
                return false;
            }
        }
        return true;
    }

    int getFreeCopterSlot()
    {
        for (int j = 0; j < MAX_N_PLAYERS; j++)
        {
			if (copterSlotFree(j))
			{
                return j;
            }
        }
        return -1;
    }

    void assignAllUnassignedPlayers()
    {


        for (int i = 0; i < playerInfos.Count; i++) //playerInfos, i probably equals id
        {
            bool playerAlreadyAssigned = playerInfos[i].gameObject.GetComponent<ExampleSimplePlayer>().isAssignedCopterSlot();

                if (!playerAlreadyAssigned)
                {

				playerInfos[i].gameObject.GetComponent<ExampleSimplePlayer>().assignCopterSlot(getFreeCopterSlot());
			}
			
        }


        /*for (int i = 0; i < playerInfos.Count; i++) //playerInfos, i probably equals id
        {
            bool playerAlreadyAssigned = playerInfos[i].gameObject.GetComponent<ExampleSimplePlayer>().isAssignedCopterSlot();

           if (!playerAlreadyAssigned)
           {
               bool foundPartner = false;

               for (int j = i + 1; j < playerInfos.Count && !foundPartner; j++) //playerInfos, i probably equals id
               {
                   bool subsequentPlayerAlreadyAssigned = playerInfos[j].gameObject.GetComponent<ExampleSimplePlayer>().isAssignedCopterSlot();

                   if (!subsequentPlayerAlreadyAssigned)
                   {
                       foundPartner = true;
                       ExampleSimplePlayer iPlayer = playerInfos[i].gameObject.GetComponent<ExampleSimplePlayer>();
                       ExampleSimplePlayer jPlayer = playerInfos[j].gameObject.GetComponent<ExampleSimplePlayer>();

                       int copterSlot = ExampleSimplePlayer.getFreeCopter();
                       iPlayer.assignCopterSlot(2 * copterSlot);
                       jPlayer.assignCopterSlot(2 * copterSlot + 1);
                   }
               }
           }
        }*/
    }

    public static void makePairRequest(int requester, int requestee)
    {
       if (playerPairRequests.ContainsKey(requester))
       {
           playerPairRequests.Remove(requester);
       }

       playerPairRequests[requester] = requestee;
    }

    void OnLevelWasLoaded(int level)
    {
        //clear static variables as necessary;
        playerInfos = null;//.Clear();
        playerPairRequests = null;
        SessionID = -1;
    }

	public class PlayerInfo
	{
		public int score;
		public Color color;
        public string id;
        public int copterSlot = -1;
        //public int requestedPartner = -999;
        public GameObject gameObject;
        public int playerNumber
        {
            get 
            {
                return int.Parse(id.Split('|')[1]);
            }
        }
	}
}
