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
        #region Omit
        base.Enter();

        _bossCrab.StateTrigger = false;
        _bossCrab.SetStateTrigger(.7f);
        _bossCrab.PopHPStack();

        AISM.character.HP -= 10;
        AISM.Animator.Play(BossCrabAnimation.Hit);
        #endregion
    }

    public override void Exit()
    {
        base.Exit();
        AISM.character.IsHit = false;
    }

    public override void Update()
    {
        if(_bossCrab.StateTrigger)
        {
            AISM.NextPattern();
            _bossCrab.StateTrigger = false;  
        }
    }
}
