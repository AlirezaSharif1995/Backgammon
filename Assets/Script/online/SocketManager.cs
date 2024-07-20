using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using Unity.VisualScripting;
using UnityEngine;
public class SocketManager : MonoBehaviour
{
    public static SocketManager Instance { get; private set; }

    public SocketIOUnity socket;
    public string serverUrl = "";
    public bool Connected;
    public string error;

    PlayerInfo playerInfo;

    public class OpponentFoundInGameData
    {
        public CheckerData[] holeSetup;
        public multipleClass multiple;
        public string currentPlayer;
        public int[] rolledDice;
        public bool isBlocked;

        public class CheckerData
        {
            public int id;
            public int column;
        }
        public class multipleClass
        {
            public string owner;
            public int ratio;
        }
    }

    public class PauseData
    {
        public string playerToken;
    }

    public class FinishData
    {
        public string winnerPlayer;
        public int winPrize;
    }

    public class DoubleData
    {
        public string currentPlayer;
        public int ratio;
    }
    public class ChangeTurnData
    {
        public string currentPlayer;
    }

    public class MoveData
    {
        public string currentPlayer;
        public int fromPosition;
        public int[] byDice;
    }

    public class UndoData
    {
        public string currentPlayer;
        public int fromPosition;
        //public int[] byDice;
    }

    public class OpponentFoundData
    {
        public string playerToken;
        public string nickname;
        public int avatarId;
        public int coin;
        public int level;
        public int starterDice;
        public float mainTime;
        public float turnRestTime;
        public float notReadyTime;
    }

    [System.Serializable]
    public class RollDiceData
    {
        public string currentPlayer;
        public int[] rolledDice;
        public bool isBlocked;
        //public int[][] availableMoves;
    }

    // Game Data

    public GameDataClass GameData;

    [System.Serializable]
    public class GameDataClass
    {
        public bool FirstMove;
        public RollDiceData FirstRollDice;
        public bool live;
        public playerData Player;
        public OpponentData Opponent;
        public float mainTime;
        public float turnRestTime;
        public float notReadyTime;
        public bool gameLive;
        public OpponentFoundInGameData OpponentFoundInGameData;
    }

    [System.Serializable]
    public class playerData
    {
        public int starterDice;
    }

    [System.Serializable]
    public class OpponentData
    {
        public string playerToken;
        public string Nickname;
        public int AvatarID;
        public int Coin;
        public int Jvl;
        public int Level;
        public int starterDice;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        
        error = "";
    }

    public void Connect()
    {
        var uri = new Uri(serverUrl);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"accessToken", playerInfo.PlayerData.accessToken }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.Polling
        });
        //socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.Options.AutoUpgrade = false;

        socket.OnPing += (sender, e) =>
        {
            Debug.Log("socket: Ping");
        };

        socket.OnPong += (sender, e) =>
        {
            Debug.Log("socket: Pong: " + e.TotalMilliseconds);
        };

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("<color=blue> socket: Connected </color>");

            Connected = true;
        };

        socket.OnError += (sender, e) =>
        {
            Debug.Log("<color=blue> socket Error: " + sender+"e:"+e +"</color>");
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("<color=blue> socket: Disconnect: " + e +"</color>");

            Connected = false;
        };

        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log("socket: " + $"{DateTime.Now} Reconnecting: attempt = {e}");
        };

        Debug.Log("<color=blue> socket: Connecting... </color>");

        socket.Connect();
    }

    void OnDestroy()
    {
        if (socket != null)
            socket.Dispose();
    }
    private void OnApplicationQuit()
    {
        if (socket != null)
            socket.Disconnect();
    }

    public void ErrorMessage(int code)
    {
        error +="\n"+ "Error: ";
        switch(code)
        {
            case -1: error += "Internet connection problem"; break;
            case 0: error += "Problem on getting information"; break;
        }
        Debug.Log("socket: "+ error);
    }

}
