using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    public AIStateMachine stateMachine;
    public AIState parent;
    public List<AIState> children = new List<AIState>();
    public int current;
    public AIState(AIStateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
        current = 0;
    }

    public virtual void SetParent(AIState _parent, AIState _child )
    {
        _child.parent = _parent;
        
    }

    public virtual void SetChildren(AIState _state)
    {
        children.Add(_state);
    }

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exit();

    public abstract void OntriggerEnter(Collider other);
}
