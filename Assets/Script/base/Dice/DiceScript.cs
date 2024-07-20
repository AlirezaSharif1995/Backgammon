using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DiceScript : MonoBehaviour
{
    DiceController controller;
    public bool RotRoll;
    public bool Roll;
    bool Rolled;
    public Transform Target;
    public Rigidbody rigid;

    public int Dice;

    MeshRenderer mesh;
    GameSetup GameSetup;
    public bool Updated;

    public bool DiceColl;

    public List<DiceCounter> DiceCounters = new List<DiceCounter>();

    private void Start()
    {
        controller = transform.parent.GetComponent<DiceController>();
        rigid = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        GameSetup = FindObjectOfType<GameSetup>();

        for (int i = 0; i < 6; i++)
        {
            DiceCounters.Add(new DiceCounter());
            DiceCounters[i] = transform.GetChild(i).GetComponent<DiceCounter>();
        }
    }
    private void Update()
    {
        if (!Updated && controller.Updated)
        {
            if (GameSetup.GeneralOptions.Type == GameSetup.type.type1)
                mesh.material = controller.Material1;
            else
            if (GameSetup.GeneralOptions.Type == GameSetup.type.type2)
                mesh.material = controller.Material2;
            else
            if (GameSetup.GeneralOptions.Type == GameSetup.type.type3)
                mesh.material = controller.Material3;

            Updated = true;
        }

        if (Roll)
            RollDice();

        mesh.enabled = controller.Show;
    }

    public bool CheckNoCollCounter()
    {
        bool noColl = true;
        for(int i=0;i< DiceCounters.Count; i++)
        {
            if (DiceCounters[i].DiceColl)
            {
                noColl = false;
                break;
            }
        }
        return noColl;
    }

    public void RollDice()
    {
        Quaternion rot = transform.rotation;
        Vector3 eulerRot = rot.eulerAngles;
        eulerRot.z -= controller.RollRotSpeed;
        eulerRot.y += controller.RollRotSpeed / 10;
        rot = Quaternion.Euler(eulerRot);
        transform.rotation = rot;

        Vector3 direction = (Target.position - transform.position).normalized;
        rigid.velocity = direction * controller.RollSpeed;
        transform.position = Vector3.MoveTowards(transform.position, Target.position, controller.RollSpeed * Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && controller.Fixing)
        {
            Roll = false;
        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.Normalize();
            rigid.AddForce(direction * controller.RollSpeed / 5, ForceMode.Impulse);
        }

        DiceColl = collision.gameObject.layer == LayerMask.NameToLayer("Dice");
       

    }
    bool IsCloseToAngle(float angle, float targetAngle)
    {
        return Mathf.Approximately(angle, targetAngle);
    }

}
