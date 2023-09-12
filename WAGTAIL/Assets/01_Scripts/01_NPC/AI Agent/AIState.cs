using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AIState
{
    public AIStateMachine stateMachine;
    public AIState parent;
    public List<AIState> children = new List<AIState>();
    public int current;

    public AIState(AIStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        current = 0;
    }

    //=======================================================
    /////             Virtual Core Method                 /////
    //=======================================================

    public virtual void SetParent(AIState parent, AIState child )
    {
        child.parent = parent;
        
    }

    public virtual void SetChildren(AIState state)
    {
        children.Add(state);
    }

    //=======================================================
    /////             Abstract Magic Method                 /////
    //=======================================================

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();

    public abstract void OntriggerEnter(Collider other);
}
