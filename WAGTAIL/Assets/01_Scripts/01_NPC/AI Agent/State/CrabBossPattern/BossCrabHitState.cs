using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabHitState : AIHitState
{
    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabHitState(AIStateMachine stateMachine)
    : base(stateMachine)
    {
    }

    public override void Enter()
    {
       base.Enter();
        curTimer = 2f;
        AISM.character.HP -= 10;
    }

    public override void Update()
    {
        if((curTimer-=Time.deltaTime)<=0f){

            AISM.NextPattern();
        }
    }
}
