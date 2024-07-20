using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CheckerController;

public class DiceAnimController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    public enum own
    {
        player,
        opponent
    }

    public own Own;

    public GameObject Dice1;
    public GameObject Dice2;

    public Animator animDice1;
    public Animator animDice2;
    
    public SpriteRenderer spriteDice1;
    public SpriteRenderer spriteDice2;

    public Transform[] playerDicePos;
    public Transform[] opponentDicePos;
    
    bool lightOn;

    public List<Transform> Targets;

    public bool Roll;
    public bool Show;

    int roll1;
    int roll2;

    GameSetup GameSetup;

    public bool Right;

    public bool Updated;

    public List<AudioSource> Sound;

    public AudioSource DoubleDiceAudio1;
    public AudioSource DoubleDiceAudio2;

    public Sprite[] dices;
    public Color diceNormalColor;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        animDice1 = Dice1.GetComponent<Animator>();
        animDice2 = Dice2.GetComponent<Animator>();
        
        spriteDice1 = Dice1.GetComponent<SpriteRenderer>();
        spriteDice2 = Dice2.GetComponent<SpriteRenderer>();

        socketManager = SocketManager.Instance;
        EndShow();
        
        animDice1.enabled = false;
        animDice2.enabled = false;
    }

    void Update()
    {
        if (!Updated && GameSetup.Updated)
        {
            Right = (Own == own.player && GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down) || (Own == own.opponent && GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Down);
            Updated = true;
        }
    }

    public IEnumerator RollDice(float dealy, bool isBlocked)
    {
        yield return new WaitForSeconds(dealy * Time.deltaTime);

        Dice1.SetActive(true);
        Dice2.SetActive(true);
        animDice1.enabled = true;
        animDice2.enabled = true;

        // Local Roll
        if (GameSetup.GeneralOptions.AI)
        {
            GameSetup.Roll.Rolls.Clear();

            int e1 = 0;
            int e2 = 0;

            if (!GameSetup.StartingRollDice)
            {
                int[] roll = { 1, 2, 3, 4, 5, 6 };
                e1 = GetRandomNumber(roll);
                e2 = Random.Range(1, 7);
            }

            GameSetup.Roll.Rolls.Add(e1);
            GameSetup.Roll.Rolls.Add(e2);

            if (e1 == e2)
            {
                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);
            }

            Debug.Log("Local Roll: " + "e: (" + e1 + "," + e2 + ")");
        }

        roll1 = GameSetup.Roll.Rolls[0];
        roll2 = GameSetup.Roll.Rolls[1];

        StartCoroutine(StartTurn(100, isBlocked));

        Show = true;
        Roll = true;

        StartCoroutine(playSound());
    }

    public void StopDice()
    {
        animDice1.enabled = false;
        animDice2.enabled = false;
        spriteDice1.color = diceNormalColor;
        spriteDice2.color = diceNormalColor;
        Dice1.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        Dice2.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
    }

    public void StartRollDice()
    {
        if (!GameSetup.Roll.playerTurn && !GameSetup.Roll.opTurn)
        {
            Dice1.SetActive(true);
            animDice1.enabled = true;
            
            int e = 0;

            // Local Roll
            if (GameSetup.GeneralOptions.AI)
            {
                GameSetup.Roll.Rolls.Clear();

                int[] roll = { 1, 2, 3, 4, 5, 6 };
                e = GetRandomNumber(roll);
            }
            else
            // Online Roll
            if (GameSetup.GeneralOptions.Online)
            {
                if (Own == own.player)
                    e = socketManager.GameData.Player.starterDice;
                else
                if (Own == own.opponent)
                    e = socketManager.GameData.Opponent.starterDice;
            }

            if (Own == own.player)
                GameSetup.Roll.playerStartRoll = e;
            else
            if (Own == own.opponent)
                GameSetup.Roll.opponentStartRoll = e;

            roll1 = e;

            if (Own == own.player)
                StartCoroutine(GameSetup.checkStartRollDice(100));

            Show = true;
            Roll = true;

            StartCoroutine(playSound());
        }
    }

    IEnumerator playSound()
    {
        yield return new WaitForSeconds(30 * Time.deltaTime);
        int play = Random.Range(0, Sound.Count);
        Sound[play].Play();
        yield return new WaitForSeconds(25 * Time.deltaTime);
        if (play != 0) Sound[play-1].Play(); else Sound[Sound.Count-1].Play();
    }

    public int GetRandomNumber(int[] numbers)
    {
        int n = numbers.Length;
        for (int i = 0; i < n; i++)
        {
            int rnd = Random.Range(i, n);
            int temp = numbers[i];
            numbers[i] = numbers[rnd];
            numbers[rnd] = temp;
        }
        return numbers[0];
    }

    public void EndShow()
    {
        Show = false;
        Roll = false;
        
        Dice1.SetActive(false);
        Dice2.SetActive(false);
    }

    public IEnumerator StartTurn(float delay, bool isBlocked)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (Dice1.activeSelf && Dice2.activeSelf)
        {
            StopDice();
            spriteDice1.sprite = dices[GameSetup.Roll.Rolls[0] - 1];
            spriteDice2.sprite = dices[GameSetup.Roll.Rolls[1] - 1];
        }

        if (Own == own.opponent)
        {
            StartCoroutine(GameSetup.SetRollUI(GameSetup.GeneralOptions.OpponentSide, GameSetup.Roll.Rolls));

            if (GameSetup.GeneralOptions.AI)
                StartCoroutine(GameSetup.AIAct(GameSetup.GeneralOptions.delayAIMove, "intelligent"));

            if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                if (isBlocked)
                    GameSetup.BlockedOnline("opponent");
            }
        }
        else
        {
            GameSetup.UsableChecker(GameSetup.GeneralOptions.PlayerLocation, GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls);
            StartCoroutine(GameSetup.SetRollUI(GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls));

            if (GameSetup.GeneralOptions.Online && socketManager.Connected)
            {
                if (isBlocked)
                    GameSetup.BlockedOnline("player");
            }
        }

        GameSetup.StartingRollDice = false;
    }
}
