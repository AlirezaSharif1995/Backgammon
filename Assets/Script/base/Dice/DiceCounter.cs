using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCounter : MonoBehaviour
{
    DiceScript DiceScript;
    public int Dice;
    public bool DiceColl;

    private void Start()
    {
        DiceScript = transform.parent.GetComponent<DiceScript>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            DiceScript.Dice = Dice;
            DiceColl = true;
        }
        else
            DiceColl = false;
    }
}
