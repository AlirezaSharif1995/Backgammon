using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    PopUpController PopUpcontroller;
    GameSetup GameSetup;
    FinishController FinishCon;
    CanvasGroup CanvasGroup;

    public PopUpController.ButtonClass YesButton;
    public PopUpController.ButtonClass NoButton;
    public string[] message;

    public bool surrenderProccessing;

    private void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        PopUpcontroller = FindObjectOfType<PopUpController>();
        FinishCon = FindObjectOfType<FinishController>();
        CanvasGroup = GetComponent<CanvasGroup>();

        socketManager = SocketManager.Instance;
        CanvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (GameSetup.AllReady)
        {
            if (CanvasGroup.alpha < 1)
                CanvasGroup.alpha += 10 * Time.deltaTime;

            if (CanvasGroup.alpha >= 1)
                CanvasGroup.alpha = 1;
        }
    }

    public void ButtonClick()
    {
        if (GameSetup.AllReady)
        {
            string Me = message[0];
            PopUpcontroller.ShowPopUpTwo(false, Me, true, YesButton, Yes, true, NoButton, No);
        }
    }

    public void Yes()
    {
        if (GameSetup.AllReady)
        {
            // Local
            if (GameSetup.GeneralOptions.AI)
            {
                FinishCon.Finished = true;
            }
            // Online
            else if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                socketManager.socket.Emit("Surrender");
                Debug.Log("<color=yellow> Surrender: </color> <color=white> Processing... </color>");

                surrenderProccessing = true;
                StopCoroutine(EmitProccess("Surrender"));
                StartCoroutine(EmitProccess("Surrender"));
            }

            PopUpcontroller.Show = false;
        }
    }


    public void No()
    {
        if (GameSetup.AllReady)
        {
            PopUpcontroller.Show = false;
        }
    }

    public IEnumerator EmitProccess(string emit)
    {
        yield return new WaitForSeconds(GameSetup.GeneralOptions.EmitTryTime * Time.deltaTime);
        if (emit == "Surrender" && surrenderProccessing)
        {
            socketManager.socket.Emit("Surrender");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> Surrender: </color> <color=white>" + "  Processing... " + "</color>");
        }
    }

}
