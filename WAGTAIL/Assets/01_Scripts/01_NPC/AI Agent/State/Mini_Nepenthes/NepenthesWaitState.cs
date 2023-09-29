using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NepenthesWaitState : AIWaitState
{
    private float curTimer = 0;
    private float waitTimer = 0;
    public NepenthesWaitState(AIStateMachine stateMachine, float waitRate) : base(stateMachine)
    {
        waitTimer = waitRate;
    }

    public override void Enter()
    {
        base.Enter();
        curTimer = 0;
        if (NextState == null)
            NextState = AISM.character.AiIdle;
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
        if (curTimer < waitTimer)
        {
            curTimer += Time.deltaTime;
        }
        else
            AISM.ChangeState(NextState);
    }
}
