using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesIdleState : AIIdleState
{
    public BossNepenthesIdleState(AIStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"Enter {this}");
    }

    public override void Update()
    {
        base.Update();
        stateMachine.NextPattern();

    }

    public override void Exit()
    {
        base.Exit();
    }
}
