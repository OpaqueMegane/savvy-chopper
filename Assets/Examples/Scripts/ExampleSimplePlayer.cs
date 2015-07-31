using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

/* 

Problem now, information redundancy, isn't well centralized
No way to remember id -> copterslot assignments
 
 * Actual player objects (come in an out of existence)
 * Player info struct 
 * IDs
 * Partner reqests
 * Copter assignments
 
 * 
 * 
 * 
 * 
 
 
 */
namespace HappyFunTimesExample {

public class ExampleSimplePlayer : MonoBehaviour {
    // Classes based on MessageCmdData are automatically registered for deserialization
    // by CmdName.
    static Dictionary<string, int> idToCopterSlotMap; //maps player ids to which copter/which axis they control evens control up/down, odds left/right
    public static GameObject[] AllCopters = null;// = { null };
    Copter myCopter
    {
        get 
        {
				setUpAllCopters();

            if (myInfo != null && myInfo.copterSlot >= 0)
            {
                try
                {
                    return AllCopters[myInfo.copterSlot / 2].GetComponent<Copter>();
                }
                catch (Exception e)
                {
                    print("");
                    return null;
                }
                
            }
            else
            {
                return null;
            }
        }
    }

		public static void setUpAllCopters()
		{
			if (AllCopters == null)
			{
				Transform copterContainer = GameObject.Find("_ALL_COPTERS").transform;
				AllCopters = new GameObject[copterContainer.childCount];
				
				for (int i = 0; i < AllCopters.Length; i++)
				{
					AllCopters[i] = copterContainer.GetChild(i).gameObject;
				}
				
				idToCopterSlotMap = new Dictionary<string, int>();
				
			}
		}


    bool isHorizontalController
    {
        get 
        {
             if (myInfo != null && myInfo.copterSlot >= 0)
             {
                 return myInfo.copterSlot % 2 == 0;
             }
             return false;
        }
    }

    //enum PlayerState {WAITING_ASSIGNMENT, ASSIGNED};

    public PlayerIDManager.PlayerInfo myInfo = null;

    int quadrantIndex = 1;

    private float previousAlpha = -1;
    private float lastOrientationTime = float.NegativeInfinity;
    private bool startingFresh = true;
    private int spinCount = 0;
    private float rpm = 0;

    private System.Random m_rand = new System.Random();
    private NetPlayer m_netPlayer;
    private Vector3 m_position;
    private Color m_color;
    private string m_name;


    [CmdName("color")]
    private class MessageColor : MessageCmdData {
        public string color = "";    // in CSS format rgb(r,g,b)
    };


    [CmdName("move")]
    private class MessageMove : MessageCmdData {
        public float x = 0;
        public float y = 0;
    };


    [CmdName("orient")]
    private class MessageOrient : MessageCmdData
    {
        public float gamma = 0;
        public float beta = 0;
        public float alpha = 0;
    };

    [CmdName("setName")]
    private class MessageSetName : MessageCmdData {
        public MessageSetName() {  // needed for deserialization
        }
        public MessageSetName(string _name) {
            name = _name;
        }
        public string name = "";
    };

    [CmdName("busy")]
    private class MessageBusy : MessageCmdData {
        public bool busy = false;
    }

    // NOTE: This message is only sent, never received
    // therefore it does not need a no parameter constructor.
    // If you do receive one you'll get an error unless you
    // add a no parameter constructor.
    [CmdName("scored")]
    private class MessageScored : MessageCmdData {
        public MessageScored(int _points) {
            points = _points;
        }

        public int points;
    }

    [CmdName("character")]
    private class MessageCharacter : MessageCmdData
    {
        public MessageCharacter(string _character, bool isHorizontal)
        {
            character = _character;
			horizontalController= isHorizontal;
        }

        public string character;
		public bool horizontalController;
    }


    private class MessageAssignID : MessageCmdData
    {
        public MessageAssignID(string _id)
        {
            id = _id;
        }
        public string id;
    }

