using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

public abstract class AIState
{
    public AIStateMachine AISM;
    public AIState Parent;
    public List<AIState> Children = new List<AIState>();
    public int Current;
    protected float curTimer = 0;

    public AIState(AIStateMachine stateMachine)
    {
        this.AISM = stateMachine;
        Current = 0;
    }

    //=======================================================
    /////             Virtual Core Method                 /////
    //=======================================================

    public virtual void SetParent(AIState parent, AIState child )
    {
        child.Parent = parent;
        
    }

    public virtual void SetChildren(AIState state)
    {
        Children.Add(state);
    }

    //=======================================================
    /////             Abstract Magic Method                 /////
    //=======================================================

    public abstract void Enter();

    public virtual void Update() 
    { 
        if(AISM.character.IsHit)
        {
            AISM.ChangeState(AISM.character.AiHit);
        }
        if (Player.Instance.movementSM.currentState == Player.Instance.death)
        {
            return;
        }
    }

    public abstract void Exit();

    public abstract void OntriggerEnter(Collider other);
}
