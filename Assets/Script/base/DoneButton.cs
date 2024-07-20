using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoneButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance; 

    GameSetup GameSetup;
    PauseController pauseController;

    public bool canDone;
    Image image;

    public bool submitProccessing;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        image = transform.GetComponent<Image>();
        pauseController = FindObjectOfType<PauseController>();

        socketManager = SocketManager.Instance;
        canDone = false;
    }

    private void Update()
    {
        image.enabled = canDone;
    }

    public void ButtonClick()
    {
        if (canDone && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI) && !GameSetup.moveVerifying)
        {
            GameSetup.Audio.Click.Play();
            playerDone();
            canDone = false;
        } 
    }

    public void playerDone()
    {
        // Local
        if (GameSetup.GeneralOptions.AI)
        {
            GameSetup.EndTurn("player", GameSetup.GeneralOptions.delayTurn);
        }
        else
        // Online
        if (GameSetup.GeneralOptions.Online && socketManager.socket.Connected)
        {
            socketManager.socket.Emit("Submit");
            Debug.Log("<color=yellow> Submit </color> <color=white>  Processing... </color>");

            submitProccessing = true;
            StopCoroutine(EmitProccess("Submit"));
            StartCoroutine(EmitProccess("Submit"));
        }

        canDone = false;
    }

    public IEnumerator EmitProccess(string emit)
    {
        yield return new WaitForSeconds(GameSetup.GeneralOptions.EmitTryTime * Time.deltaTime);
        if (emit == "Submit" && submitProccessing)
        {
            socketManager.socket.Emit("Submit");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> Submit: </color> <color=white>" + "  Processing... " + "</color>");
        }
    }
}
