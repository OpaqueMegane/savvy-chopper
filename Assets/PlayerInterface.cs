using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

//Interface between HFT and whatever game logic drives each minigame.

public class PlayerInterface : MonoBehaviour 
{/*
	public delegate void PressAction();
	public event PressAction OnButtonDown;
	public event PressAction OnButtonUp;
	// Classes based on MessageCmdData are automatically registered for deserialization
	// by CmdName.



	private class MessagePad : MessageCmdData
	{
		public int pad;
		public int dir;
	}
	private class MessageAssignID : MessageCmdData {
		public MessageAssignID(string _id) {
			id = _id;
		}
		public string id;
	}

	private class MessageCheckID : MessageCmdData 
	{
		public string id;
	}

	private class MessageButton : MessageCmdData 
	{
		public bool pressed;
	};

	private class MessageColor : MessageCmdData 
	{
		public string color = "";    // in CSS format rgb(r,g,b)
	};

	private class MessageScored : MessageCmdData 
	{
		public MessageScored(int _points) {
			points = _points;
		}
		
		public int points;
	}

	private class MessageRestoreColor: MessageCmdData
	{
		public MessageRestoreColor(Color _color)
		{
			color = "rgb("+ 
				(_color.r * 255f).ToString() +","+
				(_color.g * 255f).ToString() +","+
				(_color.b * 255f).ToString() +")";
		}
		public string color;
	}
	
	void InitializeNetPlayer(SpawnInfo spawnInfo)
	{
		// Save the netplayer object so we can use it send messages to the phone
		m_netPlayer = spawnInfo.netPlayer;
		
		// Register handler to call if the player disconnects from the game.
		m_netPlayer.OnDisconnect += Remove;
		m_netPlayer.OnNameChange += ChangeName;
		
		// Setup events for the different messages.
		m_netPlayer.RegisterCmdHandler<MessageColor>("setColor", OnColor);
		m_netPlayer.RegisterCmdHandler<MessagePad> ("pad", OnPad);
		m_netPlayer.RegisterCmdHandler<MessageButton> ("action", OnButton);
		m_netPlayer.RegisterCmdHandler<MessageCmdData> ("needID", OnNeedID);
		m_netPlayer.RegisterCmdHandler<MessageCheckID> ("checkID", OnCheckID);
	}
	
	void Start() 
	{
		m_renderer = gameObject.GetComponent<Renderer>();
		m_position = gameObject.transform.localPosition;
		SetColor(new Color(0.0f, 1.0f, 0.0f));
	}
	
	void Update()
	{
		padInput = Vector2.Lerp (padInput, padInputRaw, Time.deltaTime * 5f);
		if(myInfo != null)
			m_netPlayer.SendCmd("score", new MessageScored(myInfo.score));
	}
	
	void OnGUI()
	{
		Vector2 size = m_guiStyle.CalcSize(m_guiName);
		Vector3 coords = Camera.main.WorldToScreenPoint(transform.position);
		m_nameRect.x = coords.x - size.x * 0.5f - 5.0f;
		m_nameRect.y = Screen.height - coords.y - 30.0f;
		GUI.Box(m_nameRect, m_name, m_guiStyle);
	}
	
	void SetName(string name) 
	{
		m_name = name;
		gameObject.name = "Player-" + m_name;
		m_guiName = new GUIContent(m_name);
		m_guiStyle.normal.textColor = Color.black;
		m_guiStyle.contentOffset = new Vector2(4.0f, 2.0f);
		Vector2 size = m_guiStyle.CalcSize(m_guiName);
		m_nameRect.width  = size.x + 12;
		m_nameRect.height = size.y + 5;
	}
	
	void SetColor(Color _color)
	{
		m_color = _color;
	//	m_renderer.material.color = m_color;
		Color[] pix = new Color[1];
		pix[0] = _color;
		Texture2D tex = new Texture2D(1, 1);
		tex.SetPixels(pix);
		tex.Apply();
		m_guiStyle.normal.background = tex;
	}
	
	public void OnTriggerEnter(Collider other) {
		// Because of physics layers we can only collide with the goal
		m_netPlayer.SendCmd("scored", new MessageScored(m_rand.Next(5, 15)));
	}
	
	private void Remove(object sender, EventArgs e) {
		Destroy(gameObject);
	}

	public Vector2 padInputRaw;
	public Vector2 padInput;

	private void OnPad(MessagePad data)
	{
		Debug.Log ("pad: " + data.pad + " dir: " + data.dir);
		switch (data.dir)
		{
			case -1:
				padInputRaw = Vector2.zero;
				break;
			case 0:
				padInputRaw = Vector2.right;
				break;
			case 1:
				padInputRaw = (Vector2.right + Vector2.up).normalized;
				break;
			case 2:
				padInputRaw =Vector2.up;
				break;
			case 3:
				padInputRaw = (Vector2.up -Vector2.right).normalized;
				break;
			case 4:
				padInputRaw = -Vector2.right;
				break;
			case 5:
				padInputRaw = (-Vector2.right - Vector2.up).normalized;
				break;
			case 6:
				padInputRaw = -Vector2.up;
				break;
			case 7:
				padInputRaw = (Vector2.right - Vector2.up).normalized;
				break;
		}
	}

	private void OnNeedID(MessageCmdData data)
	{
		Debug.Log("NEED ID!");
		int ID = PlayerIDManager.NewPlayer (this);
		myInfo = PlayerIDManager.playerInfos [PlayerIDManager.playerInfos.Count - 1];
		Debug.Log (m_color);
		myInfo.color = m_color;
		m_netPlayer.SendCmd("assignID", new MessageAssignID(PlayerIDManager.SessionID.ToString() + "|"+ID.ToString()));
		m_netPlayer.SendCmd("restoreColor", new MessageRestoreColor(myInfo.color));
		m_netPlayer.SendCmd("score", new MessageScored(0));
	}

	private void OnCheckID(MessageCheckID data)
	{
		Debug.Log ("CHECK ID " + data.id);
		if (PlayerIDManager.checkID (data.id)) 
		{
			//the phone had the correct ID, we're cool.
			//Get the appropriate player info
			Debug.Log("ID is valid");
			string[] sSplit = data.id.Split('|');
			int pID = int.Parse(sSplit[1]);
			myInfo = PlayerIDManager.playerInfos[pID];
			m_netPlayer.SendCmd("score", new MessageScored(myInfo.score));
			SetColor(myInfo.color);
			//set the player's phone to the correct color
			m_netPlayer.SendCmd("restoreColor", new MessageRestoreColor(myInfo.color));
		}
		else 
		{
			Debug.Log("ID is invalid, sending a new one...");
			OnNeedID(null);
		}
	}

	public bool buttonDown = false;
	private void OnButton(MessageButton data)
	{
		buttonDown = data.pressed;
		Debug.Log ("button down? " + buttonDown + " " + data.pressed);

		if (buttonDown && OnButtonDown != null) 
			OnButtonDown ();
		else if (!buttonDown && OnButtonUp != null)
			OnButtonUp ();
	}

	private void OnColor(MessageColor data) {
		SetColor(CSSParse.Style.ParseCSSColor(data.color));
	}

	private void ChangeName(object sender, EventArgs e) {
		SetName(m_netPlayer.Name);
	}
	
	[HideInInspector]
	public Color m_color;
	public PlayerIDManager.PlayerInfo myInfo;

	private System.Random m_rand = new System.Random();
	private NetPlayer m_netPlayer;
	private Renderer m_renderer;
	private Vector3 m_position;

	private string m_name;
	private GUIStyle m_guiStyle = new GUIStyle();
	private GUIContent m_guiName = new GUIContent("");
	private Rect m_nameRect = new Rect(0,0,0,0);*/
}
