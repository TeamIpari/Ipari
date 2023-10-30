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
    private int      _progress = 0;


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

        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_BoomBurst);

        _bossCrab.StateTrigger = false;
        _bossCrab.ClearStateTriggerDelay();
        _bossCrab.PopHPStack();

        AISM.Animator.speed = 1f;

        /**보스가 체력이 모두 닳아서 죽을 경우...*/
        if((AISM.character.HP-=10)<=0f)
        {
            _progress = 1;
            AISM.Animator.speed = 0f;
            AISM.Animator.Play(BossCrabAnimation.Die, 0, .05f);
            CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .8f, .022f);
            _bossCrab.SetStateTrigger(1f);
            return;
        }
        else AISM.Animator.CrossFade(BossCrabAnimation.Hit, .1f, 0, 0f);

        _progress = 0;
        _bossCrab.SetStateTrigger(.3f);
        CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .6f, .022f);
        #endregion
    }

    public override void Exit()
    {
        base.Exit();
        AISM.character.IsHit = false;
    }

    public override void Update()
    {
        #region Omit
        if (_bossCrab.StateTrigger == false) return;

        /*****************************************************
         *   상태 트리거가 유효하면, 상황에 따른 로직을 적용한다...
         * ****/
        switch(_progress++){

                /**다음 패턴으로 넘어간다....*/
                case (0):
                {
                    AISM.NextPattern();
                    break;
                }

                /**데미지를 입고 몸을 올린다...*/
                case (1):
                {
                    AISM.Animator.speed = .8f;
                    CameraManager.GetInstance().CameraShake(1f, CameraManager.ShakeDir.ROTATE, .5f);
                    break;
                }

                /**쓰러진다...*/
                case (2):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**쓰러진 후의 진동효과...*/
                case (3):
                {
                    FModAudioManager.PlayOneShotSFX(

                        FModSFXEventType.Crab_Atk3Smash, 
                        _bossCrab.transform.position, 
                        2f
                    );

                    FModAudioManager.ApplyBGMFade(0f, 3f, 0, true);
                    CameraManager.GetInstance().CameraShake(.6f, CameraManager.ShakeDir.ROTATE, 1f);
                    break;
                }

        }

        _bossCrab.StateTrigger = false;

        #endregion
    }
}
