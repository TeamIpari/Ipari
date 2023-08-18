using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossNepenthesIdleState : AIIdleState
{
    float waitTime;
    float curTime;
    public BossNepenthesIdleState(AIStateMachine stateMachine, float waitTime) : base(stateMachine)
    {
        this.waitTime = waitTime;
    }

    public override void Enter()
    {
        base.Enter();
        curTime = 0.0f;
        Debug.Log($"Enter {this.ToString()}");
    }

    public override void Update()
    {
        base.Update();
        curTime += Time.deltaTime;
        if (stateMachine.character.IsHit)
        {
            stateMachine.ChangeState(stateMachine.character.AiHit);
        }
        if(stateMachine.character.isDeath)
        {
            stateMachine.ChangeState(stateMachine.character.AiDie);
        }
        if (curTime > waitTime)
        {
            stateMachine.NextPattern();
            return;
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
