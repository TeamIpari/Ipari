using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

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

    public virtual void Update() 
    { 
        if(stateMachine.character.IsHit)
        {
            stateMachine.ChangeState(stateMachine.character.AiHit);
        }
    }

    public abstract void Exit();

    public abstract void OntriggerEnter(Collider other);
}
