using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishCount : MonoBehaviour
{
    GameSetup GameSetup;
    Transform Up;
    Transform Down;

    TextMeshPro textUp;
    TextMeshPro textDown;

    public bool Updated;

    public int playerCount;
    public int opponentCount;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        Up = transform.GetChild(0);
        Down = transform.GetChild(1);
        textUp = Up.GetComponent<TextMeshPro>();
        textDown = Down.GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (GameSetup.Updated && !Updated)
        {
            Vector3 UpPos = Up.position;
            Vector3 DownPos = Down.position;
            UpPos = GameSetup.finishColumn[0].Pos;
            DownPos = GameSetup.finishColumn[1].Pos;

            UpPos.x += GameSetup.GeneralOptions.textDistanceX;
            DownPos.x += GameSetup.GeneralOptions.textDistanceX;

            UpPos.z -= GameSetup.GeneralOptions.textDistanceZ;
            DownPos.z -= GameSetup.GeneralOptions.textDistanceZ;

            UpPos.y += GameSetup.GeneralOptions.textDistanceY;
            DownPos.y += GameSetup.GeneralOptions.textDistanceY;

            Up.position = UpPos;
            Down.position = DownPos;

            /*textUp.fontSize = GameSetup.GeneralOptions.textSize;
            textDown.fontSize = GameSetup.GeneralOptions.textSize;*/

            Updated = true;
        }

        if (Updated)
        {
            /*textUp.gameObject.SetActive(GameSetup.finishColumn[0].FullCount > 0);
            textDown.gameObject.SetActive(GameSetup.finishColumn[1].FullCount > 0);
            textUp.text = GameSetup.finishColumn[0].FullCount.ToString();
            textDown.text = GameSetup.finishColumn[1].FullCount.ToString();*/

            playerCount = GameSetup.finishColumn[1].FullCount;
            opponentCount = GameSetup.finishColumn[0].FullCount;
        }
    }
}
