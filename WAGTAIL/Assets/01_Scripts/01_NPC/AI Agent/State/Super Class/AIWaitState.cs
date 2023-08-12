using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIWaitState : AIState
{
    public AIState NextState;

    public AIWaitState(AIStateMachine stateMachine) : base(stateMachine)
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

    }

    public virtual void SetNextState(AIState nextState)
    {
        NextState = nextState;
    }

}
