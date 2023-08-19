using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public float IdleRate;
    public float WaitRate;
    protected float AttackTimer;

    // ������ ���� �����ΰ�?
    public bool IsHit;
    // ���� �����ΰ�?
    public bool isDeath;

    public MonsterPattern[] CharacterMovementPattern;
    // �����ֱ�� ���� � ������ �����Ǿ� �ִ��� Ȯ���ϴ� �뵵.
    public MonsterPattern.Pattern[] curPattern;

    [Tooltip("ü�� ������ ��� ���� ���� ����")]
    public int CurPhaseHpArray = 0;
    [Tooltip("ü�� ������ ��� ���� ���� ����")]

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
        // ���� ���� ����

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
