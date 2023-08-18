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
        Debug.Log($"다음 페이즈 돌입.");
        if (stateMachine.IsNextTargetPhaseHp())
        {
            nextPhaseHp = stateMachine.GetNextPhaseTargetHp();
            stateMachine.character.CurPhaseHpArray++;
        }
        else
            // 다음 페이즈가 없음.
            nextPhaseHp = 0;
    }
    public void SetNextPhase()
    {
        stateMachine.character.SettingPattern(stateMachine.EPattern);
    }

    public override void Enter()
    {
        base.Enter();
        // 맞는 상태임을 인지
        stateMachine.Animator.SetTrigger("IsHit");
        if (stateMachine.character.IsHit)
            stateMachine.character.HP -= 10;
        if(stateMachine.character.HP <= nextPhaseHp)
        {
            // 다음 패턴
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
            // 체력이 0 이하일 경우 Death를 바로 출력.
            stateMachine.ChangeState(stateMachine.character.AiDie);
        }
        if(curTimer > delayTimer)
        {
            stateMachine.NextPattern();
        }
    }
}
