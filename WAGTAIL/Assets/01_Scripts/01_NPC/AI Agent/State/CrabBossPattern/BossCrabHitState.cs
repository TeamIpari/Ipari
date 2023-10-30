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

        CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .6f, .022f);

        _bossCrab.StateTrigger = false;
        _bossCrab.ClearStateTriggerDelay();
        _bossCrab.PopHPStack();

        AISM.Animator.speed = 1f;

        /**보스가 체력이 모두 닳아서 죽을 경우...*/
        if((AISM.character.HP-=10)<=0f)
        {
            AISM.Animator.CrossFade(BossCrabAnimation.Die, .1f, 0, 0f);
            return;
        }
        else AISM.Animator.CrossFade(BossCrabAnimation.Hit, .1f, 0, 0f);

        _bossCrab.SetStateTrigger(.3f);
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
            _bossCrab.StateTrigger = false;
            AISM.NextPattern();
        }
    }
}
