using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NepenthesWaitState : AIWaitState
{
    private float WaitTimer = 0;
    public NepenthesWaitState(AIStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        WaitTimer = 0;
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
        if (WaitTimer < stateMachine.character.WaitRate)
        {
            WaitTimer += Time.deltaTime;
        }
        else
            stateMachine.ChangeState(NextState);
    }
}
