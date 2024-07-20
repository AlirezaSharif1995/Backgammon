using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameSetup : MonoBehaviour
{
    public SocketManager socketManager = SocketManager.Instance;

    #region General
    // ----------------------------------------------------------- General
    [Header("General")]
    public bool Updated;
    public bool DeveloperMode;
    public bool showLog;
    public bool socketConnected;
    public bool AllReady;
    // Move Proccess
    bool moveProccessing;
    public bool moveVerifying;
    System.Collections.IEnumerator EmitMoveProccessCo;
    //
    public GameObject Log;
    Text textLog;
    public Camera mainCamera;
    public Canvas canvas;
    public GameObject Checker;
    public BoxCollider boardBoxCollider;
    public PlayerInfo playerInfo;
    PauseController pauseController;
    public CheckerController touchNearChecker;
    FinishController finishController;
    public FinishCount finishCount;
    #endregion

    #region UI
    // ----------------------------------------------------------- UI
    [System.Serializable]
    public class UIClass
    {
        public CanvasGroup Blocked;
        public Text BlockedText;
        public List<Sprite> diceImages = new List<Sprite>();
        public List<Sprite> Checker2D = new List<Sprite>();
        public CanvasGroup InternetProblemUI;
        public GameObject WaitUI;
        public UnityEngine.UI.Text WaitTextUI;
        public Text coinCount;
        public Transform WaitLoadingUI;


        // player
        public UnityEngine.UI.Text playerNickname;
        public UnityEngine.UI.Image playerTurnTiming;
        public UnityEngine.UI.Image playerFullTiming;
        public GameObject playerTimer;
        public CanvasGroup playerRolls;
        public List<Image> playerRollImages = new List<Image>();
        public bool playerRollsUpdated;
        public List<int> playerRollNumbers = new List<int>();
        public UnityEngine.UI.Image playerCheckerUI;
        public Image playerAvatarImage;
        public Text playerTurnTimeText;
        public Text playerFullTimeText;

        //opponent
        public UnityEngine.UI.Text opponentNickname;
        public UnityEngine.UI.Image opponentTurnTiming;
        public UnityEngine.UI.Image opponentFullTiming;
        public GameObject opponentAi;
        public GameObject opponentTimer;
        public CanvasGroup opponentRolls;
        public List<Image> opponentRollImages = new List<Image>();
        public bool opponentRollsUpdated;
        public List<int> opponentRollNumbers = new List<int>();
        public UnityEngine.UI.Image opponentCheckerUI;
        public Image opponentAvatarImage;
        public Text opponentTurnTimeText;
        public Text opponentFullTimeText;

        public void SetupImages()
        {
            for (int i = 0; i < 4; i++)
            {
                playerRollImages.Add(playerRolls.transform.GetChild(2).GetChild(i).GetComponent<UnityEngine.UI.Image>());
                opponentRollImages.Add(opponentRolls.transform.GetChild(2).GetChild(i).GetComponent<UnityEngine.UI.Image>());
            }
        }
    }
    public UIClass UI;
    #endregion

    #region Audio
    [System.Serializable]
    public class AudioClass
    {
        public AudioSource myTurn;
        public AudioSource opponentTurn;
        public AudioSource Click;
        public AudioSource Blocked;
        public AudioSource DoubleRequest;
    }
    public AudioClass Audio;
    #endregion

    #region VFX
    [System.Serializable]
    public class VfxClass
    {
        public GameObject Kick;
    }
    public VfxClass Vfx;
    #endregion

    #region Double
    [Header("Double")]
    DoubleController DoubleController;
    #endregion

    #region Finish Action
    public bool FinishAction;
    #endregion

    #region Opponent Information
    [Header("Opponent Information")]
    public string opponentNickname;
    public int opponentAvatarID;
    #endregion

    #region Kick
    bool kickblock;
    CheckerController kickedChecker;
    #endregion

    #region Type Options
    // ----------------------------------------------------------- Type Options

    [System.Serializable]
    public class OptionsClass
    {
        public Transform startSetup;
        public ColumnOptionsClass ColumnOption;
        public PlaceOptionsClass PlaceOption;
        public outColumnOptionsClass outColumnOption;
        public finishColumnOptionClass finishColumnOption;
    }
    public OptionsClass Options;

    #endregion

    #region General Options
    // ----------------------------------------------------------- General Options

    [System.Serializable]
    public class GeneralOptionsClass
    {
        public type Type;
        [Range(0, 10)]
        public float fixSpeed;
        public Location PlayerLocation;
        public sides playerSide;
        public Color HighLightColor;
        public GameObject BoardMiddleColl;

        [Header("Timing")]
        public float fullTime;
        public float turnTime;
        public float notReadyTime;
        public float EmitTryTime;

        [Header("Auto-fill with the 'Playerlocation'")]
        public Location OpponentLocation;
        public sides OpponentSide;

        [Header("Opponent Type")]
        public bool AI;
        public bool Online;

        [Header("Delay")]
        [Range(1, 500)]
        public float delayTurn;
        [Range(1, 500)]
        public float delayAIMove;
        [Range(1, 500)]
        public float delayUpdateNaxtMove;

        [Header("Column")]
        public GameObject HighLightPrefab;
        public GameObject HighLightFinishPrefab;
        public Vector3 rangeSizeColumn;
        public bool showEditorColumn;

        [Header("Place")]
        public bool showEditorPlace;

        [Header("FinishColumn")]
        public Vector3 rangeSizeFinishColumn;
        public Vector3 rangeDistance;

        [Header("RollDice")]
        public DiceController DiceControllerPlayer;
        public DiceController DiceControllerOpponent;
        public DiceAnimController DiceAnimConPlayer;
        public DiceAnimController DiceAnimConOpponent;

        [Range(-3, 3)]
        public float textDistanceX;
        [Range(0, 3)]
        public float textDistanceY;
        [Range(0, 3)]
        public float textDistanceZ;

        [Range(0, 40)]
        public float textSize;

        [Header("Dice Material")]
        public Material Material1;
        public Material Material2;
        public Material Material3;

        public void SetOpponent()
        {
            Location location = Location.Up;
            if (PlayerLocation == Location.Up) location = Location.Down;
            OpponentLocation = location;

            sides side = sides.White;
            if (playerSide == sides.White) side = sides.Black;
            OpponentSide = side;
        }
    }

    public GeneralOptionsClass GeneralOptions;

    public enum Location
    {
        Down,
        Up
    }

    public enum type
    {
        type1,
        type2,
        type3
    }

    public enum direction
    {
        Up,
        Down
    }

    public enum sides
    {
        none,
        White,
        Black
    }

    #endregion

    #region Timing
    // ----------------------------------------------------------- Timing

    [System.Serializable]
    public class TimeClass
    {
        public bool timePassing;

        [Header("Player")]
        public float playerFullTime;
        public float playerTurnTime;
        public float playerNotReadyTime;

        [Header("Opponent")]
        public float opponentFullTime;
        public float opponentTurnTime;
        public float opponentNotReadyTime;
    }
    public TimeClass Timing;

    #endregion

    #region Column options
    // ----------------------------------------------------------- Column Options

    [System.Serializable]
    public class ColumnOptionsClass
    {
        [Header("Setup at start")]
        [Range(0, 2)]
        public float distanceX;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 3)]
        public float middleDistance;
    }

    public ColumnOptionsClass ColumnOption;

    #endregion

    #region Place options
    // ----------------------------------------------------------- Place Options

    [System.Serializable]
    public class PlaceOptionsClass
    {
        [Header("Setup at start")]
        [Range(0, 2)]
        public float distanceZ;
        [Range(0, 2)]
        public float distanceY;

    }

    public PlaceOptionsClass PlaceOptions;
    #endregion

    #region OutColumn Options

    // ----------------------------------------------------------- OutColumn Options

    [System.Serializable]
    public class outColumnOptionsClass
    {
        [Range(0, 10)]
        public float distanceX;
        [Range(0, 5)]
        public float distanceY;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 20)]
        public float distanceBetween;
    }

    public outColumnOptionsClass outColumnOption;

    #endregion

    #region FinishColumn Options
    // ----------------------------------------------------------- finishColumn Options

    [System.Serializable]
    public class finishColumnOptionClass
    {
        [Range(0, 10)]
        public float distanceX;
        [Range(-5, 5)]
        public float distanceY;
        [Range(0, 20)]
        public float distanceZ;
        [Range(0, 20)]
        public float distanceBetween;
        [Range(0, 1)]
        public float RowDistance;
    }

    public finishColumnOptionClass finishColumnOption;
    #endregion

    #region Column Class
    // ----------------------------------------------------------- Column Class

    [System.Serializable]
    public class ColumnClass
    {
        public int ID;
        public sides Side;
        public int FullCount;
        public Vector3 Pos;
        public int maxCount;
        public direction Direction;
        public bool HighLight;
        public GameObject HighLightObject;
        public List<PlaceClass> Place = new List<PlaceClass>();
        public int BlockedRoll;

        public void Update()
        {
            FullCount = 0;
            foreach (var place in Place)
            {
                if (place.Full)
                    FullCount++;
            }
        }
    }

    public List<ColumnClass> Column = new List<ColumnClass>();
    public List<ColumnClass> outColumn = new List<ColumnClass>();
    public List<ColumnClass> finishColumn = new List<ColumnClass>();

    #endregion

    #region place Class
    // ----------------------------------------------------------- Place Class

    [System.Serializable]
    public class PlaceClass
    {
        public bool Full;
        public Vector3 Pos;
        public CheckerController Checker;
    }
    #endregion

    #region Roll
    // ----------------------------------------------------------- Roll

    [System.Serializable]
    public class RollClass
    {
        //player
        public bool playerTurn;
        public bool playerAction;
        public bool playerHome;
        public bool playerDone;
        public bool playerProcessing;
        public bool AutoRoll;
        //opponent
        public bool opTurn;
        public bool opponentAction;
        public bool opponentHome;
        public bool opponentProcessing;
        //other
        public RollButton rollButton;
        public DoneButton doneButton;
        public List<int> Rolls = new List<int>();
        public int playerStartRoll;
        public int opponentStartRoll;

        //public List<int> Blocked = new List<int>();
    }

    [Header("Roll")]
    public RollClass Roll;
    #endregion

    #region HighLight
    // ----------------------------------------------------------- HighLight

    [Header("HighLight Options")]
    public Material highLightMaterial;
    bool change;
    Color myColor;
    public bool disableCheckersHighLight;

    public CheckerController[] Checkers;

    #endregion

    #region History
    // ----------------------------------------------------------- History
    [Header("History")]
    public UndoButton UndoButton;

    [System.Serializable]
    public class HistoryClass
    {
        public CheckerController checker;
        public ColumnClass startColumn;
        public ColumnClass targetColumn;
        public List<int> roll = new List<int>();
        public bool haveKick;
        public CheckerController checkerKick;
        public ColumnClass startColumnKick;
        public ColumnClass targetColumnKick;
    }

    public List<HistoryClass> History = new List<HistoryClass>();
    #endregion

    #region Starting
    // ----------------------------------------------------------- Starting
    [Header("Starting")]
    public bool Starting;
    public bool StartingRollDice;

    [System.Serializable]
    public class SpawnLocationClass
    {
        public Transform Right;
        public Transform Left;
    }
    public SpawnLocationClass SpawnLocation;
    #endregion

    #region Void Start
    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- void Start

    void Start()
    {
        /*textLog = Log.transform.GetChild(0).GetComponent<Text>();
        if (showLog) Debug.Log("<color=red> ---------------- CONSOLE: ----------------</color>");*/

        playerInfo = FindObjectOfType<PlayerInfo>();
        UI.BlockedText = UI.Blocked.transform.GetChild(1).GetComponent<Text>();
        pauseController = FindObjectOfType<PauseController>();
        finishController = FindObjectOfType<FinishController>();
        finishCount = FindObjectOfType<FinishCount>();

        socketManager = SocketManager.Instance;

        UI.SetupImages();
        UI.Blocked.alpha = 0;
        UI.InternetProblemUI.alpha = 0;
        UI.WaitUI.gameObject.SetActive(false);
        UI.playerFullTimeText.text = "0:" + Timing.playerFullTime.ToString("F0");
    }

    void SetScreen()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    #endregion

    #region Void Update
    // -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_- void Update
    void Update()
    {
        if (!Updated && playerInfo.Updated)
        {
            /*if (playerInfo.PlayerData.Level == 1)
                GeneralOptions.Type = type.type1;
            if (playerInfo.PlayerData.Level == 2)
                GeneralOptions.Type = type.type2;
            if (playerInfo.PlayerData.Level == 3)
                GeneralOptions.Type = type.type3;*/  // itsnew

            SetScreen();
            myColor = GeneralOptions.HighLightColor;
            GeneralOptions.SetOpponent();
            SetupColumns();
            SetupOutColumn();
            SetupFinishColumn();
            SetupPlaces();
            Starting = false;

            // Set Checker UI
            if (GeneralOptions.playerSide == sides.White)
                UI.playerCheckerUI.sprite = UI.Checker2D[0];
            else
                UI.playerCheckerUI.sprite = UI.Checker2D[1];

            if (GeneralOptions.OpponentSide == sides.White)
                UI.opponentCheckerUI.sprite = UI.Checker2D[0];
            else
                UI.opponentCheckerUI.sprite = UI.Checker2D[1];

            // Set Online
            if (socketManager.GameData.live)
            {
                GeneralOptions.AI = false;
                GeneralOptions.Online = true;

                opponentNickname = socketManager.GameData.Opponent.Nickname;
                opponentAvatarID = socketManager.GameData.Opponent.AvatarID;

                GeneralOptions.turnTime = socketManager.GameData.turnRestTime;
                GeneralOptions.fullTime = socketManager.GameData.mainTime;
                GeneralOptions.notReadyTime = socketManager.GameData.notReadyTime;

                Debug.Log("<color=blue> ( Online ) </color>");

                SocketOn();
                pauseController.PauseOn();
            }
            else
            // Set Local
            if (GeneralOptions.AI)
            {
                opponentAvatarID = UnityEngine.Random.Range(1, playerInfo.avatars.Count);
                string[] names = { "Alice", "Bob", "Charlie", "David", "Eva", "Frank", "Grace", "Henry", "Isabel", "Jack" };
                int randomIndex = UnityEngine.Random.Range(0, names.Length);
                opponentNickname = names[randomIndex];

                AllReady = true;
                Debug.Log("<color=blue> ( AI ) </color>");

                BeginSetup();
            }

            Updated = true;
        }

        if (Updated)
        {
            Log.SetActive(showLog);
            SetInternetProblemUI();
            if (Starting) setStartingPos();
            //SetHighLightMaterial();
            UndoButton.canUndo = Roll.playerTurn && History.Count > 0;
            UndoButton.textCount.text = "x" + History.Count.ToString();

            foreach (ColumnClass column in Column)
            {
                column.Update();
                column.HighLightObject.SetActive(column.HighLight);

                if (column.FullCount > 0)
                {
                    if (column.Place[0].Checker.Side == CheckerController.side.White)
                        column.Side = sides.White;
                    else
                    if (column.Place[0].Checker.Side == CheckerController.side.Black)
                        column.Side = sides.Black;
                    else
                        column.Side = sides.none;
                }
            }

            foreach (ColumnClass column in outColumn)
                column.Update();

            foreach (ColumnClass column in finishColumn)
            {
                column.Update();
                column.HighLightObject.SetActive(column.HighLight);
            }

            SetHomeCheckers();

            setTime();
            //player
            UI.playerTurnTiming.fillAmount = LineRange(GeneralOptions.turnTime, Timing.playerTurnTime);
            UI.playerFullTiming.fillAmount = LineRange(GeneralOptions.fullTime, Timing.playerFullTime);
            UI.playerNickname.text = playerInfo.PlayerData.nickname;
            UI.playerTimer.SetActive(Roll.playerTurn);
            UI.playerAvatarImage.sprite = playerInfo.avatars[playerInfo.PlayerData.avatarId];
            UI.playerAvatarImage.enabled = !Roll.playerTurn;
            UI.playerTurnTimeText.text = "0:" + Timing.playerTurnTime.ToString("F0");
            if (Roll.playerTurn) UI.playerFullTimeText.text = "0:" + Timing.playerFullTime.ToString("F0");
            //opponent
            UI.opponentTurnTiming.fillAmount = LineRange(GeneralOptions.turnTime, Timing.opponentTurnTime);
            UI.opponentFullTiming.fillAmount = LineRange(GeneralOptions.fullTime, Timing.opponentFullTime);
            UI.opponentNickname.text = opponentNickname;
            UI.opponentTimer.SetActive(Roll.opTurn);
            UI.opponentAvatarImage.sprite = playerInfo.avatars[opponentAvatarID];
            UI.opponentAvatarImage.enabled = !Roll.opTurn;
            UI.opponentTurnTimeText.text = "0:" + Timing.opponentTurnTime.ToString("F0");
            if (Roll.opTurn) UI.opponentFullTimeText.text = "0:" + Timing.opponentFullTime.ToString("F0");
            
            // Local
            if (GeneralOptions.AI)
            {
                Roll.playerProcessing = Roll.playerTurn && Roll.Rolls.Count == 0 && !Roll.rollButton.canRoll && History.Count == 0;
                Roll.opponentProcessing = Roll.opTurn && Roll.Rolls.Count == 0 && !Roll.rollButton.canRoll && History.Count == 0;
            }
            else
            // Online
            if (GeneralOptions.Online)
            {
                Roll.playerProcessing = pauseController.Pause || !socketManager.Connected;
                Roll.opponentProcessing = pauseController.Pause || !socketManager.Connected;
            }

            UI.opponentAi.SetActive(GeneralOptions.AI);

            if (GeneralOptions.AI) CheckPlayerBlocked();

            //GeneralOptions.BoardMiddleColl.SetActive(!Roll.playerAction); // itsnew
            SetAlphaRollUI();
            UI.coinCount.text = playerInfo.PlayerData.coin.ToString();

            float rotSpeed = 150 * Time.deltaTime;
            Quaternion rot = UI.WaitLoadingUI.transform.rotation;
            Vector3 eulerRot = rot.eulerAngles;
            eulerRot.z += rotSpeed;
            rot = Quaternion.Euler(eulerRot);
            UI.WaitLoadingUI.transform.rotation = rot;

            if (socketManager.Connected && !socketConnected)
            {
                Timing.playerNotReadyTime = GeneralOptions.notReadyTime;
                socketConnected = true;
            }
        }
    }
    #endregion
    
    public void ToggleAutoRoll()
    {
        Roll.AutoRoll = !Roll.AutoRoll;
    }

    #region Set Alpha Roll UI
    void SetAlphaRollUI()
    {
        float speed = 10 * Time.deltaTime;

        // player
        if (Roll.playerTurn && Roll.playerAction && UI.playerRollsUpdated)
        {
            UI.playerRolls.gameObject.SetActive(true);

            if (UI.playerRolls.alpha < 1)
                UI.playerRolls.alpha += speed;

            if (UI.playerRolls.alpha >= 1)
                UI.playerRolls.alpha = 1;
        }
        else
        {
            if (UI.playerRolls.alpha > 0)
                UI.playerRolls.alpha -= speed;

            if (UI.playerRolls.alpha <= 0)
            {
                UI.playerRolls.alpha = 0;
                UI.playerRolls.gameObject.SetActive(false);
            }
        }

        // opponent
        if (Roll.opTurn && Roll.opponentAction && UI.opponentRollsUpdated)
        {
            UI.opponentRolls.gameObject.SetActive(true);

            if (UI.opponentRolls.alpha < 1)
                UI.opponentRolls.alpha += speed;

            if (UI.opponentRolls.alpha >= 1)
                UI.opponentRolls.alpha = 1;
        }
        else
        {
            if (UI.opponentRolls.alpha > 0)
                UI.opponentRolls.alpha -= speed;

            if (UI.opponentRolls.alpha <= 0)
            {
                UI.opponentRolls.alpha = 0;
                UI.opponentRolls.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Set Roll UI
    public IEnumerator SetRollUI(sides side, List<int> rolls)
    {
        CanvasGroup Rolls = UI.playerRolls;
        List<Image> RollImages = new List<Image>();
        List<int> RollNumbers = new List<int>();

        bool player = false;
        bool opponent = false;

        yield return new WaitForSeconds(10 * Time.deltaTime);

        if (side == GeneralOptions.playerSide)
        {
            Rolls = UI.playerRolls;
            RollImages = UI.playerRollImages;
            RollNumbers = UI.playerRollNumbers;

            player = true;

        }
        else if (side == GeneralOptions.OpponentSide)
        {
            Rolls = UI.opponentRolls;
            RollImages = UI.opponentRollImages;
            RollNumbers = UI.opponentRollNumbers;

            opponent = true;
        }

        RollImages[0].enabled = true;
        RollImages[1].enabled = true;
        RollImages[2].enabled = false;
        RollImages[3].enabled = false;

        RollImages[0].sprite = UI.diceImages[rolls[0] - 1];
        RollImages[1].sprite = UI.diceImages[rolls[1] - 1];

        RollNumbers[0] = rolls[0];
        RollNumbers[1] = rolls[1];

        Transform frameDown = Rolls.transform.GetChild(1);
        Vector3 DownPos = Rolls.transform.GetChild(4).position;
        Vector3 UpPos = Rolls.transform.GetChild(3).position;

        bool haveFour = false;
        bool isDouble = false;

        if (rolls[0] == rolls[1])
            isDouble = true;

        if (rolls.Count > 2)
        {
            RollImages[2].enabled = true;
            RollImages[3].enabled = true;

            RollImages[2].sprite = UI.diceImages[rolls[2] - 1];
            RollImages[3].sprite = UI.diceImages[rolls[3] - 1];

            RollNumbers[2] = rolls[2];
            RollNumbers[3] = rolls[3];

            haveFour = true;

            frameDown.position = DownPos;
        }
        else
            frameDown.position = UpPos;

        if (player)
            UI.playerRollsUpdated = true;
        else if (opponent)
            UI.opponentRollsUpdated = true;

        float alphaHigh = 1f;
        float alphaLow = 0.3f;
        while (Rolls.alpha > 0 || (Roll.playerTurn && player) || (Roll.opTurn && opponent))
        {
            yield return new WaitForSeconds(Time.deltaTime);
            Color color0 = RollImages[0].color;
            Color color1 = RollImages[1].color;
            Color color2 = RollImages[2].color;
            Color color3 = RollImages[3].color;

            if (!isDouble)
            {
                if (haveFour)
                {
                    if (Roll.Rolls.Contains(RollNumbers[3]))
                        color3.a = alphaHigh;
                    else
                        color3.a = alphaLow;

                    if (Roll.Rolls.Contains(RollNumbers[2]))
                        color2.a = alphaHigh;
                    else
                        color2.a = alphaLow;
                }

                if (Roll.Rolls.Contains(RollNumbers[1]))
                    color1.a = alphaHigh;
                else
                    color1.a = alphaLow;

                if (Roll.Rolls.Contains(RollNumbers[0]))
                    color0.a = alphaHigh;
                else
                    color0.a = alphaLow;

            }
            else
            {
                if (Roll.Rolls.Count > 3)
                    color3.a = alphaHigh;
                else
                    color3.a = alphaLow;

                if (Roll.Rolls.Count > 2)
                    color2.a = alphaHigh;
                else
                    color2.a = alphaLow;

                if (Roll.Rolls.Count > 1)
                    color1.a = alphaHigh;
                else
                    color1.a = alphaLow;

                if (Roll.Rolls.Count > 0)
                    color0.a = alphaHigh;
                else
                    color0.a = alphaLow;
            }

            RollImages[0].color = color0;
            RollImages[1].color = color1;
            RollImages[2].color = color2;
            RollImages[3].color = color3;
        }

        if (player)
            UI.playerRollsUpdated = false;
        else if (opponent)
            UI.opponentRollsUpdated = false;
    }
    #endregion

    #region Set Blocked UI
    IEnumerator SetBlockedUI(string english, string persian, string arabic)
    {
        UI.BlockedText.text = english;
        
        Audio.Blocked.Play();
        float speed = 10;

        while (UI.Blocked.alpha < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            UI.Blocked.alpha += speed * Time.deltaTime;
        }

        yield return new WaitForSeconds(100 * Time.deltaTime);

        while (UI.Blocked.alpha > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            UI.Blocked.alpha -= speed * Time.deltaTime;
        }
    }
    #endregion

    public float LineRange(float max, float current)
    {
        return current / max;
    }

    #region GetTouchNearChecker
    public CheckerController GetTouchNearChecker()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        CheckerController targetChecker = null;

        if (touchNearChecker == null)
        {
            for (int i = 0; i < Checkers.Length; i++)
            {
                float touchDistanceTarget = float.MaxValue;
                float touchDistance = Vector3.Distance(mainCamera.WorldToScreenPoint(Checkers[i].transform.position), mousePosScreen);

                if (targetChecker != null)
                {
                    touchDistanceTarget = Vector3.Distance(mainCamera.WorldToScreenPoint(targetChecker.transform.position), mousePosScreen);
                    if (touchDistance < touchDistanceTarget) targetChecker = Checkers[i];

                }
                else if (touchDistance < 120)
                {
                    targetChecker = Checkers[i];
                }
            }
        }

        return targetChecker;
    }
    #endregion

    #region Set Time
    void setTime()
    {
        if ((Roll.playerTurn || Roll.opTurn) && !Roll.playerProcessing && !Roll.opponentProcessing)
        {
            if (!Timing.timePassing)
            {
                if (Roll.playerTurn)
                    Timing.playerTurnTime = GeneralOptions.turnTime;
                else
                if (Roll.opTurn)
                    Timing.opponentTurnTime = GeneralOptions.turnTime;

                Timing.timePassing = true;

            } else
            {
                if (Roll.playerTurn)
                {
                    Timing.playerTurnTime -= Time.deltaTime;

                    if (Timing.playerTurnTime <= 0)
                    {
                        Timing.playerTurnTime = 0;
                        Timing.playerFullTime -= Time.deltaTime;

                        if (Timing.playerFullTime <= 0)
                        {
                            Timing.playerFullTime = 0;

                            if (GeneralOptions.AI)
                            {
                                finishController.playerWin = false;
                                finishController.Finished = true;
                            }

                            //Debug.Log("Player Time Out");
                        }
                    }
                }
                else
                if (Roll.opTurn)
                {
                    Timing.opponentTurnTime -= Time.deltaTime;

                    if (Timing.opponentTurnTime <= 0)
                    {
                        Timing.opponentTurnTime = 0;
                        Timing.opponentFullTime -= Time.deltaTime;

                        if (Timing.opponentFullTime <= 0)
                        {
                            Timing.opponentFullTime = 0;

                            if (GeneralOptions.AI)
                            {
                                finishController.playerWin = true;
                                finishController.Finished = true;
                            }

                            //Debug.Log("Opponent Time Out");
                        }

                    }
                }
            }
        }

        if (Roll.playerTurn && !Roll.playerAction && Timing.playerTurnTime < GeneralOptions.turnTime / 2.5f) // force to roll dice
            Roll.rollButton.ButtonClick();

        if (!Roll.playerTurn)
            Timing.playerTurnTime = GeneralOptions.turnTime;

        if (!Roll.opTurn)
            Timing.opponentTurnTime = GeneralOptions.turnTime;
    }
    #endregion

    #region Set Internet Problem UI
    void SetInternetProblemUI()
    {
        float speed = 10 * Time.deltaTime;

        if (GeneralOptions.Online && !socketManager.Connected && !finishController.Finished)
        {
            UI.InternetProblemUI.gameObject.SetActive(true);

            if (UI.InternetProblemUI.alpha < 1)
                UI.InternetProblemUI.alpha += speed;

            if (UI.InternetProblemUI.alpha <= 1)
                UI.InternetProblemUI.alpha = 1;
        }
        else
        {
            if (UI.InternetProblemUI.alpha > 0)
                UI.InternetProblemUI.alpha -= speed;

            if (UI.InternetProblemUI.alpha <= 0)
            {
                UI.InternetProblemUI.alpha = 0;
                UI.InternetProblemUI.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Verify Move
    public void VerifyMove(HistoryClass history)
    {
        if (GeneralOptions.Online && socketManager.Connected)
        {
            var obj = new
            {
                fromPosition = history.startColumn.ID == 24 ? 25 : history.startColumn.ID + 1,
                byDice = history.roll
            };

            if (DeveloperMode)
                Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> Move: (Sanded) </color> <color=white>" + obj.fromPosition + " | " + obj.byDice[0] + "</color>");

            socketManager.socket.Emit("Move", obj);
            moveVerifying = true;

            moveProccessing = true;
            EmitMoveProccessCo = EmitProccessMove(obj);
            StopCoroutine(EmitMoveProccessCo);
            StartCoroutine(EmitMoveProccessCo);

            Debug.Log("<color=yellow> Move </color> <color=white>  Verifying... </color>");
        }
    }

    IEnumerator EmitProccessMove(params object[] obj)
    {
        yield return new WaitForSeconds(GeneralOptions.EmitTryTime * Time.deltaTime);
        if (moveProccessing)
        {
            socketManager.socket.Emit("Move", obj);
            Debug.Log("<color=red> (TRY) </color> <color=yellow> Move </color> <color=white>  Verifying... </color>");
        }
    }
    #endregion

    #region Socket On
    public void SocketOn()
    {
        print("<color=red> Listening To Server </color>");

        #region TEST
        socketManager.socket.On("TEST_EMIT", (response) =>
        {
            Debug.Log(response.ToString());
        });
        #endregion

        #region Opponent Found
        socketManager.socket.On("OpponentFound", (response) =>
        {
            try
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> OpponentFound:  </color> <color=white>" + response.ToString() + "</color>");

                var result = JsonConvert.DeserializeObject<List<SocketManager.OpponentFoundInGameData>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    SetGame(result[2].holeSetup, result[2].multiple, result[3].currentPlayer, result[3].rolledDice, result[3].isBlocked);
                    Debug.Log("<color=yellow> OpponentFound: </color> <color=white> Done </color>");
                });
            }catch(Exception e) { Debug.LogException(e); }
        });
        #endregion

        #region Move
        socketManager.socket.On("Move", (response) =>
        {
            try
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> Move: </color> <color=white>" + response.ToString() + "</color>");

                var result = JsonConvert.DeserializeObject<List<SocketManager.MoveData>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    string name = "Opponent";

                    if (result[0].currentPlayer == playerInfo.PlayerData.token)
                    {
                        name = "Player";
                        Debug.Log("<color=green> Move: (" + name + "):  </color> <color=white> Verified </color>");
                        moveVerifying = false;

                        if (EmitMoveProccessCo != null)
                        {
                            moveProccessing = false;
                            StopCoroutine(EmitMoveProccessCo);
                        }
                    }
                    else
                    {
                        if (Roll.opTurn)
                        {
                            ColumnClass columnStart = null;

                            int fromColumn = 1;
                            bool isOut = result[0].fromPosition == 25;

                            if (isOut)
                            {
                                fromColumn = -1;
                                columnStart = outColumn[1];
                            }
                            else
                            {
                                fromColumn = 25 - result[0].fromPosition - 1;
                                columnStart = Column[fromColumn];
                            }

                            CheckerController checkerTarget = GetLastChecker(columnStart, GeneralOptions.OpponentSide);

                            int targetColumnID = 0;

                            for (int i = 0; i < result[0].byDice.Length; i++)
                                targetColumnID += result[0].byDice[i];

                            int finalyColumn = fromColumn + targetColumnID;
                            print(finalyColumn);

                            ColumnClass columnTarget = null;

                            if (finalyColumn > 23 || finalyColumn < 0)
                            {
                                columnTarget = finishColumn[0];
                            }
                            else
                            {
                                columnTarget = Column[fromColumn + targetColumnID];
                            }

                            checkerTarget.CheckFinish(columnTarget);
                            checkerTarget.UpdateRolls(columnTarget, true, false);

                            PlaceChecker(checkerTarget, columnTarget, true, columnStart, false);

                            Debug.Log("<color=yellow> Move: (" + name + "):  </color> <color=white> Done </color>");
                            moveVerifying = false;
                        }
                    }
                });
            }catch(Exception e) { Debug.LogException(e); }
        });
        #endregion

        #region Undo
        socketManager.socket.On("Undo", (response) =>
        {
            try
            {
                if (DeveloperMode)
                    Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> Undo:  </color> <color=white>" + response.ToString() + "</color>");

                var result = JsonConvert.DeserializeObject<List<SocketManager.UndoData>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    UndoButton.undoProccessing = false;
                    StopCoroutine(UndoButton.EmitProccess("Undo"));

                    if (EmitMoveProccessCo != null)
                    {
                        moveProccessing = false;
                        StopCoroutine(EmitMoveProccessCo);
                    }

                    moveVerifying = false;

                    // Player
                    if (playerInfo.PlayerData.token == result[0].currentPlayer)
                    {
                        UndoButton.Undo(true, result[0].fromPosition - 1);
                    }
                    // Opponent
                    else
                    {
                        UndoButton.Undo(true, 25 - result[0].fromPosition - 1);

                    }

                    Debug.Log("<color=yellow> Undo: </color> <color=white>" + "Done" + "</color>");
                });
            }catch (Exception e) { Debug.LogException(e); }
        });
        #endregion

        #region Logger
        socketManager.socket.On("Logger", (response) =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Debug.Log("<color=red> Logger: " + response.ToString() + " </color>");
            });
        });
        #endregion

        #region Change Turn
        socketManager.socket.On("ChangeTurn", (response) =>
        {
            try
            {
                if (DeveloperMode)
                    Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> ChangeTurn:  </color> <color=white>" + response.ToString() + "</color>");

                var result = JsonConvert.DeserializeObject<List<SocketManager.ChangeTurnData>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Roll.doneButton.submitProccessing = false;
                    StopCoroutine(Roll.doneButton.EmitProccess("Submit"));

                    if (result[0].currentPlayer == playerInfo.PlayerData.token)
                    {
                        EndTurn("opponent", GeneralOptions.delayTurn);
                        Debug.Log("<color=green> ChangeTurn: (Player) </color> <color=white> Done </color>");
                    }
                    else
                    {
                        EndTurn("player", GeneralOptions.delayTurn);
                        Debug.Log("<color=green> ChangeTurn: (Opponent) </color> <color=white> Done </color>");
                    }
                });
            }catch (Exception e) { Debug.LogException(e);}
        });
        #endregion

        #region DoubleOffer
        socketManager.socket.On("DoubleOffer", (response) =>
        {
            Debug.Log(response);
            var result = JsonConvert.DeserializeObject<List<SocketManager.DoubleData>>(response.ToString());

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                DoubleController.doubleOfferProccessing = false;
                StopCoroutine(DoubleController.EmitProccess("DoubleOffer"));

                if (result[0].currentPlayer == playerInfo.PlayerData.token)
                {
                    DoubleController.Requesting = true;
                    DoubleController.RequestedMe = true;
                    DoubleController.myRequest = false;

                    Debug.Log("<color=green> DoubleOffer (to Player) </color> <color=white> Done </color>");
                }
                else
                {
                    Debug.Log("<color=green> DoubleOffer (to Opponent) </color> <color=white> Done </color>");
                }
            });
        });
        #endregion

        #region AcceptDouble
        socketManager.socket.On("AcceptDouble", (response) =>
        {
            var result = JsonConvert.DeserializeObject<List<SocketManager.DoubleData>>(response.ToString());

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                DoubleController.doubleAcceptProccessing = false;
                StopCoroutine(DoubleController.EmitProccess("AcceptDouble"));

                StartCoroutine(DoubleController.EndRequest(true, result[0].ratio,GeneralOptions.OpponentSide));
                Debug.Log("<color=green> AcceptDouble </color> <color=white> Done </color>");
            });
        });
        #endregion
    }

    #region RollDice Opponent
    void RollDiceOpponent()
    {
        socketManager.socket.On("RollDice", (response) =>
        {
            if (DeveloperMode)
                Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> RollDice: (Opponent) </color> <color=white>" + response.ToString() + "</color>");

            var result = JsonConvert.DeserializeObject<List<SocketManager.RollDiceData>>(response.ToString());
            
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (socketManager.GameData.FirstMove) // First RollDice
                {
                    socketManager.GameData.FirstRollDice.currentPlayer = result[0].currentPlayer;
                    socketManager.GameData.FirstRollDice.rolledDice = result[0].rolledDice;

                    StartCoroutine(StartRollDice(50));

                    UI.WaitUI.gameObject.SetActive(false);
                    AllReady = true;

                    Debug.Log("<color=green> RollDice: (First) </color> <color=white> Done </color>");
                }
                else
                if (result[0].currentPlayer != playerInfo.PlayerData.token) // opponent RollDice
                {
                    GeneralOptions.DiceAnimConPlayer.EndShow();
                    GeneralOptions.DiceAnimConOpponent.EndShow();

                    Roll.Rolls.Clear();

                    int e1 = result[0].rolledDice[0];
                    int e2 = result[0].rolledDice[1];

                    Roll.Rolls.Add(e1);
                    Roll.Rolls.Add(e2);

                    if (e1 == e2)
                    {
                        Roll.Rolls.Add(e1);
                        Roll.Rolls.Add(e2);
                    }

                    Roll.opponentAction = true;

                    float delay = 1;
                    if (StartingRollDice) delay = 30;

                    StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(delay, result[0].isBlocked));

                    Debug.Log("<color=green> RollDice: (Opponent) </color> <color=white> Done </color>");

                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls)); 
                }
            });
        });
    }
    #endregion

    #endregion

    #region Set Game
    void SetGame(SocketManager.OpponentFoundInGameData.CheckerData[] holeSetup, SocketManager.OpponentFoundInGameData.multipleClass multiple,string currentPlayer, int[] roll, bool isBlocked)
    {
        if (DeveloperMode)
            Debug.Log("<color=red> (DEVELOPER MODE) </color> <color=yellow> SetGame: </color> holeSetup: " + holeSetup.ToString() + " | multiple: " + multiple.ToString());

        ClearFullPlaces();

        // Set Checkers Columns
        for (int i = 0; i < holeSetup.Length; i++)
        {
            PlaceClass placeTarget = null;
            int columnTargetID = holeSetup[i].column;
            int checkerTargetID = 29 - holeSetup[i].id;
            ColumnClass columnTarget = null;


            if (columnTargetID == 0)
            {
                if (checkerTargetID >= 15)
                    placeTarget = GetEmpityPlace(outColumn[1]); // Player   
                else
                {
                    columnTarget = finishColumn[1];
                    placeTarget = GetEmpityPlace(finishColumn[1]); // Opponent
                }
                    
            }
            else
            if (columnTargetID == 25)
            {
                if (checkerTargetID >= 15)
                {
                    placeTarget = GetEmpityPlace(finishColumn[0]); // Player
                    columnTarget = finishColumn[0];
                }
                else
                    placeTarget = GetEmpityPlace(outColumn[0]); // Opponent
            }
            else
                placeTarget = GetEmpityPlace(Column[columnTargetID-1]);

            print(i);
            CheckerController checkerTarget = Checkers[checkerTargetID];
            placeTarget.Checker = checkerTarget;
            checkerTarget.FixTarget = placeTarget.Pos;
            placeTarget.Full = true;
            checkerTarget.canFix = true;

            if (columnTarget != null)
                checkerTarget.CheckFinish(columnTarget);
        }

        // Set Double owner
        if (multiple.owner == playerInfo.PlayerData.token)
            DoubleController.Side = GeneralOptions.playerSide;
        else if (multiple.owner != "")
            DoubleController.Side = GeneralOptions.OpponentSide;
        else
            DoubleController.Side = sides.none;

        // Set Double Multiple
        DoubleController.Multiple = multiple.ratio;
        DoubleController.SetMultipleRot();

        ResetCheckers();

        if (roll[0] != 0)
        {
            Roll.Rolls.Clear();

            if (socketManager.GameData.FirstMove)
            {
                socketManager.GameData.FirstRollDice.currentPlayer = currentPlayer;
                socketManager.GameData.FirstRollDice.rolledDice = roll;

                Roll.Rolls.Add(roll[0]);
                Roll.Rolls.Add(roll[1]);

                if (roll[0] == roll[1])
                {
                    Roll.Rolls.Add(roll[0]);
                    Roll.Rolls.Add(roll[1]);
                }

                StartCoroutine(StartRollDice(50));

                socketManager.GameData.FirstMove = false;

                SetRollUI(GeneralOptions.playerSide, Roll.Rolls);

            }else
            {
                GeneralOptions.DiceAnimConPlayer.EndShow();
                GeneralOptions.DiceAnimConOpponent.EndShow();

                Roll.Rolls.Add(roll[0]);
                Roll.Rolls.Add(roll[1]);

                if (roll[0] == roll[1])
                {
                    Roll.Rolls.Add(roll[0]);
                    Roll.Rolls.Add(roll[1]);
                }

                if (currentPlayer == playerInfo.PlayerData.token)
                {
                    StartCoroutine(GeneralOptions.DiceAnimConPlayer.RollDice(1, isBlocked));
                    StartCoroutine(SetRollUI(GeneralOptions.playerSide, Roll.Rolls));

                    Roll.opTurn = false;
                    Roll.opponentAction = false;
                    Roll.playerTurn = true;
                    Roll.playerAction = true;
                }
                else
                {
                    StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(1, isBlocked));
                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls));

                    Roll.playerTurn = false;
                    Roll.playerAction = false;
                    Roll.opTurn = true;
                    Roll.opponentAction = true;
                }
            }
        }else
        {
            if (currentPlayer == playerInfo.PlayerData.token)
            {
                Roll.playerTurn = true;
                Roll.playerAction = true;
                Roll.opTurn = false;
                Roll.opponentAction = false;

                StartCoroutine(SetTurn("player", GeneralOptions.delayTurn));
            }
            else
            {
                Roll.playerTurn = false;
                Roll.playerAction = false;
                Roll.opTurn = true;
                Roll.opponentAction = true;

                StartCoroutine(SetTurn("opponent", GeneralOptions.delayTurn));
            }
        }

        Debug.Log("<color=green> SetGame </color> <color=white> Updated </color>");
    }
    #endregion

    #region get Empity Place
    PlaceClass GetEmpityPlace(ColumnClass column)
    {
        for (int i = 0; i < column.Place.Count; i++)
            if (!column.Place[i].Full) return column.Place[i];

        return null;
    }
    #endregion

    #region Clear Full Places
    void ClearFullPlaces()
    {
        // Column
        foreach(ColumnClass column in Column)
        {
            foreach(PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }

        // Finish Column
        foreach (ColumnClass column in finishColumn)
        {
            foreach (PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }

        // OutColumn
        foreach (ColumnClass column in outColumn)
        {
            foreach (PlaceClass place in column.Place)
            {
                place.Full = false;
            }
        }
    }
    #endregion

    #region Set Home Checkers
    void SetHomeCheckers()
    {
        List<CheckerController> playerCheckers = new List<CheckerController>();
        List<CheckerController> opponentCheckers = new List<CheckerController>();

        foreach (CheckerController checker in Checkers)
        {
            if (((checker.Side == CheckerController.side.White && GeneralOptions.playerSide == sides.White) || (checker.Side == CheckerController.side.Black && GeneralOptions.playerSide == sides.Black)) && !playerCheckers.Contains(checker))
                playerCheckers.Add(checker);

            if (((checker.Side == CheckerController.side.White && GeneralOptions.OpponentSide == sides.White) || (checker.Side == CheckerController.side.Black && GeneralOptions.OpponentSide == sides.Black)) && !opponentCheckers.Contains(checker))
                opponentCheckers.Add(checker);
        }

        int playerHomeCount = 0;
        int opponentHomeCount = 0;

        for (int i = 0; i < playerCheckers.Count; i++)
        {
            //player
            if (GeneralOptions.PlayerLocation == Location.Down && (playerCheckers[i].columnStart.ID <= 5 && (playerCheckers[i].columnStart.ID >= 0 && playerCheckers[i].columnStart.ID <= 23)) || playerCheckers[i].finished) playerHomeCount += 1;
            else
            if (GeneralOptions.PlayerLocation == Location.Up && (playerCheckers[i].columnStart.ID >= 18 && (playerCheckers[i].columnStart.ID >= 0 && playerCheckers[i].columnStart.ID <= 23)) || playerCheckers[i].finished) playerHomeCount += 1;

            //opponent
            if (GeneralOptions.OpponentLocation == Location.Down && (opponentCheckers[i].columnStart.ID <= 5 && (opponentCheckers[i].columnStart.ID >= 0 && opponentCheckers[i].columnStart.ID <= 23)) || opponentCheckers[i].finished) opponentHomeCount += 1;
            else
            if (GeneralOptions.OpponentLocation == Location.Up && (opponentCheckers[i].columnStart.ID >= 18 && (opponentCheckers[i].columnStart.ID >= 0 && opponentCheckers[i].columnStart.ID <= 23)) || opponentCheckers[i].finished) opponentHomeCount += 1;
        }

        Roll.playerHome = playerHomeCount == 15;
        Roll.opponentHome = opponentHomeCount == 15;
    }
    #endregion

    #region Setup Finish Column
    void SetupFinishColumn()
    {
        finishColumnOptionClass finishColumnOption = Options.finishColumnOption;

        for (int i = 0; i < 2; i++)
            finishColumn.Add(new ColumnClass());


        finishColumn[0].Pos = Column[0].Pos;
        finishColumn[0].Pos.x += finishColumnOption.distanceX;
        //finishColumn[0].Pos.y -= finishColumnOption.distanceY; // itsnew
        finishColumn[0].Pos.y += finishColumnOption.distanceZ; // itsnew

        finishColumn[1].Pos = finishColumn[0].Pos;
        finishColumn[1].Pos.y -= finishColumnOption.distanceBetween; // itsnew

        finishColumn[0].ID = 24;
        finishColumn[1].ID = -1;

        finishColumn[0].Direction = direction.Up;
        finishColumn[1].Direction = direction.Up;

        if ((GeneralOptions.playerSide == sides.White && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.Black) && GeneralOptions.PlayerLocation == Location.Up)
        {
            finishColumn[0].Side = sides.Black;
            finishColumn[1].Side = sides.White;

        }
        else
        if ((GeneralOptions.playerSide == sides.Black && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.White) && GeneralOptions.PlayerLocation == Location.Up)
        {
            finishColumn[0].Side = sides.White;
            finishColumn[1].Side = sides.Black;
        }

        float dis = -0.3f;
        for (int i = 0; i < 2; i++)
            finishColumn[i].HighLightObject = Instantiate(GeneralOptions.HighLightFinishPrefab, new Vector3(finishColumn[i].Pos.x, finishColumn[i].Pos.y, finishColumn[i].Pos.z + dis), GeneralOptions.HighLightFinishPrefab.transform.rotation);

    }
    #endregion

    #region Setup Out Column
    void SetupOutColumn()
    {
        outColumnOptionsClass outColumnOption = Options.outColumnOption;;

        for (int i = 0; i < 2; i++)
            outColumn.Add(new ColumnClass());

        outColumn[0].Pos = Column[0].Pos;
        outColumn[0].Pos.x -= outColumnOption.distanceX;
        //outColumn[0].Pos.y += outColumnOption.distanceY; // itsnew
        outColumn[0].Pos.y += outColumnOption.distanceZ; // itsnew

        outColumn[1].Pos = outColumn[0].Pos;
        outColumn[1].Pos.y -= outColumnOption.distanceBetween; // itsnew

        outColumn[0].ID = 24;
        outColumn[1].ID = -1;

        outColumn[0].Direction = direction.Down;
        outColumn[1].Direction = direction.Up;

        if ((GeneralOptions.playerSide == sides.White && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.Black) && GeneralOptions.PlayerLocation == Location.Up)
        {
            outColumn[0].Side = sides.White;
            outColumn[1].Side = sides.Black;

        } else
        if ((GeneralOptions.playerSide == sides.Black && GeneralOptions.PlayerLocation == Location.Down) || (GeneralOptions.playerSide == sides.White) && GeneralOptions.PlayerLocation == Location.Up)
        {
            outColumn[0].Side = sides.Black;
            outColumn[1].Side = sides.White;
        }
    }
    #endregion

    #region CheckBlock
    bool CheckBlock()
    {
        foreach (CheckerController checker in Checkers)
        {
            if (checker.canDrag)
                return false;
        }
        return true;
    }
    #endregion

    #region Starting To End
    IEnumerator StartingToEnd(float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        try
        {
            Starting = false;

            /*foreach (CheckerController checker in Checkers)
                checker.SetLayer("Default");*/ // itsnew

            if (GeneralOptions.AI)
                StartCoroutine(StartRollDice(50));
            else
            if (GeneralOptions.Online && socketManager.Connected)
            {
                if (!socketManager.GameData.gameLive)
                {
                    UI.WaitUI.gameObject.SetActive(true);
                    RollDiceOpponent();
                    socketManager.socket.Emit("Ready");
                    Debug.Log("<color=green> READY </color>");
                }
                else
                {
                    socketManager.socket.Emit("Resume");
                    Debug.Log("<color=green> Resume </color>");
                    socketManager.GameData.gameLive = false;
                    AllReady = true;
                }
            }
        }catch(Exception e) { Debug.LogException(e); }
    }
    #endregion

    #region Start RollDice
    IEnumerator StartRollDice(float delay)
    {
        if (Roll.playerStartRoll == 0 || Roll.playerStartRoll == Roll.opponentStartRoll)
        {
            StartingRollDice = true;
            GeneralOptions.DiceAnimConPlayer.EndShow();
            GeneralOptions.DiceAnimConOpponent.EndShow();
            yield return new WaitForSeconds(delay * Time.deltaTime);

            GeneralOptions.DiceAnimConPlayer.StartRollDice();
            GeneralOptions.DiceAnimConOpponent.StartRollDice();

            Debug.Log("Start Roll: " + "Player: " + Roll.playerStartRoll + " | " + "Opponent: " + Roll.opponentStartRoll);
        }
    }
    #endregion

    #region Check Start RollDice
    public IEnumerator checkStartRollDice(float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (Roll.playerStartRoll == Roll.opponentStartRoll)
        {
            StartCoroutine(StartRollDice(50));
        }
        else
        if (Roll.playerStartRoll > Roll.opponentStartRoll)
        {
            StartCoroutine(SetTurn("player", GeneralOptions.delayTurn));
            StartCoroutine(SetBlockedUI("YOU START", "شما شروع کنید" ,"أنت تبدأ"));
            
            GeneralOptions.DiceAnimConPlayer.StopDice();
            GeneralOptions.DiceAnimConOpponent.StopDice();
            GeneralOptions.DiceAnimConPlayer.spriteDice1.sprite = GeneralOptions.DiceAnimConPlayer.dices[Roll.playerStartRoll - 1];
            GeneralOptions.DiceAnimConOpponent.spriteDice1.sprite = GeneralOptions.DiceAnimConOpponent.dices[Roll.opponentStartRoll - 1];
        }
        else
        {
            StartCoroutine(SetTurn("opponent", GeneralOptions.delayTurn));
            StartCoroutine(SetBlockedUI("OPPONENT START", "حریف شروع می کند", "الخصم يبدأ"));
            
            GeneralOptions.DiceAnimConPlayer.StopDice();
            GeneralOptions.DiceAnimConOpponent.StopDice();
            GeneralOptions.DiceAnimConPlayer.spriteDice1.sprite = GeneralOptions.DiceAnimConPlayer.dices[Roll.playerStartRoll - 1];
            GeneralOptions.DiceAnimConOpponent.spriteDice1.sprite = GeneralOptions.DiceAnimConOpponent.dices[Roll.opponentStartRoll - 1];
        }
    }
    #endregion

    #region SetTurn
    IEnumerator SetTurn(string side, float delay)
    {
        foreach (CheckerController checker in Checkers)
        {
            checker.canDrag = false;
            checker.HighLight = false;
            checker.columnTarget.Clear();
        }

        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (side == "player")
        {
            Roll.playerTurn = true;
            Audio.myTurn.Play();
            
            if ((socketManager.GameData.FirstMove && GeneralOptions.Online) || (StartingRollDice && GeneralOptions.AI))
            {
                StartCoroutine(Roll.rollButton.PlayerRoll());
                print("Roll");
            }
                
            else
                Roll.rollButton.canRoll = true;
        }
        else
        if (side == "opponent")
        {
            Roll.opTurn = true;
            Audio.opponentTurn.Play();

            // Local
            if (GeneralOptions.AI)
            {
                //int DoubleReq = UnityEngine.Random.Range(1, 20);
                bool goodDouble = (finishCount.playerCount < finishCount.opponentCount);
                if (goodDouble && DoubleController.Multiple < 64 && (DoubleController.Side == GeneralOptions.OpponentSide || DoubleController.Side == sides.none) && !StartingRollDice)
                    DoubleController.RequestDouble(GeneralOptions.playerSide);
                else
                {
                    if (!StartingRollDice)
                    {
                        GeneralOptions.DiceAnimConPlayer.EndShow();
                        GeneralOptions.DiceAnimConOpponent.EndShow();

                        float delay2 = 1;
                        if (StartingRollDice) delay2 = 30;

                        StartCoroutine(GeneralOptions.DiceAnimConOpponent.RollDice(delay2,false));

                    }
                    else
                    {
                        Roll.Rolls.Clear();

                        int e1 = Roll.playerStartRoll;
                        int e2 = Roll.opponentStartRoll;

                        Roll.Rolls.Add(e1);
                        Roll.Rolls.Add(e2);

                        if (e1 == e2)
                        {
                            Roll.Rolls.Add(e1);
                            Roll.Rolls.Add(e2);
                        }

                        StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls));
                        StartCoroutine(GeneralOptions.DiceAnimConOpponent.StartTurn(100,false));
                    }
                }
            }else
            // Online
            if (GeneralOptions.Online)
            {
                if (socketManager.GameData.FirstMove)
                {
                    Roll.Rolls.Clear();

                    int e1 = socketManager.GameData.FirstRollDice.rolledDice[0];
                    int e2 = socketManager.GameData.FirstRollDice.rolledDice[1];

                    Roll.Rolls.Add(e1);
                    Roll.Rolls.Add(e2);

                    if (e1 == e2)
                    {
                        Roll.Rolls.Add(e1);
                        Roll.Rolls.Add(e2);
                    }

                    Roll.opponentAction = true;

                    StartCoroutine(SetRollUI(GeneralOptions.OpponentSide, Roll.Rolls));

                    socketManager.GameData.FirstMove = false;
                }
            }
        }
        Debug.Log(side + " Turn");
    }
    #endregion

    #region  Blocked Online
    public void BlockedOnline(string side)
    {
        Roll.Rolls.Clear();

        if (side == "player")
        {
            StartCoroutine(SetBlockedUI("BLOCKED", "مسدود شد", "تم الحظر"));
            Roll.doneButton.playerDone();
            Debug.Log("<color=yellow> Blocked </color> <color=white> Player </color>");
        }
        else if (side == "opponent")
        {
            StartCoroutine(SetBlockedUI("OPPONENT BLOCKED", "حریف مسدود شد", "تم حظر الخصم"));
            Debug.Log("<color=yellow> Blocked </color> <color=white> Opponent </color>");

        }
    }
    #endregion

    #region AI Act
    public IEnumerator AIAct(float delay, string type)
    {
        if (Roll.Rolls.Count > 0)
            UsableChecker(GeneralOptions.OpponentLocation, GeneralOptions.OpponentSide, Roll.Rolls);
        yield return new WaitForSeconds(delay * Time.deltaTime);

        List<CheckerController> avableCheckers = new List<CheckerController>();

        for (int i = 0; i < Checkers.Length; i++)
        {
            if (Checkers[i].columnTarget.Count > 0 && !avableCheckers.Contains(Checkers[i]))
                avableCheckers.Add(Checkers[i]);
        }

        if (avableCheckers.Count > 0)
        {
            CheckerController checkerTarget = null;
            ColumnClass columnTarget = null;
            if (type == "random")
            {
                int checkerTargetID = -1;
                checkerTargetID = UnityEngine.Random.Range(0, avableCheckers.Count);
                checkerTarget = avableCheckers[checkerTargetID];
                
                int columnTargetID = -1;
                columnTargetID = UnityEngine.Random.Range(0, checkerTarget.columnTarget.Count);
                columnTarget = checkerTarget.columnTarget[columnTargetID];

            }else if (type == "intelligent")
            {
                // Check For Kick Checker (1)
                #region Check For Kick Checker

                bool canKick = false;
                bool haveTarget = false;
                foreach (CheckerController checker in avableCheckers)
                {
                    if (checker.columnTarget.Count > 0)
                    {
                        haveTarget = true;
                        break;
                    }
                }

                if (haveTarget)
                {
                    foreach (CheckerController checker in avableCheckers)
                    {
                        foreach (ColumnClass column in checker.columnTarget)
                        {
                            if (column.Side == GeneralOptions.playerSide && column.FullCount == 1)
                            {
                                canKick = true;
                                columnTarget = column;
                                checkerTarget = checker;
                                break;
                            }
                        }
                    }
                }
                #endregion
                if (!canKick || !haveTarget)
                {
                    // Check For Out Home Checker (2)
                    #region Check For Out Home Checker

                    List<CheckerController> outHomeCheckers = new List<CheckerController>();
                    
                    foreach (CheckerController checker in avableCheckers)
                    {
                        if (checker.columnStart.ID < 18)
                            outHomeCheckers.Add(checker);
                    }

                    bool haveAllyColumn = false;
                    if (outHomeCheckers.Count > 0)
                    {
                        // Check For Have Ally Column (3)
                        #region Check For Have Ally Column
                        
                        haveTarget = false;
                        foreach (CheckerController checker in outHomeCheckers)
                        {
                            if (checker.columnTarget.Count > 0)
                            {
                                haveTarget = true;
                                break;
                            }
                        }

                        if (haveTarget)
                        {
                            foreach (CheckerController checker in outHomeCheckers)
                            {
                                foreach (ColumnClass column in checker.columnTarget)
                                {
                                    if (column.FullCount > 0 && column.Side == GeneralOptions.OpponentSide)
                                    {
                                        haveAllyColumn = true;
                                        columnTarget = column;
                                        checkerTarget = checker;
                                        break;
                                    }
                                }
                            }
                        }
                        
                        if (!haveAllyColumn || !haveTarget)
                        {
                            int targetCheckerID = UnityEngine.Random.Range(0, outHomeCheckers.Count);
                            int targetColumnID = UnityEngine.Random.Range(0,outHomeCheckers[targetCheckerID].columnTarget.Count);
                            columnTarget = outHomeCheckers[targetCheckerID].columnTarget[targetColumnID];
                            checkerTarget = outHomeCheckers[targetCheckerID];
                        }
                        #endregion
                    }
                    #endregion
                    else
                    {
                        // Check For Can Finish Checker (4)
                        #region Check For Can Finish Checker
                        
                        haveTarget = false;
                        foreach (CheckerController checker in avableCheckers)
                        {
                            if (checker.columnTarget.Count > 0)
                            {
                                haveTarget = true;
                                break;
                            }
                        }
                        
                        bool canFinishChecker = false;
                        if (haveTarget)
                        {
                            
                            foreach (CheckerController checker in avableCheckers)
                            {
                                foreach (ColumnClass column in checker.columnTarget)
                                {
                                    if (column.ID > 23)
                                    {
                                        canFinishChecker = true;
                                        columnTarget = column;
                                        checkerTarget = checker;
                                    }
                                }
                            }
                        }

                        if (!canFinishChecker || !haveTarget)
                        {
                            int targetCheckerID = UnityEngine.Random.Range(0, avableCheckers.Count);
                            int targetColumnID = UnityEngine.Random.Range(0,avableCheckers[targetCheckerID].columnTarget.Count);
                            columnTarget = avableCheckers[targetCheckerID].columnTarget[targetColumnID];
                            checkerTarget = avableCheckers[targetCheckerID];
                        }
                        #endregion
                    }
                }
            }

            if (checkerTarget != null && columnTarget != null)
            {
                checkerTarget.CheckFinish(columnTarget);
                ResetCheckers();
                checkerTarget.UpdateRolls(columnTarget, false, false);
                PlaceChecker(checkerTarget, columnTarget, true, checkerTarget.columnStart, false);

                if (Roll.Rolls.Count > 0)
                    StartCoroutine(checkerTarget.UpdateNaxtMove(GeneralOptions.delayUpdateNaxtMove, "opponent"));
                else
                    EndTurn("opponent", GeneralOptions.delayTurn);
            }else
                EndTurn("opponent", GeneralOptions.delayTurn);
        }
        else
        {
            EndTurn("opponent", GeneralOptions.delayTurn);
            Roll.Rolls.Clear();
            Debug.Log("Opponent Blocked !"); 
            StartCoroutine(SetBlockedUI("OPPONENT BLOCKED", "حریف مسدود شد", "تم حظر الخصم"));
        }
    }
    #endregion

    #region Get Touch Position
    public Vector3 GetTouchPosition()
    {
        Vector3 touchPosition = Vector3.zero;

        /*if (Input.touchCount > 0)
        {
            // Touch
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (boardBoxCollider.Raycast(ray, out RaycastHit hit, 10000))
                touchPosition = hit.point;
        }
        else
        {
            // Mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (boardBoxCollider.Raycast(ray, out RaycastHit hit, 10000))
                touchPosition = hit.point;
        }*/ // itsnew

        touchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // itsnew CODE

        return touchPosition;
    }
    #endregion

    #region Clear ColumnHighLight
    void ClearColumnHighLight()
    {
        foreach (ColumnClass column in Column)
            column.HighLight = false;

        foreach (ColumnClass column in finishColumn)
            column.HighLight = false;
    }
    #endregion

    #region EndTurn
    public void EndTurn(string side, float changeTurnDelay)
    {
        History.Clear();
        ClearColumnHighLight();

        GeneralOptions.DiceAnimConOpponent.EndShow();
        GeneralOptions.DiceAnimConPlayer.EndShow();

        if (side == "player")
        {
            Roll.playerTurn = false;
            Roll.playerAction = false;

            if (GeneralOptions.Online)
                RollDiceOpponent();

            ResetCheckers();
            GeneralOptions.DiceControllerPlayer.Show = false;
            StartCoroutine(SetTurn("opponent", changeTurnDelay));
        }
        else
        if (side == "opponent")
        {
            Roll.opTurn = false;
            Roll.opponentAction = false;

            ResetCheckers();
            GeneralOptions.DiceControllerOpponent.Show = false;
            StartCoroutine(SetTurn("player", changeTurnDelay));
        }

        Debug.Log("End " + side + " Turn");
    }
    #endregion

    #region Begin Setup
    public void BeginSetup()
    {
        //yield return new WaitForSeconds(delay * Time.deltaTime);
        Timing.playerFullTime = GeneralOptions.fullTime;
        Timing.opponentFullTime = GeneralOptions.fullTime;
        Starting = true;
        StartCoroutine(StartingToEnd(100));

        GameObject CheckersObj = new GameObject("Checkers");

        for (int i = 0; i < 30; i++)
        {
            GameObject checker = Instantiate(Checker, transform.position, Checker.transform.rotation);
            CheckerController CheckerCon = checker.GetComponent<CheckerController>();
            checker.transform.SetParent(CheckersObj.transform);
            CheckerCon.ID = i;
            //StartCoroutine(CheckerCon.SetLayerTime("OnTop", "Default", 100)); // itsnew

            /*if (GeneralOptions.Type == type.type1) CheckerCon.Type = CheckerController.type.type1;
            if (GeneralOptions.Type == type.type2) CheckerCon.Type = CheckerController.type.type2;
            if (GeneralOptions.Type == type.type3) CheckerCon.Type = CheckerController.type.type3;*/ // itsnew

            if (i < 15)
            {
                if (GeneralOptions.PlayerLocation == Location.Down)
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.Black;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.White;
                } else
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.White;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.Black;
                }

            }
            else
            {
                if (GeneralOptions.PlayerLocation == Location.Down)
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.White;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.Black;
                } else
                {
                    if (GeneralOptions.playerSide == sides.White)
                        CheckerCon.Side = CheckerController.side.Black;

                    if (GeneralOptions.playerSide == sides.Black)
                        CheckerCon.Side = CheckerController.side.White;
                }
            }

            if (i <= 4 || (i == 28) || (i == 29) || (i >= 13 && i <= 19))
                checker.transform.position = SpawnLocation.Right.position;
            else
                checker.transform.position = SpawnLocation.Left.position;


            switch (i)
            {
                case <= 1: PlaceChecker(CheckerCon,Column[0],false,Column[0], false); break;
                case <= 6: PlaceChecker(CheckerCon,Column[11],false,Column[0], false); break;
                case <= 9: PlaceChecker(CheckerCon,Column[16],false,Column[0], false); break;
                case <= 14: PlaceChecker(CheckerCon,Column[18],false,Column[0], false); break;
                case <= 16: PlaceChecker(CheckerCon,Column[23],false,Column[0], false); break;
                case <= 21: PlaceChecker(CheckerCon,Column[12],false,Column[0], false); break;
                case <= 24: PlaceChecker(CheckerCon,Column[7],false,Column[0], false); break;
                case <= 29: PlaceChecker(CheckerCon,Column[5],false,Column[0], false); break;
            }

            /*if (i < 15)
            {
                PlaceChecker(CheckerCon, Column[23], false, Column[0], false);
            }
            else
                PlaceChecker(CheckerCon, Column[0], false, Column[0], false);*/
                
            /*if (i < 15)
            {
                if (i < 12)
                    PlaceChecker(CheckerCon, Column[18], false, Column[0], false);
                else
                    PlaceChecker(CheckerCon, Column[22], false, Column[0], false);
            }
            else
            {
                if (i < 19)
                    PlaceChecker(CheckerCon, Column[1], false, Column[0], false);
                else
                if (i < 26)
                    PlaceChecker(CheckerCon, Column[5], false, Column[0], false);
                else
                if (i < 27)
                    PlaceChecker(CheckerCon, Column[11], false, Column[0], false);
                else
                if (i < 28)
                    PlaceChecker(CheckerCon, Column[13], false, Column[0], false);
                else
                    PlaceChecker(CheckerCon, Column[19], false, Column[0], false);
            }*/

            CheckerCon.canDrag = false;
            CheckerCon.Stand = false;
            Checkers = FindObjectsOfType<CheckerController>();
            StartCoroutine(startingCheckers(3));
        }
    }
    #endregion

    #region place Checker
    public void PlaceChecker(CheckerController checker, ColumnClass columnTarget, bool haveStart, ColumnClass columnStart, bool isHistory)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                if (haveStart)
                {
                    checker.FirstAudio = false;
                    checker.played = false;
                }

                /*if (columnTarget.ID >= 0 && columnTarget.ID <= 23)
                    StartCoroutine(checker.SetLayerTime("OnTop", "Default", 70));
                else
                    checker.SetLayer("Default");*/ // itsnew

                sides side = sides.White;
                bool kick = false;
                if (haveStart)
                {
                    for (int i = 0; i < columnStart.Place.Count; i++)
                    {
                        if (columnStart.Place[i].Checker == checker)
                        {
                            if (columnStart.Side != side) side = sides.Black;
                            columnStart.Place[i].Checker = null;
                            columnStart.Place[i].Full = false;
                            break;
                        }
                    }
                }

                for (int i = 0; i < columnTarget.Place.Count; i++)
                {
                    if (!columnTarget.Place[i].Full)
                    {
                        if (columnTarget.Place[i] != columnTarget.Place[0] && side != columnTarget.Side && columnTarget.Side != sides.none)
                        {
                            KickChecker(columnTarget.Place[i - 1], columnTarget);

                            checker.kickplayed = false;
                            kick = true;
                            i -= 1;
                        }

                        if (!isHistory)
                            checker.multipy = 1.2f;
                        else
                            checker.multipy = 4;

                        columnTarget.Place[i].Checker = checker;
                        checker.columnStart = columnTarget;
                        checker.FixTarget = columnTarget.Place[i].Pos;
                        checker.isDragging = false;
                        checker.canDrag = false;
                        //checker.canFix = true;
                        checker.Fixing = true;
                        columnTarget.Place[i].Full = true;
                        checker.SetLayerOrder("place");

                        if (kick)
                        {
                            StartCoroutine(setCheckerCanFix(checker));
                            kick = false;
                        }

                        return;
                    }

                    if (i == columnTarget.Place.Count - 1) Debug.Log(columnTarget + ": is Full !");
                }

            }
            catch (Exception error) { Debug.Log(error); }
        });

    }
    #endregion

    #region Starting checkers
    public IEnumerator startingCheckers(float delay)
    {
        int i = 29;
        while (true)
        {
            yield return new WaitForSeconds(delay * Time.deltaTime);

            if (i >= 0)
            {
                Checkers[i].multipy = 2;
                Checkers[i].canFix = true;
                setPos(i);
            }
            else
                break;

            i -= 1;
        }

        void setPos(int target)
        {
            if (target <= 4 || (target == 28) || (target == 29) || (target >= 13 && target <= 19))
                Checkers[target].transform.position = SpawnLocation.Right.position;
            else
                Checkers[target].transform.position = SpawnLocation.Left.position;
        }
    }
    #endregion

    #region Set starting Pos
    void setStartingPos()
    {
        float speed = 5;
        Vector3 spawnPosRight = SpawnLocation.Right.transform.position;
        Vector3 spawnPosLeft = SpawnLocation.Left.transform.position;
        spawnPosRight.z -= speed * Time.deltaTime;
        spawnPosLeft.z -= speed * Time.deltaTime;
        SpawnLocation.Right.transform.position = spawnPosRight;
        SpawnLocation.Left.transform.position = spawnPosLeft;
    }
    #endregion

    #region Set checker CanFix
    IEnumerator setCheckerCanFix(CheckerController checker)
    {
        yield return new WaitForSeconds(25 * Time.deltaTime);
        while (checker.distanceToFix > 1.1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        kickedChecker.multipy = 2.2f;
        kickedChecker.canFix = true;

        GameObject kickVFX = Instantiate(Vfx.Kick, Camera.main.WorldToScreenPoint(checker.transform.position), Quaternion.identity, canvas.transform);
        yield return new WaitForSeconds(50 * Time.deltaTime);
        Destroy(kickVFX);
    }
    #endregion

    #region Kick Checker
    void KickChecker(PlaceClass place,ColumnClass startColumn)
    {
        ColumnClass column = outColumn[0];
        if ((place.Checker.Side == CheckerController.side.Black && GeneralOptions.playerSide == sides.Black) || (place.Checker.Side == CheckerController.side.White && GeneralOptions.playerSide == sides.White))
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                column = outColumn[0];
            else
                column = outColumn[1];
        }
        else
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                column = outColumn[1];
            else
                column = outColumn[0];
        }

        for (int j = 0; j < column.Place.Count; j++)
        {
            if (!column.Place[j].Full)
            {
                column.Place[j].Checker = place.Checker;
                //column.Place[j].Checker.SetLayerTime("OnTop", "Default", 50); // itsnew
                place.Checker = null;
                place.Full = false;
                column.Place[j].Checker.columnStart = column;
                column.Place[j].Checker.FixTarget = column.Place[j].Pos;
                column.Place[j].Checker.isDragging = false;
                column.Place[j].Checker.canDrag = false;
                column.Place[j].Checker.canFix = false;
                column.Place[j].Checker.Fixing = true;
                column.Place[j].Full = true;
                kickedChecker = column.Place[j].Checker;

                //Add Kick information to History
                if (History.Count > 0)
                {
                    HistoryClass history = History[History.Count - 1];
                    history.haveKick = true;
                    history.checkerKick = column.Place[j].Checker;
                    history.startColumnKick = startColumn;
                    history.targetColumnKick = column;
                }

                return;
            }
        }
    }
    #endregion

    #region Reset Checkers
    public void ResetCheckers()
    {
        foreach (CheckerController checker in Checkers)
        {
            checker.columnTarget.Clear();
            checker.canDrag = false;
        }
    }
    #endregion

    #region Check Player Blocked
    void CheckPlayerBlocked()
    {
        if (CheckBlock() && Roll.playerTurn && !Roll.playerProcessing && Roll.playerAction && History.Count == 0)
        {
            StartCoroutine(SetBlockedUI("BLOCKED", "مسدود شد", "تم الحظر"));
            Roll.doneButton.playerDone();
            Debug.Log("Blocked !");
        }
    }
    #endregion

    #region Check Have Kick
    bool CheckHaveKick(ColumnClass outcolumn, sides side)
    {
        for (int i = 0; i < outcolumn.Place.Count; i++)
        {
            if (outcolumn.Place[i].Full)
                return true;
        }

        return false;
    }
    #endregion

    #region Reset Column HighLight
    public void ResetColumnHighLight()
    {
        foreach (ColumnClass column in Column)
            column.HighLight = false;

        foreach (ColumnClass column in finishColumn)
            column.HighLight = false;
    }
    #endregion

    #region Get Last Checker
    public CheckerController GetLastChecker(ColumnClass column, sides side)
    {
        if (column.FullCount > 0 && column.Side == side)
        {
            for (int i = 14; i >= 0; i--)
            {
                if (column.Place[i].Full)
                {
                    return column.Place[i].Checker;
                }
            }
        }

        return null;
    }
    #endregion

    #region Usable Checker
    public IEnumerator UsableCheckerDelay(float delay, Location location, sides side, List<int> roll)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        UsableChecker(location, side, roll);
    }

    void ResetBlockedRoll()
    {
        foreach(ColumnClass column in Column)
        {
            column.BlockedRoll = 0;
        }

        foreach (ColumnClass column in outColumn)
        {
            column.BlockedRoll = 0;
        }

        foreach (ColumnClass column in finishColumn)
        {
            column.BlockedRoll = 0;
        }
    }

    public void UsableChecker(Location location, sides side, List<int> roll)
    {
        ResetColumnHighLight();
        ResetCheckers();
        ResetBlockedRoll();

        // Find OutColumn
        ColumnClass outcolumn = outColumn[0];
        if (GeneralOptions.playerSide == side)
        {
            if (GeneralOptions.PlayerLocation == Location.Up)
                outcolumn = outColumn[1];
        } else
        {
            if (GeneralOptions.PlayerLocation == Location.Down)
                outcolumn = outColumn[1];
        }

        List<CheckerController> LastCheckers = new List<CheckerController>();

        if (!CheckHaveKick(outcolumn, side)) // NotHave Kick Checker
        {
            foreach(ColumnClass column in Column)
            {
                if (GetLastChecker(column, side) != null && !LastCheckers.Contains(GetLastChecker(column, side)))
                    LastCheckers.Add(GetLastChecker(column, side));
            }
        }
        else // Have Kick Checker
        {
            for (int i = 14; i >= 0; i--)
            {
                if (outcolumn.Place[i].Full && !LastCheckers.Contains(outcolumn.Place[i].Checker))
                    LastCheckers.Add(outcolumn.Place[i].Checker);
            }
        }

        ResetCheckers();
        CheckFinishColumn(side, roll);

        foreach (CheckerController checker in LastCheckers)
        {
            CheckMovableCloumns(checker, roll, side, location);
            if (GeneralOptions.playerSide == side && checker.columnTarget.Count > 0)
                checker.canDrag = true;
        }

        Roll.playerAction = GeneralOptions.playerSide == side;
        Roll.opponentAction = GeneralOptions.OpponentSide == side;

        // Fix
        bool haveUsable = false;
        for (int i = 0; i < Checkers.Length; i++)
            if (Checkers[i].canDrag) { haveUsable = true; break; }
        if (!haveUsable)
            if (Roll.playerTurn) playerDone(true);
    }
    #endregion

    #region Player Done
    public void playerDone(bool done)
    {
        Roll.doneButton.canDone = done;

        if (done)
            ClearColumnHighLight();
    }
    #endregion

    #region Check Movable Columns
    void CheckMovableCloumns(CheckerController checker, List<int> roll, sides side, Location location)
    {
        kickblock = false;

        int targetID;
        for (int n1 = 0; n1 < Roll.Rolls.Count; n1++)
        {
            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1];
            else
            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1];
            else break;

            bool columnCheck = ColumnCheck(targetID, side);
            if (!columnCheck)
            {
                if (targetID >= 0 && targetID <= 23)
                {
                    //Roll.Blocked.Add(roll[n1]);
                    checker.columnStart.BlockedRoll = roll[n1];
                }
            }
            

            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]))
            {
                kickblock = CheckKickBlock(targetID, side);
                if (kickblock)
                {
                    if (targetID >= 0 && targetID <= 23)
                    {
                        //Roll.Blocked.Add(roll[n1]);
                        checker.columnStart.BlockedRoll = roll[n1];
                    }
                }
                

                checker.columnTarget.Add(Column[targetID]);
                if (Roll.Rolls.Count > 1)
                {
                    for (int n2 = 0; n2 < Roll.Rolls.Count; n2++)
                    {
                        if (n2 != n1)
                        {
                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2];
                            else
                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2];
                            else break;

                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                            {
                                kickblock = CheckKickBlock(targetID, side);
                                checker.columnTarget.Add(Column[targetID]);

                                if (Roll.Rolls.Count > 2)
                                {
                                    for (int n3 = 0; n3 < Roll.Rolls.Count; n3++)
                                    {
                                        if (n3 != n1 && n3 != n2)
                                        {
                                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2] - roll[n3];
                                            else
                                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2] + roll[n3];
                                            else break;

                                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                                            {
                                                kickblock = CheckKickBlock(targetID, side);
                                                checker.columnTarget.Add(Column[targetID]);

                                                if (Roll.Rolls.Count > 3)
                                                {
                                                    for (int n4 = 0; n4 < Roll.Rolls.Count; n4++)
                                                    {
                                                        if (n4 != n1 && n4 != n2 && n4 != n3)
                                                        {
                                                            if (location == Location.Down) targetID = checker.columnStart.ID - roll[n1] - roll[n2] - roll[n3] - roll[n4];
                                                            else
                                                            if (location == Location.Up) targetID = checker.columnStart.ID + roll[n1] + roll[n2] + roll[n3] + roll[n4];
                                                            else break;

                                                            if (ColumnCheck(targetID, side) && !checker.columnTarget.Contains(Column[targetID]) && !kickblock)
                                                            {
                                                                checker.columnTarget.Add(Column[targetID]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    bool CheckKickBlock(int targetID, sides side)
    {
        if (Column[targetID].Side != side && Column[targetID].Side != sides.none)
            return true;
        else
            return false;
    }

    bool ColumnCheck(int targetID, sides side)
    {
        bool maxCheck = targetID >= 0 && targetID < Column.Count;
        if (maxCheck)
        {
            if (Column[targetID].Side == side || Column[targetID].FullCount <= 1)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    #endregion

    #region Check Finish Column
    void CheckFinishColumn(sides side, List<int> roll)
    {
        // Player Home
        bool playerHome = GeneralOptions.playerSide == side && Roll.playerHome;
        bool playerDownHome = false;
        bool playerUpHome = false;

        if (playerHome)
        {
            playerDownHome = GeneralOptions.PlayerLocation == Location.Down;
            playerUpHome = GeneralOptions.PlayerLocation == Location.Up;
        }

        bool playerCanHome = playerDownHome || playerUpHome;

        // Opponent Home
        bool opponentHome = GeneralOptions.OpponentSide == side && Roll.opponentHome;
        bool opponentDownHome = false;
        bool opponentUpHome = false;

        if (opponentHome)
        {
            opponentDownHome = GeneralOptions.OpponentLocation == Location.Down;
            opponentUpHome = GeneralOptions.OpponentLocation == Location.Up;
        }

        bool Down = playerDownHome || opponentDownHome;
        bool Up = playerUpHome || opponentUpHome;
        bool opponentCanHome = opponentDownHome || opponentUpHome;
        bool Home = playerCanHome || opponentCanHome;

        ColumnClass finishColumnDown = finishColumn[1];
        ColumnClass finishColumnUp = finishColumn[0];

        if (Home)
        {
            List<ColumnClass> homeColumn = new List<ColumnClass>();
            // Home Columns
            if (Down)
            {
                for (int i = 1; i <= 6; i++)
                    homeColumn.Add(Column[i - 1]);
            }
            else
            if (Up)
            {
                for (int i = 1; i <= 6; i++)
                    homeColumn.Add(Column[24 - i]);
            }

            // Last Checkers of Home Columns
            List<CheckerController> homeLastCheckers = new List<CheckerController>();
            CheckerController lastChecker = null;

            for (int j = 0; j < homeColumn.Count; j++)
            {
                lastChecker = GetLastchecker(homeColumn[j], side);

                if (lastChecker != null && !homeLastCheckers.Contains(lastChecker))
                {
                    homeLastCheckers.Add(lastChecker);
                    lastChecker = null;
                }
            }

            // check for Add FinishColumn to Checker
            if (homeLastCheckers.Count > 0)
            {
                foreach (CheckerController checker in homeLastCheckers)
                {
                    int targetID = checker.columnStart.ID;
                    if (Up) targetID = Mathf.Abs(checker.columnStart.ID - 23);

                    for (int n1 = 0; n1 < roll.Count; n1++)
                    {
                        
                        if (targetID == (roll[n1] - 1))
                        {
                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                checker.columnTarget.Add(finishColumnDown);
                            else
                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                checker.columnTarget.Add(finishColumnUp);
                        }
                        else
                        {
                            if (Roll.Rolls.Count > 1)
                            {
                                for (int n2 = 0; n2 < roll.Count; n2++)
                                {
                                    if (n1 != n2)
                                    {
                                        if (targetID == (roll[n1] + roll[n2]) - 1)
                                        {
                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                checker.columnTarget.Add(finishColumnDown);
                                            else
                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                checker.columnTarget.Add(finishColumnUp);
                                        }
                                        else
                                        {
                                            if (Roll.Rolls.Count > 2)
                                            {
                                                for (int n3 = 0; n3 < roll.Count; n3++)
                                                {
                                                    if (n3 != n1 && n3 != n2)
                                                    {
                                                        if (targetID == (roll[n1] + roll[n2] + roll[n3]) - 1)
                                                        {
                                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                                checker.columnTarget.Add(finishColumnDown);
                                                            else
                                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                                checker.columnTarget.Add(finishColumnUp);
                                                        }
                                                        else
                                                        {
                                                            if (Roll.Rolls.Count > 3)
                                                            {
                                                                for (int n4 = 0; n4 < roll.Count; n4++)
                                                                {
                                                                    if (n4 != n1 && n4 != n2 && n4 != n3)
                                                                    {
                                                                        if (targetID == (roll[n1] + roll[n2] + roll[n3] + roll[n4]) - 1)
                                                                        {
                                                                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                                                                checker.columnTarget.Add(finishColumnDown);
                                                                            else
                                                                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                                                                checker.columnTarget.Add(finishColumnUp);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int maxRoll = FindMaxRoll(roll);
            bool empity = true;

            for (int k = 0; k < homeColumn.Count; k++)
            {
                if ((homeColumn[k].ID >= maxRoll - 1 && Down) || (Mathf.Abs(homeColumn[k].ID - 23) >= maxRoll - 1 && Up))
                {
                    if (homeColumn[k].FullCount > 0 && homeColumn[k].Side == side)
                        empity = false;
                }
            }

            if (empity)
            {
                for (int l = 2; l <= 6; l++)
                {
                    if (maxRoll - l >= 0)
                    {
                        if (homeColumn[maxRoll - l].FullCount > 0 && homeColumn[maxRoll - l].Side == side)
                        {
                            CheckerController checker = GetLastchecker(homeColumn[maxRoll - l], side);

                            if (Down && !checker.columnTarget.Contains(finishColumnDown))
                                checker.columnTarget.Add(finishColumnDown);
                            else
                            if (Up && !checker.columnTarget.Contains(finishColumnUp))
                                checker.columnTarget.Add(finishColumnUp);

                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Find Max Roll
    public int FindMaxRoll(List<int> roll)
    {
        int MaxRoll = 0;

        MaxRoll = roll[0];

        if (roll.Count > 1)
        {
            if (roll[1] > MaxRoll)
                MaxRoll = roll[1];

            if (roll.Count > 2)
            {
                if (roll[2] > MaxRoll)
                    MaxRoll = roll[2];

                if (roll.Count > 3)
                {
                    if (roll[3] > MaxRoll)
                        MaxRoll = roll[3];
                }
            }
        }

        return MaxRoll;
    }
    #endregion

    #region Get Last Checker
    CheckerController GetLastchecker(ColumnClass column, sides side)
    {
        CheckerController lastChecker = null;
        if (column.FullCount > 0 && column.Side == side)
        {
            for (int i = 14; i >= 0; i--)
            {
                if (column.Place[i].Full && lastChecker == null)
                    lastChecker = column.Place[i].Checker;
            }
        }

        return lastChecker;
    }
    #endregion

    #region Draw Gizmos
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (GeneralOptions.showEditorPlace || GeneralOptions.showEditorColumn)
        {
            foreach(ColumnClass column in Column)
            {   
                if (GeneralOptions.showEditorPlace)
                {
                    foreach(PlaceClass palce in column.Place)
                    {
                        if (!palce.Full) Gizmos.color = new Color(1,0,0,0.8f); else Gizmos.color = new Color(0,1,0,0.8f);
                        Gizmos.DrawSphere(palce.Pos, 0.15f);
                    } 
                }

                if (GeneralOptions.showEditorColumn)
                {
                    Gizmos.color = new Color(0,0,1,0.3f);
                    Gizmos.DrawCube(column.Pos, GeneralOptions.rangeSizeColumn);
                }
            }
        }

        if (GeneralOptions.showEditorPlace)
        {
            foreach(ColumnClass column in outColumn)
            {
                foreach(PlaceClass palce in column.Place)
                {
                    if (!palce.Full) Gizmos.color = new Color(1,0,0,0.8f); else Gizmos.color = new Color(0,1,0,0.8f);
                    Gizmos.DrawSphere(palce.Pos, 0.15f);
                } 
            }

            foreach (ColumnClass column in finishColumn)
            {
                foreach (PlaceClass palce in column.Place)
                {
                    if (!palce.Full) Gizmos.color = new Color(1, 0, 0, 0.8f); else Gizmos.color = new Color(0, 1, 0, 0.8f);
                    Gizmos.DrawSphere(palce.Pos, 0.15f);
                }

                if (GeneralOptions.showEditorColumn)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.3f);
                    Gizmos.DrawCube(column.Pos + GeneralOptions.rangeDistance, GeneralOptions.rangeSizeFinishColumn);
                }
            }
        }
        
    }
#endif
    #endregion

    #region Setup Columns
    void SetupColumns()
    {
        GameObject ColumnObjs = new GameObject("Column Objects");
        GameObject ColumnHighLights = new GameObject("Column HighLights");
        ColumnHighLights.transform.SetParent(ColumnObjs.transform);

        ColumnOptionsClass ColumnOption = Options.ColumnOption;

        for (int i=0;i<24;i++)
        {
            Column.Add(new ColumnClass());
            Column[i].ID = i;
            Column[i].Side = sides.none;

            // Set Direction
            if (i < 12)
                Column[i].Direction = direction.Up;
            else
                Column[i].Direction = direction.Down;


            Transform startSetup = Options.startSetup;

            // Set Position
            if (i == 0)
                Column[i].Pos = startSetup.position;
            else
            {
                Column[i].Pos = Column[i-1].Pos;

                if (i < 12 && i != 6)
                    Column[i].Pos.x -= ColumnOption.distanceX;
                else
                if (i == 6)
                    Column[i].Pos.x -= ColumnOption.middleDistance;
                else
                if (i == 12)
                    Column[i].Pos.y += ColumnOption.distanceZ; // itsnew
                else
                if (i > 12 && i != 18)
                    Column[i].Pos.x += ColumnOption.distanceX;
                else
                if (i == 18)
                    Column[i].Pos.x += ColumnOption.middleDistance;
            }

            float dis = -0.3f;
            UnityEngine.Quaternion rot = GeneralOptions.HighLightPrefab.transform.rotation;
            if (i >= 12)
            {
                rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 180);
                dis = Mathf.Abs(dis);
            }

            GameObject highLight = Column[i].HighLightObject = Instantiate(GeneralOptions.HighLightPrefab,new Vector3(Column[i].Pos.x,Column[i].Pos.y,Column[i].Pos.z+dis),rot);
            highLight.transform.SetParent(ColumnHighLights.transform);
        }
    }
    #endregion

    #region Setup Places
    void SetupPlaces()
    {
        foreach(ColumnClass column in Column)
            SetupPlacesMain(column);

        foreach(ColumnClass column in outColumn)
            SetupPlacesMain(column);

        foreach (ColumnClass column in finishColumn)
            SetupPlacesFinish(column);
    }

    void SetupPlacesMain(ColumnClass column)
    {
        PlaceOptionsClass PlaceOptions = Options.PlaceOption;

        column.maxCount = 5;

        for (int i=0;i<15;i++)
        {
            column.Place.Add(new PlaceClass());
            column.Place[i].Full = false;

            // Set Position
            if (i == 0)
                column.Place[i].Pos = column.Pos;
            else
            {
                // Set Row
                if (column.Place.Count == column.maxCount+1)
                {
                    switch(column.maxCount)
                    {
                        case 5: SetupNewRow(column,0, PlaceOptions); column.maxCount = 9; break;
                        case 9: SetupNewRow(column,5, PlaceOptions); column.maxCount = 12; break;
                        case 12: SetupNewRow(column,9, PlaceOptions); column.maxCount = 14; break;
                        case 14: SetupNewRow(column,12, PlaceOptions); column.maxCount = 15; break;
                    }
                }
                else
                {
                    switch(column.Direction)
                    {
                        case direction.Up: column.Pos.y += PlaceOptions.distanceZ;  break; // itsnew
                        case direction.Down: column.Pos.y -= PlaceOptions.distanceZ;  break; // itsnew
                    }
                }

                column.Place[i].Pos = column.Pos;
            }
        }
    }

    void SetupPlacesFinish(ColumnClass column)
    {
        for (int i = 0; i < 15; i++)
        {
            column.Place.Add(new PlaceClass());
            column.Place[i].Full = false;

            if (i == 0)
                column.Place[i].Pos = column.Pos;
            else
                column.Pos.y += finishColumnOption.RowDistance; // itsnew

            column.Place[i].Pos = column.Pos;
        }

    }

    void SetupNewRow(ColumnClass column,int startedPlaceID, PlaceOptionsClass PlaceOptions)
    {
        float RowDistance = Vector3.Distance(column.Place[startedPlaceID].Pos,column.Place[startedPlaceID+1].Pos)/2;
        column.Pos = column.Place[startedPlaceID].Pos;

        switch(column.Direction)
        {
            case direction.Up: column.Pos.y += RowDistance;  break; // itsnew
            case direction.Down: column.Pos.y -= RowDistance;  break; // itsnew
        }

        column.Pos.z -= PlaceOptions.distanceY; // itsnew
    }
    #endregion

    #region show LOG
    /*void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (textLog != null)
        {
            textLog.text += "\n" + logString;

            if (textLog.text.Length > 480)
                textLog.text = "<color=red> ---------------- CONSOLE: ----------------</color>";
        }
    }*/
    #endregion
}