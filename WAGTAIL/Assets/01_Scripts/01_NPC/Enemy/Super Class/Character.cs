using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterPattern
{
    // ���� ����
    public enum Pattern
    {
        IDLE,
        MOVE,
        WAIT,
        SPECAIL1,
        SPECAIL2,
        SPECAIL3,
        SPECAIL4,
        DIE,
    }
    [SerializeField]
    public Pattern[] EPatterns;
    public int PhaseHp;
}

public class Character : MonoBehaviour
{

    //========================================
    //////      Property And Fields      /////
    //========================================
    public int HP;
    public int AttackDamage;

    public float MoveSpeed;

    public float AttackRange;
    public float AttackRate;
    public float IdleRate;
    public float WaitRate;
    protected float AttackTimer;

    // ������ ���� �����ΰ�?
    public bool IsHit;
    // ���� �����ΰ�?
    public bool isDeath;

    public MonsterPattern[] CharacterMovementPattern;
    // �����ֱ�� ���� � ������ �����Ǿ� �ִ��� Ȯ���ϴ� �뵵.
    public MonsterPattern.Pattern[] CurPattern;

    [Tooltip("ü�� ������ ��� ���� ���� ����")]
    private int CurPhaseHpArray = 0;

    public int GetCurPhaseHpArray
    {
        get
        {
            return CurPhaseHpArray;
        }
        set
        {
            CurPhaseHpArray = value;
        }

    }

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
    
    //=======================================
    //////       Public Methods          ////
    //=======================================

    public virtual void SetAttackPattern()
    {

    }
    public virtual void SettingPattern(MonsterPattern.Pattern[] pattern)
    {

    }
    protected virtual void AddPattern(AIState curPattern)
    {

    }

    public virtual void CAttack(Vector3 pos)
    {

    }

    public void AttackTimerReset()
    {
        AttackTimer = 0;
    }



    public bool isAttack()
    {
        if (AiSM.Target == null)
        {
            return false;
        }
        if (AttackTimer < AttackRate)
        {

            AttackTimer += Time.deltaTime;

            return false;
        }

        // ���� ���� ����
        return true;
    }
    //public AIIdleState AIIdleState;
}
