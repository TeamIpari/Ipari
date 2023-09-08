using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PullInOutState : State
{
    //======================================
    ////       Property and fields      ////
    ///=====================================
    PullableObject PulledTarget { get; }
    float          PullPower    { get; set; }


    //===================================
    ////       Override methods      ////
    //===================================
    public PullInOutState(Player player, StateMachine stateMachine)
    :base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        
    }

    public override void HandleInput()
    {

    }

    public override void LogicUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }

    public override void Exit()
    {

    }


    //===================================
    ////       Core methods         /////
    ///==================================
    public void HoldTarget(PullableObject target)
    {

    }

    
}
