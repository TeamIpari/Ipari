using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIIdleState : AIState
{
    public AIIdleState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void OntriggerEnter(Collider other)
    {

    }

    public override void Update()
    {
        base.Update();
    }

}