    private class MessageCheckID : MessageCmdData
    {
        public string id;
    }



    private class MessageRequestPartner : MessageCmdData
    {
        public int partnerId;
    }


    void InitializeNetPlayer(SpawnInfo spawnInfo) {
        m_netPlayer = spawnInfo.netPlayer;
        m_netPlayer.OnDisconnect += Remove;

        // Setup events for the different messages.
        m_netPlayer.RegisterCmdHandler<MessageColor>(OnColor);
        m_netPlayer.RegisterCmdHandler<MessageMove>(OnMove);
        m_netPlayer.RegisterCmdHandler<MessageSetName>(OnSetName);
        m_netPlayer.RegisterCmdHandler<MessageBusy>(OnBusy);
        m_netPlayer.RegisterCmdHandler<MessageOrient>(OnOrient);

        m_netPlayer.RegisterCmdHandler<MessageCmdData>("needID", OnNeedID);
        m_netPlayer.RegisterCmdHandler<MessageCheckID>("checkID", OnCheckID);
        m_netPlayer.RegisterCmdHandler<MessageRequestPartner>("requestPartner", OnRequestPartner);

        ExampleSimpleGameSettings settings = ExampleSimpleGameSettings.settings();
        m_position = new Vector3(m_rand.Next(settings.areaWidth), 0, m_rand.Next(settings.areaHeight));
        //transform.localPosition = m_position;

        SetName(spawnInfo.name);
    }

    void Start() {

        myInfo = null;
        //m_position = gameObject.transform.localPosition;
        m_color = new Color(0.0f, 1.0f, 0.0f);

        if (AllCopters == null)
        {
            Transform copterContainer = GameObject.Find("_ALL_COPTERS").transform;
            AllCopters = new GameObject[copterContainer.childCount];
            
            for (int i = 0; i < AllCopters.Length; i++)
            {
                AllCopters[i] = copterContainer.GetChild(i).gameObject;
            }

            idToCopterSlotMap = new Dictionary<string, int>();
           
        }
      
    }


    public static int getFreeCopter()
    {
        for (int i = 0; i < AllCopters.Length; i++)
        {
            bool horizOccupied = false;
            bool vertOccupied = false;
           
            for (int j = 0; j < PlayerIDManager.playerInfos.Count; j++)
            {
                int copterIdx = PlayerIDManager.playerInfos[j].copterSlot;
                if (copterIdx/2 == i)
                {
                    horizOccupied = copterIdx % 2 == 1;
                    vertOccupied = copterIdx % 2 == 0;
                }

            }

            if (!horizOccupied && !vertOccupied)
            {
                Copter copter = AllCopters[i].GetComponent<Copter>();
                return i;
            }
        }
        return -1;
    }
    /*
    public int getFreeCopterSlot()
    {
        //find a free control slot on a copter
        int pi = -10;
        for (int i = 0; i < AllCopters.Length; i++)
        {
            Copter copter = AllCopters[i].GetComponent<Copter>();
            
            if (copter.horizontalPlayer == null)
            {
                copter.horizontalPlayer = this.gameObject;
                //isHorizontalController = true;
                //this.myCopter = copter;
                pi = 2*i + 1;
   
                break;
            }
            else if (copter.verticalPlayer == null)
            {
                copter.verticalPlayer = this.gameObject;
                //isHorizontalController = false;
                //this.myCopter = copter;
                pi = 2*i;
                break;
            }
        }
        return pi;
    }*/

    public void Update() {
        bool haveId = myInfo != null;
        if (myCopter == null && haveId)
        {
            if (idToCopterSlotMap.ContainsKey(myInfo.id)) // if this ID is already mapped to a particular slot...
            {

            }
            //int mySlot = getFreeCopterSlot();
           //assignCopterSlot(mySlot);
 
        }
    }

