using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int HP;
    public int AttackDamage;

    public float MoveSpeed;

    public float AttackRange;
    public float AttackRate;
    protected float AttackTimer;

    // State Machine
    public AIStateMachine AiSM;

    // States 
    public AIIdleState AiIdle;
    public AIAttackState AiAttack;
    public AIMoveState AiMove;



    public virtual bool isAttack()
    {
        if (AttackTimer < AttackRate)
        {
            //Debug.Log("AA");
            AttackTimer += Time.deltaTime;

            return false;
        }
        //Debug.Log("BB");
        // ���� ���� ����
        return true;
    }

    public void AttackTimerReset()
    {
        AttackTimer = 0;
    }




    //public AIIdleState AIIdleState;
}
