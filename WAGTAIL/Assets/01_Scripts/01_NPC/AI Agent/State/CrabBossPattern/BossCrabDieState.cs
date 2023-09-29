using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BossCrabDieState : AIDieState
{
    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabDieState(AIStateMachine stateMachine)
    : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AISM.Animator.SetTrigger("isDeath");
    }

}
