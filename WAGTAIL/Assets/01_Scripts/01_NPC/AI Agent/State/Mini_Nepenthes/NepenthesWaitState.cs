using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NepenthesWaitState : AIWaitState
{
    private float CurTimer = 0;
    private float WaitTimer = 0;
    public NepenthesWaitState(AIStateMachine stateMachine, float WaitRate) : base(stateMachine)
    {
        WaitTimer = WaitRate;
    }

    public override void Enter()
    {
        base.Enter();
        CurTimer = 0;
        if (NextState == null)
            NextState = stateMachine.character.AiIdle;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        base.OntriggerEnter(other);
    }

    public override void Update()
    {
        if (CurTimer < WaitTimer)
        {
            CurTimer += Time.deltaTime;
        }
        else
            stateMachine.ChangeState(NextState);
    }
}
