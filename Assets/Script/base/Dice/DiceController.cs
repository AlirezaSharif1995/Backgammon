using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public bool Roll;
    public bool Fixing;
    bool Rolled;
    public bool Updated;
    public bool Show;
    public GameSetup.Location Location;
    GameSetup GameSetup;

    public Material Material1;
    public Material Material2;
    public Material Material3;

    public enum owner
    {
        Player,
        opponent
    }
    public owner Owner;

    [Header("Options")]
    public float RollRotSpeed;
    public float RollSpeed;
    //
    public DiceScript Dice1;
    public DiceScript Dice2;

    // Target Roll
    [System.Serializable]
    public class TargetRollClass
    {
        public List<Transform> Right;
        public List<Transform> Left;
    }
    public TargetRollClass TargetRoll;

    // Start Roll
    [System.Serializable]
    public class StartRollClass
    {
        public Transform Right;
        public Transform Left;
    }
    public StartRollClass StartRoll;

    public Quaternion q;


    private void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
    }

    public void RollDice()
    {
        Fixing = false;
        Rolled = false;
        Roll = true;
    }

    private void Update()
    {
        if (!Updated && GameSetup.Updated)
        {
            if (Owner == owner.Player && GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Down || Owner == owner.opponent && GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Down)
                Location = GameSetup.Location.Down;
            else
            if (Owner == owner.Player && GameSetup.GeneralOptions.PlayerLocation == GameSetup.Location.Up || Owner == owner.opponent && GameSetup.GeneralOptions.OpponentLocation == GameSetup.Location.Up)
                Location = GameSetup.Location.Up;

            Updated = true;
        }

        if (Updated)
        {
            if (Roll && !Fixing)
            {
                if (Location == GameSetup.Location.Down)
                {
                    Dice1.Target = TargetRoll.Right[0];
                    Dice2.Target = TargetRoll.Right[1];

                    Dice1.transform.position = StartRoll.Right.GetChild(0).position;
                    Dice2.transform.position = StartRoll.Right.GetChild(1).position;
                }
                else
                {
                    Dice1.Target = TargetRoll.Left[0];
                    Dice2.Target = TargetRoll.Left[1];

                    Dice1.transform.position = StartRoll.Left.GetChild(0).position;
                    Dice2.transform.position = StartRoll.Left.GetChild(1).position;
                }

                Dice1.transform.rotation = Random.rotation;//q;//Random.rotation;
                Dice2.transform.rotation = Random.rotation;//q;//Random.rotation;

                Dice1.Roll = true;
                Dice2.Roll = true;

                Show = true;
                Fixing = true;
            }

            //Debug.Log(Dice1.rigid.velocity == Vector3.zero);
            float distance = Vector3.Distance(Dice1.transform.position, Dice2.transform.position);

            if (distance < 1 /*&& (Dice1.rigid.velocity == Vector3.zero || Dice2.rigid.velocity == Vector3.zero) */&& Roll && (Dice1.DiceColl || Dice2.DiceColl))
            {
                Vector3 direction = Dice1.transform.position - Dice2.transform.position;
                Dice1.rigid.AddForce(direction.normalized * RollSpeed * 3);
            }

            if (Roll && Dice1.rigid.velocity.magnitude > 0)
                Rolled = true;

            // Done RollDice
            if (Rolled && Dice1.rigid.angularVelocity == Vector3.zero && Dice2.rigid.velocity == Vector3.zero)
            {
                StartCoroutine(UpdateRoll(30));
                Roll = false;
                Fixing = false;
                Rolled = false;
            }
        }
    }

    IEnumerator UpdateRoll(float delay)
    {
        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (!Dice1.CheckNoCollCounter() && !Dice2.CheckNoCollCounter())
        {
            GameSetup.Roll.Rolls.Clear();

            int e1 = Dice1.Dice;
            int e2 = Dice2.Dice;

            GameSetup.Roll.Rolls.Add(e1);
            GameSetup.Roll.Rolls.Add(e2);

            if (e1 == e2)
            {
                GameSetup.Roll.Rolls.Add(e1);
                GameSetup.Roll.Rolls.Add(e2);
            }

            if (Owner == owner.Player)
            {
                GameSetup.UsableChecker(GameSetup.GeneralOptions.PlayerLocation, GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls);
                GameSetup.SetRollUI(GameSetup.GeneralOptions.playerSide, GameSetup.Roll.Rolls);
            }
            else if (GameSetup.GeneralOptions.AI)
                StartCoroutine(GameSetup.AIAct(GameSetup.GeneralOptions.delayAIMove, "random"));

            //GameSetup.UsableChecker(GameSetup.GeneralOptions.OpponentLocation, GameSetup.GeneralOptions.OpponentSide, GameSetup.Roll.Rolls);

            Debug.Log("e: " + e1 + "," + e2);
        }else
        {
            Debug.Log("noColl");

            Vector3 direction = Dice1.transform.position - Dice2.transform.position;
            direction.Normalize();

            if (Dice1.CheckNoCollCounter())
                Dice1.rigid.AddForce(direction * RollSpeed / 5, ForceMode.Impulse);
            else
            if (Dice2.CheckNoCollCounter())
                Dice2.rigid.AddForce(direction * RollSpeed / 5, ForceMode.Impulse);


            StartCoroutine(UpdateRoll(50));
        }
    }
}
