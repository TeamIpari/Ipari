using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesDieState : AIDieState
{
    //==========================================
    /////           magic Methods           ////
    //==========================================
    public BossNepenthesDieState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        AISM.Animator.SetTrigger("isDeath");
        BossRoomFieldManager.Instance.EnableBrokenPlatformComponent();
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
        base.Update();
    }
}
