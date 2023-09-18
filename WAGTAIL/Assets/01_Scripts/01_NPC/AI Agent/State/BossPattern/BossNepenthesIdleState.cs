using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossNepenthesIdleState : AIIdleState
{
    //===========================================
    /////       Property And Fields         /////
    //===========================================
    float waitTime;
    float curTime;

    //=======================================
    //////       Public Methods          ////
    //=======================================
    public BossNepenthesIdleState(AIStateMachine stateMachine, float waitTime) : base(stateMachine)
    {
        this.waitTime = waitTime;
    }

    public override void Enter()
    {
        base.Enter();
        curTime = 0.0f;
        Debug.Log($"Enter {this.ToString()}");
    }

    public override void Update()
    {
        base.Update();
        curTime += Time.deltaTime;
        if (AISM.character.IsHit)
        {
            AISM.ChangeState(AISM.character.AiHit);
        }
        if(AISM.character.isDeath)
        {
            AISM.ChangeState(AISM.character.AiDie);
        }
        if (curTime > waitTime)
        {
            AISM.NextPattern();
            return;
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
