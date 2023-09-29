using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesDieState : AIDieState
{

    //==========================================
    /////        properties Methods         ////
    //==========================================
    private float curTimer;
    private float brokenTime;
    private bool oneChance;
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
        curTimer = 0;
        brokenTime = 3.2f;
        oneChance = false;
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
        curTimer += Time.deltaTime;
        if(curTimer > brokenTime && !oneChance)
        {
            BossRoomFieldManager.Instance.EnableBrokenPlatformComponent();
            oneChance = true;
        }
    }
}
