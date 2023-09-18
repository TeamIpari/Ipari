using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesWaitState : AIWaitState
{
    private float CurTimer = 0;
    private float WaitTimer = 0;

    public BossNepenthesWaitState(AIStateMachine stateMachine, float waitRate) : base(stateMachine)
    {
        WaitTimer = waitRate;
    }

    public override void Enter()
    {
        base.Enter();
        CurTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void SetNextState(AIState nextState)
    {
        base.SetNextState(nextState);
    }

    public override void Update()
    {
        base.Update();
        CurTimer += Time.deltaTime;
        if (CurTimer > WaitTimer)
            AISM.NextPattern();
    }
}
