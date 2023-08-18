using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesHitState : AIHitState
{
    float curTimer = 0;
    float delayTimer = 5.0f;
    public BossNepenthesHitState(AIStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 맞는 상태임을 인지
        stateMachine.Animator.SetTrigger("IsHit");
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
