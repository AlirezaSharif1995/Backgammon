using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollButton : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    GameSetup GameSetup;
    DoubleController DoubleController;
    PlayerInfo playerInfo;
    Image image;
    PauseController pauseController;

    public bool canRoll;
    public bool can;
    bool rollProccessing;

    public GameObject diceIcon;
    
    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        DoubleController = FindObjectOfType<DoubleController>();
        playerInfo = FindObjectOfType<PlayerInfo>();
        image = transform.GetComponent<Image>();
        pauseController = FindObjectOfType<PauseController>();

        socketManager = SocketManager.Instance;

        canRoll = false;
    }

    public void ButtonClick()
    {
        if (can)
        {
            GameSetup.Audio.Click.Play();
            StartCoroutine(PlayerRoll());
            canRoll = false;
        }
    }

    public IEnumerator PlayerRoll()
    {
        bool Rolled = false;
        bool FirstMove = false;

        int e1 = 0;
        int e2 = 0;

        bool isBlocked = false;

        // Local Roll
        if (GameSetup.GeneralOptions.AI)
        {
            if (!GameSetup.StartingRollDice)
            {
                GameSetup.GeneralOptions.DiceAnimConPlayer.EndShow();
                GameSetup.GeneralOptions.DiceAnimConOpponent.EndShow();

            }else
            {
                e1 = GameSetup.Roll.playerStartRoll;
                e2 = GameSetup.Roll.opponentStartRoll;

                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);

                if (e1 == e2)
                {
                    GameSetup.Roll.Rolls.Add(e1);
                    GameSetup.Roll.Rolls.Add(e2);
                }
            }

            Rolled = true;
        }
        else
        // Online Roll
        if (GameSetup.GeneralOptions.Online && socketManager.Connected)
        {
            GameSetup.Roll.Rolls.Clear();

            if (socketManager.GameData.FirstMove) // First Move
            {
                e1 = socketManager.GameData.FirstRollDice.rolledDice[0];
                e2 = socketManager.GameData.FirstRollDice.rolledDice[1];

                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);

                if (e1 == e2)
                {
                    GameSetup.Roll.Rolls.Add(e1);
                    GameSetup.Roll.Rolls.Add(e2);
                }

                socketManager.GameData.FirstMove = false;

                GameSetup.SetRollUI(GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls);

                FirstMove = true;
                Rolled = true;

            }else // Other Move
            {
                GameSetup.GeneralOptions.DiceAnimConPlayer.EndShow();
                GameSetup.GeneralOptions.DiceAnimConOpponent.EndShow();

                socketManager.socket.On("RollDice", (response) =>
                {
                    if (GameSetup.DeveloperMode)
                        Debug.Log("<color=blue>(DEVELOPER MODE) </color> <color=yellow> RollDice: (Player) </color> <color=white>" + response.ToString() + "</color>");

                    var result = JsonConvert.DeserializeObject<List<SocketManager.RollDiceData>>(response.ToString());

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        rollProccessing = false;
                        StopCoroutine(EmitProccess("RollDice"));
                        
                        if (result[0].currentPlayer == playerInfo.PlayerData.token)
                        {
                            e1 = result[0].rolledDice[0];
                            e2 = result[0].rolledDice[1];

                            GameSetup.Roll.Rolls.Add(e1);
                            GameSetup.Roll.Rolls.Add(e2);

                            if (e1 == e2)
                            {
                                GameSetup.Roll.Rolls.Add(e1);
                                GameSetup.Roll.Rolls.Add(e2);
                            }

                            isBlocked = result[0].isBlocked;

                            Debug.Log("<color=green> RollDice: (Player) </color> <color=white> Done </color>");
                            Rolled = true;
                        }
                    });
                });

                socketManager.socket.Emit("RollDice");
                Debug.Log("<color=yellow> RollDice (Player) </color> <color=white>  Processing... </color>");

                rollProccessing = true;
                StopCoroutine(EmitProccess("RollDice"));
                StartCoroutine(EmitProccess("RollDice"));
            }
        }

        if ((!FirstMove && GameSetup.GeneralOptions.Online) || (!GameSetup.StartingRollDice && GameSetup.GeneralOptions.AI))
        {
            while (!Rolled) { yield return new WaitForSeconds(Time.deltaTime); /* Wait for Rolled */ }
            float delay = 0.1f;
            if (GameSetup.StartingRollDice) delay = 30;
            GameSetup.StartingRollDice = false;

            StartCoroutine(GameSetup.GeneralOptions.DiceAnimConPlayer.RollDice(delay,(!GameSetup.GeneralOptions.AI && GameSetup.GeneralOptions.Online && isBlocked)));

        }else
        {
            StartCoroutine(GameSetup.GeneralOptions.DiceAnimConPlayer.StartTurn(50,false));
            GameSetup.StartingRollDice = true;
        }
    }

    void Update()
    {
        image.enabled = canRoll && !DoubleController.Requesting && !GameSetup.Roll.AutoRoll;
        diceIcon.SetActive(canRoll);
        can = canRoll && !DoubleController.Requesting && !socketManager.GameData.FirstMove && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI);

        if (GameSetup.Roll.AutoRoll && can)
            ButtonClick();
    }

    IEnumerator EmitProccess(string emit)
    {
        yield return new WaitForSeconds(GameSetup.GeneralOptions.EmitTryTime * Time.deltaTime);
        if (emit == "RollDice" && rollProccessing)
        {
            socketManager.socket.Emit("RollDice");
            Debug.Log("<color=red> (TRY) </color> <color=yellow> RollDice (Player) </color> <color=white>  Processing... </color>");
        }
    }

    public int GetRandomNumber(int[] numbers)
    {
        int n = numbers.Length;
        for (int i = 0; i < n; i++)
        {
            int rnd = UnityEngine.Random.Range(i, n);
            int temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }
        return numbers[0];
    }
}
