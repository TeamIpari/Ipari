using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class MonsterPattern
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
    [SerializeField]
    public Pattern[] EPatterns;
    public int PhaseHp;
}

public class Character : MonoBehaviour
{
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

    public MonsterPattern[] CharacterMovementPattern;
    // 보여주기용 현재 어떤 패턴이 장착되어 있는지 확인하는 용도.
    public MonsterPattern.Pattern[] curPattern;

    [Tooltip("체력 이하일 경우 다음 패턴 시작")]
    public int CurPhaseHpArray = 0;
    [Tooltip("체력 이하일 경우 다음 패턴 시작")]

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
    public virtual void SettingPattern(MonsterPattern.Pattern[] _pattern)
    {

    }
    protected virtual void AddPattern(AIState curPattern)
    {

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
