using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // 공격 패턴
    public enum Pattern
    {
        IDLE,
        MOVE,
        WAIT,
        SPECAIL1,
        SPECAIL2,
        SPECAIL3,
        DIE,
    }
    public int HP;
    public int AttackDamage;

    public float MoveSpeed;

    public float AttackRange;
    public float AttackRate;
    public float WaitRate;
    protected float AttackTimer;

    // 공격을 받은 상태인가?
    public bool IsHit;
    // 죽은 상태인가?
    public bool isDeath;

    public Pattern[] CharacterMovementPattern;

    public Transform RotatePoint;

    // State Machine
    public AIStateMachine AiSM;

    // States 
    public AIIdleState AiIdle;
    public AIAttackState AiAttack;
    public AIMoveState AiMove;
    public AIWaitState AiWait;
    public AIHitState AiHit;
    public AIDieState AiDie;

    public virtual void SetAttackPattern()
    {

    }

    public bool isAttack()
    {
        if (AttackTimer < AttackRate)
        {
            AttackTimer += Time.deltaTime;

            return false;
        }
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
