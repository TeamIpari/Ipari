using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesHitState : AIHitState
{
    //=========================================
    /////       Property And Fields         /////
    //=========================================
    float curTimer = 0;
    float delayTimer = 5.0f;
    int nextPhaseHp = 0;
  
    //=========================================
    /////       Magic Mathods               /////
    //=========================================
    public BossNepenthesHitState(AIStateMachine stateMachine) : base(stateMachine)
    {
        nextPhaseHp = stateMachine.GetNextPhaseTargetHp();
    }


    public override void Enter()
    {
        base.Enter();
        // 맞는 상태임을 인지
        AISM.Animator.SetTrigger("IsHit");
        if (AISM.character.IsHit)
            AISM.character.HP -= 10;
        if(AISM.character.HP <= nextPhaseHp)
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
        AISM.character.IsHit = false;
    }

    public override void Update()
    {
        base.Update();
        curTimer += Time.deltaTime;
        if(AISM.character.HP <= 0)
        {
            // 체력이 0 이하일 경우 Death를 바로 출력.
            AISM.ChangeState(AISM.character.AiDie);
        }
        if(curTimer > delayTimer)
        {
            AISM.NextPattern();
        }
    }
    //===========================================
    /////           Core Methods            /////
    //===========================================
    public void SetPhaseHp()
    {
        Debug.Log($"다음 페이즈 돌입.");
        if (AISM.IsNextTargetPhaseHp())
        {
            nextPhaseHp = AISM.GetNextPhaseTargetHp();
            AISM.character.CurPhaseHpArray++;
        }
        else
            // 다음 페이즈가 없음.
            nextPhaseHp = 0;
    }
    public void SetNextPhase()
    {
        AISM.character.SettingPattern(AISM.EPattern);
    }
}
