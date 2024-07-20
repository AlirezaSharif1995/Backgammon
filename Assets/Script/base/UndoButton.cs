using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameSetup;

public class UndoButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;
    GameSetup GameSetup;
    PauseController pauseController;

    GameSetup.HistoryClass history;
    public GameSetup.HistoryClass historyProcessing;
    public bool canUndo;

    public Text textCount;

    public bool undoProccessing;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        pauseController = FindObjectOfType<PauseController>();

        socketManager = SocketManager.Instance;

        canUndo = false;
    }

    private void Update()
    {
        transform.GetComponent<Image>().enabled = canUndo;
        transform.GetChild(0).gameObject.SetActive(canUndo);
    }

    public void ButtonClick()
    {
        if (canUndo && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI))
        {
            GameSetup.Audio.Click.Play();

            // Local
            if (GameSetup.GeneralOptions.AI)
                Undo(false, 0);
            else
            // Online
            {
                socketManager.socket.Emit("Undo");
                Debug.Log("<color=yellow> Undo: </color> <color=white>" + "'  Processing... '" + "</color>");

                undoProccessing = true;
                StopCoroutine(EmitProccess("Undo"));
                StartCoroutine(EmitProccess("Undo"));
            }
        }
    }

    public IEnumerator EmitProccess(string emit)
    {
        yield return new WaitForSeconds(GameSetup.GeneralOptions.EmitTryTime * Time.deltaTime);
        if (emit == "Undo" && undoProccessing)
        {
            socketManager.socket.Emit("Undo");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> Undo: </color> <color=white>" + "  Processing... " + "</color>");
        }
    }

    public void Undo(bool fromServer, int fromPosition)
    {
        if (historyProcessing.checker == null)
        {
            bool founded = false;
            for (int i = GameSetup.History.Count - 1; i >= 0; i--)
            {
                if (GameSetup.History[i] != null)
                {
                    if (fromServer && GameSetup.History[i].startColumn.ID != fromPosition)
                        Debug.Log("<color=red> Wrong (FromPosition) from server </color>");

                    if (!fromServer || (fromServer && GameSetup.History[i].startColumn.ID == fromPosition))
                    {
                        history = GameSetup.History[i];
                        founded = true;
                        historyProcessing.checker = history.checker;
                        break;
                    }
                }
            }

            if (founded)
            {
                GameSetup.ResetColumnHighLight();
                GameSetup.ResetCheckers();
                history.checker.Stand = false;
                GameSetup.PlaceChecker(history.checker, history.startColumn, true, history.targetColumn, true);

                if (history.haveKick)
                    GameSetup.PlaceChecker(history.checkerKick, history.startColumnKick, true, history.targetColumnKick, true);

                for (int j = 0; j < history.roll.Count; j++)
                    GameSetup.Roll.Rolls.Add(history.roll[j]);

                if (GameSetup.Roll.playerTurn)
                {
                    StartCoroutine(GameSetup.UsableCheckerDelay(Time.deltaTime, GameSetup.GeneralOptions.PlayerLocation, GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls));
                    GameSetup.playerDone(false);
                }

                StartCoroutine(RemoveHistory(history, 2 * Time.deltaTime + 0.1f));
                Debug.Log("Undo: " + GameSetup.History.Count);

            }else
            {
                Debug.Log("Undo: " + "Not Founded");
            }
        }
    }

    IEnumerator RemoveHistory(HistoryClass history, float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);
        GameSetup.History.Remove(history);
        historyProcessing.checker = null;
    }
}
