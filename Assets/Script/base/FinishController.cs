using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FinishController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    [Header("General")]
    public bool Finished;
    public bool LastFinish;
    public bool showFinish;
    public int opponentAvatarID;
    float timeToExit;
    public string mainMenuScene;
    public bool Updated;
    public float timeToExitDefault;
    bool delayFinish;
    public bool playerWin; // Local
    public bool lastFinish_Opponent; // Local

    [Header("Access")]
    public CanvasGroup FinishUI;
    GameSetup GameSetup;
    PlayerInfo playerInfo;
    public GameObject WinUI;
    public GameObject LoseUI;
    /*public AvatarScript PlayerAvatar;
    public AvatarScript OpponentAvatar;*/
    public Animator animCamera;
    public Text ReturnTimerUI;
    public Image BlackScreenUI;
    DoubleController DoubleController;
    public CanvasGroup mainCanvasUI;
    public ExitButton ExitButton;

    public UnityEngine.UI.Text playerNickname;
    public UnityEngine.UI.Text opponentNickname;
    public UnityEngine.UI.Text priceCount;

    [Header("Text")]
    public UnityEngine.UI.Text YouWinText;
    public UnityEngine.UI.Text YouLostText;
    public UnityEngine.UI.Text playerWinnerText;
    public UnityEngine.UI.Text opponentWinnerText;
    public UnityEngine.UI.Text BackToMenuText;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        DoubleController = FindObjectOfType<DoubleController>();

        socketManager = SocketManager.Instance;
        timeToExit = timeToExitDefault;
    }

    void Update()
    {
        if (!Updated && playerInfo.Updated && GameSetup.Updated)
        {
            //playerNickname.text = playerInfo.PlayerData.username;
            opponentNickname.text = GameSetup.opponentNickname;
            opponentAvatarID = GameSetup.opponentAvatarID;

            CheckForFinishOnline();

            Updated = true;
        }

        if (Updated)
        {
            if (GameSetup.finishColumn[1].FullCount == 15 && !LastFinish)
            {
                Destroy(GameSetup.finishColumn[1].Place[14].Checker.gameObject);
                LastFinish = true;
            }

            lastFinish_Opponent = GameSetup.finishColumn[0].FullCount == 15;
  
            if (GameSetup.GeneralOptions.AI) CheckFinishLocal();

            SetAlpha();

            /*animCamera.SetBool("Finish", LastFinish);
            animCamera.SetBool("delayFinish", delayFinish);*/

            if (showFinish)
            {
                timeToExit -= Time.deltaTime;

                if (timeToExit <=0)
                {
                    timeToExit = 0;
                    SceneManager.LoadScene(mainMenuScene);
                }
            }

            if (delayFinish && !showFinish)
            {
                BlackScreenUI.enabled = true;

                Color color = BlackScreenUI.color;
                if (color.a < 0.8f)
                    color.a += Time.deltaTime;
                BlackScreenUI.color = color;
            }

            if (showFinish)
            {
                Color color = BlackScreenUI.color;
                if (color.a > 0)
                    color.a -= 2 * Time.deltaTime;
                BlackScreenUI.color = color;
            }

            if (DoubleController.Requesting && Finished)
            {
                DoubleController.Response(false);
            }

            if (Finished)
            {
                if (mainCanvasUI.alpha > 0)
                    mainCanvasUI.alpha -= 10 * Time.deltaTime;

                if (mainCanvasUI.alpha <= 0)
                    mainCanvasUI.alpha = 0;
            }else
            {
                mainCanvasUI.alpha = 1;
            }
        }
    }

    void CheckForFinishOnline()
    {
        if (GameSetup.GeneralOptions.Online && socketManager.socket.Connected)
        {
            socketManager.socket.On("Finish", (response) =>
            {
                var result = JsonConvert.DeserializeObject<List<SocketManager.FinishData>>(response.ToString());

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    DoubleController.surrenderProccessing = false;
                    StopCoroutine(DoubleController.EmitProccess("Surrender"));

                    ExitButton.surrenderProccessing = false;
                    StopCoroutine(ExitButton.EmitProccess("Surrender"));

                    Finished = true;

                    if (result[0].winnerPlayer == playerInfo.PlayerData.token)
                    {
                        /*PlayerAvatar.Winner = true;
                        OpponentAvatar.Winner = false;*/

                        WinUI.SetActive(true);
                        LoseUI.SetActive(false);

                        priceCount.text = "+ ";
                        priceCount.text += result[0].winPrize;
                        print(result[0].winPrize);
                        if (GameSetup.finishColumn[1].FullCount != 15) delayFinish = true;

                        Debug.Log("<color=green> Finish </color> <color=white> Win </color>");

                    }
                    else if (!delayFinish)
                    {
                        /*PlayerAvatar.Winner = false;
                        OpponentAvatar.Winner = true;*/

                        WinUI.SetActive(false);
                        LoseUI.SetActive(true);

                        priceCount.text = "- ";
                        priceCount.text += result[0].winPrize;

                        delayFinish = true;

                        Debug.Log("<color=green> Finish </color> <color=white> Lose </color>");
                    }
                });

                if (GameSetup.DeveloperMode)
                    Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> Finish:  </color> <color=white>" + response.ToString() + "</color>");
            });
        }
    } 

    void CheckFinishLocal()
    {
        if (Finished && !LastFinish && !delayFinish)
        {
            /*PlayerAvatar.Winner = playerWin;
            OpponentAvatar.Winner = !playerWin;*/

            WinUI.SetActive(playerWin);
            LoseUI.SetActive(!playerWin);

            if (playerWin)
                priceCount.text = "+ 0";
            else
                priceCount.text = "- 0";

            delayFinish = true;

            if (playerWin)
                Debug.Log("Finish: Win");
            else
                Debug.Log("Finish: Lose");
        }

        if (LastFinish || lastFinish_Opponent)
        {
            Finished = true;

            if ((GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down && GameSetup.finishColumn[1].FullCount == 15) || (GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Up && GameSetup.finishColumn[0].FullCount == 15))
            {
                /*PlayerAvatar.Winner = true;
                OpponentAvatar.Winner = false;*/

                WinUI.SetActive(true);
                LoseUI.SetActive(false);

                priceCount.text = "+ 0";
                priceCount.text += "0";

                Debug.Log("Finish: Win");
            }
            else if (!delayFinish)
            {
                /*PlayerAvatar.Winner = false;
                OpponentAvatar.Winner = true;*/

                WinUI.SetActive(false);
                LoseUI.SetActive(true);

                priceCount.text = "- 0";

                delayFinish = true;
                Debug.Log("Finish: Lose");
            }
        }
    }

    void SetAlpha()
    {
        float speed = 5 * Time.deltaTime;

        if (showFinish)
        {
            FinishUI.gameObject.SetActive(true);

            if (FinishUI.alpha < 1)
                FinishUI.alpha += speed;

            if (FinishUI.alpha >= 1)
                FinishUI.alpha = 1;

        }else
        {
            if (FinishUI.alpha > 0)
                FinishUI.alpha -= speed;

            if (FinishUI.alpha <= 0)
            {
                FinishUI.alpha = 0;
                FinishUI.gameObject.SetActive(false);
            }
        }
    }

    /*public void BackButtonClick()
    {
        if (showFinish)
            Debug.Log("Back");
    }*/
}
