using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoubleController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    [Header("General")]
    public int Multiple;
    public GameSetup.sides Side;
    public GameSetup.sides oldSide;
    public bool canDouble;

    [Header("Request")]
    public bool myRequest;
    public bool Requesting;
    public bool RequestedMe;
    public CanvasGroup DoubleRequestUI;
    public float RequestFullTime;
    public float RequestTime;
    public Image FillImage;
    public GameObject LoadingIcon;
    public Image DoubleButtonUI;
    public Text MultipleInfoText;

    [Header("Response")]
    public CanvasGroup DoubleResponseUI;
    public GameObject WaitBarUI;
    public GameObject AcceptUI;
    public GameObject ResignUI;
    public Image BarUI;
    float FillBar;
    bool resigned;

    GameSetup GameSetup;
    Text DoubleTextUI;
    RollButton RollButton;
    Animator anim;
    PlayerInfo playerInfo;
    PauseController pauseController;
    FinishController FinishCon;
    

    [Header("Rots")]
    public Quaternion Rot2;
    public Quaternion Rot4;
    public Quaternion Rot8;
    public Quaternion Rot16;
    public Quaternion Rot32;
    public Quaternion Rot64;

    public Quaternion TargetRot;

    public bool doubleOfferProccessing;
    public bool surrenderProccessing;
    public bool doubleAcceptProccessing;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        RollButton = FindObjectOfType<RollButton>();
        anim = GetComponent<Animator>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        pauseController = FindObjectOfType<PauseController>();
        FinishCon = FindObjectOfType<FinishController>();

        socketManager = SocketManager.Instance;
        Multiple = 1;
        TargetRot = Rot64;
        Side = GameSetup.sides.none;
        oldSide = Side;
    }

    void Update()
    {
        if (GameSetup.Updated)
        {
            canDouble = Multiple < 64 && RollButton.canRoll && (Side == GameSetup.sides.none || Side == GameSetup.GeneralOptions.playerSide) && !Requesting;
            DoubleButtonUI.enabled = canDouble;
        }

        /*if (playerInfo.Updated)
            anim.SetInteger("level", playerInfo.PlayerData.Level);*/  // itsnew

        float Speed = 2 * Time.deltaTime;
        //transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, TargetRot, Speed); // itsnew

        float speed = 10 * Time.deltaTime;
        if (RequestedMe)
        {
            DoubleRequestUI.gameObject.SetActive(true);

            if (DoubleRequestUI.alpha < 1)
                DoubleRequestUI.alpha += speed;

            if (DoubleRequestUI.alpha >= 1)
                DoubleRequestUI.alpha = 1;

            RequestTime -= Time.deltaTime;
            FillImage.fillAmount = GameSetup.LineRange(RequestFullTime, RequestTime);

            if (RequestTime <= 0)
                StartCoroutine(EndRequest(false, Multiple,GameSetup.GeneralOptions.OpponentSide));

            float rotSpeed = 150 * Time.deltaTime;
            //Loading Icon
            Quaternion rot = LoadingIcon.transform.rotation;
            Vector3 eulerRot = rot.eulerAngles;
            eulerRot.z -= rotSpeed;
            rot = Quaternion.Euler(eulerRot);
            LoadingIcon.transform.rotation = rot;

            //Information: Multiple
            switch(Multiple)
            {
                case 1: MultipleInfoText.text = "X2"; break;
                case 2: MultipleInfoText.text = "X4"; break;
                case 4: MultipleInfoText.text = "X8"; break;
                case 8: MultipleInfoText.text = "X16"; break;
                case 16: MultipleInfoText.text = "X32"; break;
            }
        }
        else
        {
            if (DoubleRequestUI.alpha > 0)
                DoubleRequestUI.alpha -= speed;

            if (DoubleRequestUI.alpha <= 0)
            {
                DoubleRequestUI.alpha = 0;
                DoubleRequestUI.gameObject.SetActive(false);
            }

            RequestTime = RequestFullTime;
        }

        setAlphaMyRequest();
    }

    void SetSide()
    {
        if (oldSide == GameSetup.sides.none)
        {
            if (Side == GameSetup.GeneralOptions.playerSide)
            {
                if (GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down)
                    anim.SetTrigger("StartedDown");
                else
                    anim.SetTrigger("StartedUp");
            }
            else
            {
                if (GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Down)
                    anim.SetTrigger("StartedDown");
                else
                    anim.SetTrigger("StartedUp");
            }
        }
        else
        if (oldSide == GameSetup.GeneralOptions.playerSide && Side == GameSetup.GeneralOptions.OpponentSide)
        {
            if (GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Down)
                anim.SetTrigger("UpToDown");
            else
                anim.SetTrigger("DownToUp");
        }
        else 
        if (oldSide == GameSetup.GeneralOptions.OpponentSide && Side == GameSetup.GeneralOptions.playerSide)
        {
            if (GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down)
                anim.SetTrigger("UpToDown");
            else
                anim.SetTrigger("DownToUp");
        }

        oldSide = Side;
    }

    void setAlphaMyRequest()
    {
        float speed = 10 * Time.deltaTime;

        if (Requesting && myRequest)
        {
            BarUI.fillAmount = GameSetup.LineRange(GameSetup.GeneralOptions.turnTime, FillBar);
            FillBar -= Time.deltaTime;

            DoubleResponseUI.gameObject.SetActive(true);

            if (DoubleResponseUI.alpha < 1)
                DoubleResponseUI.alpha += speed;

            if (DoubleResponseUI.alpha >= 1)
                DoubleResponseUI.alpha = 1;

            // Time Out (Double Response)
            if (FillBar <= 0)
            {
                FillBar = 0;

                /*if (!resigned)
                {
                    socketManager.socket.Emit("Surrender");
                    Debug.Log("<color=yellow> Surrender </color> <color=white> Processing... </color>");
                    resigned = true;

                    surrenderProccessing = true;
                    StopCoroutine(EmitProccess("Surrender"));
                    StartCoroutine(EmitProccess("Surrender"));
                }*/
            }

        }else
        {
            if (DoubleResponseUI.alpha > 0)
                DoubleResponseUI.alpha -= speed;

            if (DoubleResponseUI.alpha <= 0)
            {
                DoubleResponseUI.alpha = 0;
                DoubleResponseUI.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator EmitProccess(string emit)
    {
        yield return new WaitForSeconds(GameSetup.GeneralOptions.EmitTryTime * Time.deltaTime);
        if (emit == "Surrender" && surrenderProccessing)
        {
            socketManager.socket.Emit("Surrender");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> Surrender </color> <color=white> Processing... </color>");
        }
        if (emit == "DoubleOffer" && surrenderProccessing)
        {
            socketManager.socket.Emit("DoubleOffer");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> DoubleOffer </color> <color=white> Processing... </color>");
        }
        if (emit == "AcceptDouble" && surrenderProccessing)
        {
            socketManager.socket.Emit("AcceptDouble");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> AcceptDouble </color> <color=white> Processing... </color>");
        }
    }

    public void DoubleButtonClick()
    {
        if (canDouble && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI))
        {
            RequestDouble(GameSetup.GeneralOptions.OpponentSide);
            canDouble = false;
        }
    }

    public void RequestDouble(GameSetup.sides side)
    {
        Requesting = true;
        GameSetup.Audio.DoubleRequest.Play();

        if (side == GameSetup.GeneralOptions.playerSide)
        {
            myRequest = false;
            RequestedMe = true;
        }
        else
        {
            myRequest = true;
            FillBar = GameSetup.GeneralOptions.turnTime;
            resigned = false;

            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                bool response = false;
                if (GameSetup.finishCount.playerCount < GameSetup.finishCount.opponentCount)
                    response = true;
                else
                {

                    int homeCountPlayer = 0;
                    int homeCountOpponent = 0;
                    foreach (CheckerController checker in GameSetup.Checkers)
                    {
                        if (checker.ID >= 15 && checker.columnStart.ID < 6)
                            homeCountPlayer += 1;

                        if (checker.ID < 15 && checker.columnStart.ID >= 18)
                            homeCountOpponent += 1;
                    }
                    
                    if ((homeCountOpponent >= homeCountPlayer) || (homeCountOpponent+homeCountPlayer < 30))
                        response = true;
                }

                StartCoroutine(EndRequest(response, Multiple, GameSetup.GeneralOptions.OpponentSide));
            }
            else
            // Online
            if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                socketManager.socket.Emit("DoubleOffer");

                WaitBarUI.SetActive(true);
                AcceptUI.SetActive(false);
                ResignUI.SetActive(false);

                Debug.Log("<color=yellow> DoubleOffer </color> <color=white>  Processing... </color>");

                doubleOfferProccessing = true;
                StopCoroutine(EmitProccess("DoubleOffer"));
                StartCoroutine(EmitProccess("DoubleOffer"));
            }
        }
    }
    public void Response(bool accept)
    {
        GameSetup.Audio.Click.Play();

        if (accept)
        {
            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                StartCoroutine(EndRequest(true, Multiple,GameSetup.GeneralOptions.OpponentSide));
            }
            else
            // Online
            if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                socketManager.socket.Emit("AcceptDouble");
                Debug.Log("<color=yellow> AcceptDouble </color> <color=white>  Processing... </color>");

                doubleAcceptProccessing = true;
                StopCoroutine(EmitProccess("AcceptDouble"));
                StartCoroutine(EmitProccess("AcceptDouble"));
            }
        }
        else
        {
            StartCoroutine(EndRequest(false, Multiple,GameSetup.GeneralOptions.playerSide));
        }
    }

    public IEnumerator EndRequest(bool accept, int LastDouble, GameSetup.sides side)
    {
        WaitBarUI.SetActive(false);
        AcceptUI.SetActive(accept);
        ResignUI.SetActive(!accept);
        
        if (RequestedMe)
            yield return new WaitForSeconds(Time.deltaTime);
        else
            yield return new WaitForSeconds(100 * Time.deltaTime);

        if (accept)
        {
            Multiple *= 2;

            // Online
            if (Multiple != LastDouble && GameSetup.GeneralOptions.Online)
            {
                Multiple = LastDouble;
                Debug.Log("<color=red> DoubleOffer </color> <color=white> Wrong Double Corrected: </color>" + Multiple);
            }

            SetMultipleRot();

            if (RequestedMe)
            {
                Side = GameSetup.GeneralOptions.playerSide;

                // Local
                if (GameSetup.GeneralOptions.AI)
                {
                    float delay2 = 1;
                    if (GameSetup.StartingRollDice) delay2 = 30;
                    GameSetup.StartingRollDice = false;

                    StartCoroutine(GameSetup.GeneralOptions.DiceAnimConOpponent.RollDice(delay2,false));
                }
            }
            else
            {
                Side = GameSetup.GeneralOptions.OpponentSide;
            }

            SetSide();

        }else
        {
            // Online
            if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                socketManager.socket.Emit("Surrender");
                Debug.Log("<color=yellow> Surrender </color> <color=white> Processing... </color>");

                surrenderProccessing = true;
                StopCoroutine(EmitProccess("Surrender"));
                StartCoroutine(EmitProccess("Surrender"));
            }
            else
            // Local
            {
                FinishCon.playerWin = (side == GameSetup.GeneralOptions.OpponentSide);
                FinishCon.Finished = true;
            }
        }

        myRequest = false;
        RequestedMe = false;
        Requesting = false;
    }

    public void SetMultipleRot()
    {
        switch (Multiple)
        {
            case 2: TargetRot = Rot2; break;
            case 4: TargetRot = Rot4; break;
            case 8: TargetRot = Rot8; break;
            case 16: TargetRot = Rot16; break;
            case 32: TargetRot = Rot32; break;
            case 64: TargetRot = Rot64; break;
        }
    }
}