    private void SetName(string name) {
        m_name = name;
    }

    public void OnTriggerEnter(Collider other) {
        // Because of physics layers we can only collide with the goal
        //m_netPlayer.SendCmd(new MessageScored(m_rand.Next(5, 15)));
    }

    private void Remove(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    private void OnColor(MessageColor data) {
        m_color = CSSParse.Style.ParseCSSColor(data.color);
        gameObject.GetComponent<Renderer>().material.color = m_color;
    }

    private void OnMove(MessageMove data) {
        /*ExampleSimpleGameSettings settings = ExampleSimpleGameSettings.settings();
        m_position.x = data.x * settings.areaWidth;
        m_position.z = settings.areaHeight - (data.y * settings.areaHeight) - 1;  // because in 2D down is positive.

        gameObject.transform.localPosition = m_position;*/
    }

    private void OnSetName(MessageSetName data) {
        if (data.name.Length == 0) {
            m_netPlayer.SendCmd(new MessageSetName(m_name));
        } else {
            SetName(data.name);
        }
    }

    private void OnBusy(MessageBusy data) {
        // not used.
    }

    private void OnRequestPartner(MessageRequestPartner data)
    {

            int requester = this.myInfo.playerNumber;
        int requestee = data.partnerId;
        PlayerIDManager.makePairRequest(requester, requestee);
			print(requester + " requests " + data.partnerId);
    }

    //Player requesting a new ID
    private void OnNeedID(MessageCmdData data)
    {
        Debug.Log("NEED ID!");
        int ID = PlayerIDManager.NewPlayer(this);
        myInfo = PlayerIDManager.playerInfos[PlayerIDManager.playerInfos.Count - 1];
        Debug.Log(m_color);
        myInfo.color = m_color;
        myInfo.id = PlayerIDManager.SessionID.ToString() + "|" + ID.ToString();
        //myInfo.copterSlot = getFreeCopterSlot();
        m_netPlayer.SendCmd("assignID", new MessageAssignID(myInfo.id));
        
        //m_netPlayer.SendCmd("restoreColor", new MessageRestoreColor(myInfo.color));
        //m_netPlayer.SendCmd("score", new MessageScored(0));
    }

    //Player checking an existing ID (from a cookie)
    private void OnCheckID(MessageCheckID data)
    {
        Debug.Log("CHECK ID " + data.id);
        if (PlayerIDManager.checkID(data.id))// if their ID was already good, we can assume(?) they have a copter assignment
        {
            //the phone had the correct ID, we're cool.
            //Get the appropriate player info
            Debug.Log("ID is valid");
            string[] sSplit = data.id.Split('|');
            int pID = int.Parse(sSplit[1]);
            myInfo = PlayerIDManager.playerInfos[pID];
            myInfo.id = data.id;
            //m_netPlayer.SendCmd("score", new MessageScored(myInfo.score));

			//remind of character, and player number
			m_netPlayer.SendCmd(new MessageCharacter(Copter.getFruitNameByIndex(this.myCopter.copterNumber), this.isHorizontalController));
			m_netPlayer.SendCmd("assignID", new MessageAssignID(myInfo.id));
          
        }
        else
        {
            Debug.Log("ID is invalid, sending a new one...");
            OnNeedID(null);
        }
    }

    Vector3 prevOrientation = Vector3.zero;

    Vector3 fixAngles(Vector3 toFix)
    {
        for (int i = 0; i < 3; i++)
        {
            if (toFix[i] > 180)
            {
                toFix[i] -= 360;
            }

            if (toFix[i] < -180)
            {
                toFix[i] += 360;
            }
        }
        return toFix;
    }

    private int windmillState = 0;
    int windmillDir = 0;

    List<Vector3> avWindow = new List<Vector3>();


		void checkForDebugMovement(MessageOrient data)
		{
			if (myCopter != null && Mathf.Abs(data.alpha) == 777)
			{
				myCopter.scored();
			}

			if (myCopter != null && Mathf.Abs(data.alpha) == 999)
			{
				
				if (this.isHorizontalController)
				{
					if (data.alpha == 999)
					{
						myCopter.thrustHorizontal(1.0f);
					}
					else
					{
						myCopter.thrustHorizontal(-1.0f);
					}
					//print(quadrantIndex + " right");
				}
				else
                {
                    
                    if (data.alpha == 999)
                    {
                        myCopter.thrustUp();
                    }
                    else
                    {
						myCopter.thrustDown();
                    }
                    
                }   
            }
        }
        
        private void OnOrient(MessageOrient data)
        {
      //increases to pos max,
        //decreases to negative min

			checkForDebugMovement(data);
            
            
            bool forwards = true;
            
            Vector3 curOrientation = new Vector3(data.alpha, data.beta, data.gamma);
            //curOrientation = fixAngles(curOrientation);
        
                avWindow.Add(curOrientation);

                int smoothWindowSize = 8;

                if (avWindow.Count >= smoothWindowSize)
                {
                    //avWindow.Clear();
                    if (avWindow.Count > smoothWindowSize)
                    {
                        avWindow.RemoveAt(0);
                    }

                    curOrientation = Vector3.zero;
                    for (int i = 0; i < avWindow.Count; i++ )
                    {
                        curOrientation += avWindow[i];
                    }
                    curOrientation = curOrientation * (800.0f / avWindow.Count);
                
                    
                        for (int i = 0; i < 3; i++)
                        {
                            if (Mathf.Abs(curOrientation[i]) > 2)
                            {
                                curOrientation[i]  *= 1.0f / Mathf.Abs(curOrientation[i]);
                            }
                            else
                            {
                                curOrientation[i]  = 0;
                            }
                           
                            
                        }
                    

                        if (curOrientation != prevOrientation)
                        {
                            //print("  " + curOrientation.x + "   " + curOrientation.y + "   " + curOrientation.z);
                        }
                }
                
               

             //   float accThresh = 100;
       //         if (Mathf.Abs(data.alpha) > accThresh || 
           //         Mathf.Abs(data.beta) > accThresh || 
     //             Mathf.Abs(data.gamma) > accThresh)
      //  {
             
            /*for (int i = 0; i < 3; i++ )
            {
                if (Mathf.Abs(curOrientation[i]) <= accThresh)
                {
                    curOrientation[i] = 0;
                }
                else
                {
                    curOrientation[i] /= Mathf.Abs(curOrientation[i]);
                }
            }*/
  

       // }


                float[] xSigns = { 1, -1, -1, 1 };
                float[] zSigns = { 1,1,-1,-1};
              

                int stateForward = (windmillState + 1) % 4;  
                int stateBack = (windmillState - 1 + 4) % 4;

                bool hitStateForwards = curOrientation.x == xSigns[stateForward] && curOrientation.z == zSigns[stateForward];
                bool hitStateBackwards = curOrientation.x == xSigns[stateBack] && curOrientation.z == zSigns[stateBack];
                if (hitStateForwards)
        {

            if (windmillDir >= 2)
            {
                windmillState = stateForward;   
                print("advance");


                if (this.isHorizontalController)
                {
                    myCopter.thrustHorizontal(-1.0f);
                    //print(quadrantIndex + " right");
                }
                else
                {
                    if (myCopter != null)
                    {
                        //myCopter.thrustUp();
                        myCopter.thrustDown();
                    }

                }

            }

            windmillDir = Mathf.Min(windmillDir + 1, 4);
   
        }

                else if (hitStateBackwards || data.alpha == -999)
        {
            if (windmillDir <= -2)
            {
                windmillState = stateBack;// (windmillState + 1) % 4;
                print("retreat");

                if (this.isHorizontalController)
                {
                    myCopter.thrustHorizontal(1.0f);
                    //print(quadrantIndex + " right");
                }
                else
                {
                    if (myCopter != null)
                    {
                        myCopter.thrustUp();
                    }

                }
            }

            windmillDir = Mathf.Max(windmillDir - 1, -4);

        }
        else
        {
            windmillDir = windmillDir < 0 ? windmillDir + 1 : windmillDir - 1;
        }
        prevOrientation = curOrientation;
  
        //The old method... needs speed increased in copter
        //checkPhoneSpinOnTable(data);
      
       
        
    }

     void checkPhoneSpinOnTable(MessageOrient data)
    {

        float angDiff = previousAlpha - data.beta;

        if (angDiff > 180)
        {
            angDiff -= 360;
        }

        if (angDiff < -180)
        {
            angDiff += 360;
        }
        //print(angDiff);
        //int range = 90;

        /*int checkRange = 45;
        int passAngle = 90;
        
        int minAngle = passAngle - checkRange;
        int maxAngle = passAngle + checkRange;

        bool pastAngle = previousAlpha >= minAngle && data.alpha <= maxAngle;*/

        bool[] pastQuadrantArray =
        {
            previousAlpha > 0 && data.alpha < 180, // passing 90
            previousAlpha > 90 && data.alpha < 270, // passing 180
            previousAlpha > 180 && data.alpha < 259, // passing 270
            previousAlpha > 270 && data.alpha < 90, // passing zero
        };

        bool[] pastQuadrantArrayBackwards =
        {
            data.alpha > 0 && previousAlpha < 180, // passing 90
            data.alpha > 90 && previousAlpha < 270, // passing 180
            data.alpha > 180 && previousAlpha < 259, // passing 270
            data.alpha > 270 && previousAlpha < 90, // passing zero
            
            
            
        };



        if ((pastQuadrantArray[quadrantIndex] && angDiff < 0) || data.alpha == 999) //ccw
        {
            quadrantIndex = (quadrantIndex + 1) % pastQuadrantArray.Length;
            //print(quadrantIndex);


            //print("rpm : " + rpm * 60 + " _ spins : " + spinCount);

            //this.rigidbody.AddForce(Vector3.up * upSpeed, ForceMode.Acceleration);

            //!!!

            if (this.isHorizontalController)
            {
                myCopter.thrustHorizontal(1.0f);
                //print(quadrantIndex + " right");
            }
            else
            {
                if (myCopter != null)
                {
                    myCopter.thrustUp();
                }

            }

            if (quadrantIndex == 0)
            {
                spinCount++;
            }
        }
        else if ((pastQuadrantArrayBackwards[quadrantIndex] && angDiff > 0) || data.alpha == -999)
        {
            if (this.isHorizontalController)
            {
                myCopter.thrustHorizontal(-1.0f);
                //print(quadrantIndex + " left");
            }

            quadrantIndex = ((quadrantIndex - 1) + pastQuadrantArray.Length) % pastQuadrantArray.Length;
            //print(quadrantIndex);
            if (quadrantIndex == 0)
            {


                spinCount--;


            }
        }


        previousAlpha = data.beta;
        
    }

    public bool isAssignedCopterSlot()
    {
        return myCopter != null;
    }

    public void assignCopterSlot(int slot)
    {
        print("character?");
        if (myInfo != null)
        {
            print("character");
            myInfo.copterSlot = slot;
            m_netPlayer.SendCmd(new MessageCharacter(Copter.getFruitNameByIndex(slot / 2), this.isHorizontalController));
            AllCopters[slot / 2].SetActive(true);
        }
        //isHorizontalController = slot % 2 == 1;
        //myCopter = AllCopters[slot / 2].GetComponent<Copter>();
    }

    void OnLevelWasLoaded(int level)
    {
        print("dddddddddddd");
        //clear static variables as necessary;
        idToCopterSlotMap = null;//.Clear();
        AllCopters = null;
    }
}



}  // namespace HappyFunTimesExample

