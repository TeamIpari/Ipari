using System.Collections;
using System.Collections.Generic;
using static BossCrab;
using UnityEngine;

public sealed class BossCrabHitState : AIHitState
{
    //===============================================
    //////               Fields                //////
    //===============================================
    private BossCrab _bossCrab;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabHitState(AIStateMachine stateMachine, BossCrab bossCrab)
    : base(stateMachine)
    {
        _bossCrab = bossCrab;
    }

    public override void Enter()
    {
       base.Enter();

        curTimer = .5f;
        AISM.character.HP -= 10;
        AISM.Animator.Play(BossCrabAnimation.Hit, 0, 0f);
        _bossCrab.PopHPStack();
    }

    public override void Exit()
    {
        base.Exit();
        AISM.character.IsHit = false;
        AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsIdle);
    }

    public override void Update()
    {
        if((curTimer-=Time.deltaTime)<=0f){

            AISM.NextPattern();
        }
    }
}
