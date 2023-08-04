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
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetNextState(AIState nextState)
    {
        NextState = nextState;
    }

}
