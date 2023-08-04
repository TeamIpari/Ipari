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
    public float WaitRate;
    protected float AttackTimer;

    // State Machine
    public AIStateMachine AiSM;

    // States 
    public AIIdleState AiIdle;
    public AIAttackState AiAttack;
    public AIMoveState AiMove;
    public AIWaitState AiWait;



    public bool isAttack()
    {
        if (AttackTimer < AttackRate)
        {
            //Debug.Log("AA");
            AttackTimer += Time.deltaTime;

            return false;
        }
        //Debug.Log("BB");
        // 공격 가능 상태
        return true;
    }

    public virtual void CAttack()
    {

    }

    public void AttackTimerReset()
    {
        AttackTimer = 0;
    }




    //public AIIdleState AIIdleState;
}
