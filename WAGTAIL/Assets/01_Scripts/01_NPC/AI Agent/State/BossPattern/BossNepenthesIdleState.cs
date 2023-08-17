using System.Collections;
using System.Collections.Generic;
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
