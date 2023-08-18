using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesHitState : AIHitState
{
    float curTimer = 0;
    float delayTimer = 5.0f;
    int nextPhaseHp = 0;
  
    public BossNepenthesHitState(AIStateMachine stateMachine) : base(stateMachine)
    {
        nextPhaseHp = stateMachine.GetNextPhaseTargetHp();
    }


    public void SetPhaseHp()
    {
        Debug.Log($"���� ������ ����.");
        if (stateMachine.IsNextTargetPhaseHp())
        {
            nextPhaseHp = stateMachine.GetNextPhaseTargetHp();
            stateMachine.character.CurPhaseHpArray++;
        }
        else
            // ���� ����� ����.
            nextPhaseHp = 0;
    }
    public void SetNextPhase()
    {
        stateMachine.character.SettingPattern(stateMachine.EPattern);
    }

    public override void Enter()
    {
        base.Enter();
        // �´� �������� ����
        stateMachine.Animator.SetTrigger("IsHit");
        if (stateMachine.character.IsHit)
            stateMachine.character.HP -= 10;
        if(stateMachine.character.HP <= nextPhaseHp)
        {
            // ���� ����
            SetPhaseHp();
            SetNextPhase();
        }
        curTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.character.IsHit = false;
    }

    public override void Update()
    {
        base.Update();
        curTimer += Time.deltaTime;
        if(stateMachine.character.HP <= 0)
        {
            // ü���� 0 ������ ��� Death�� �ٷ� ���.
            stateMachine.ChangeState(stateMachine.character.AiDie);
        }
        if(curTimer > delayTimer)
        {
            stateMachine.NextPattern();
        }
    }
}
