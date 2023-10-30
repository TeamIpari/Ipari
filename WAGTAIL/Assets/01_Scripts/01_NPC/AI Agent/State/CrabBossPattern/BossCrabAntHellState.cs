using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossCrab;

public class BossCrabAntHellState : AIAttackState
{
    //========================================
    //////          Property            //////
    //========================================
    private SandScriptBase _targetSand;
    private BossCrab       _bossCrab;
    private float          _duration = 0f;
    private int            _progress = 0;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabAntHellState(AIStateMachine stateMachine, GameObject andhellPrefab, float duration, BossCrab bossCrab)
    : base(stateMachine)
    {
        #region Omit
        _duration = duration;
        _bossCrab = bossCrab;

        if (andhellPrefab != null){

            _targetSand = andhellPrefab.GetComponent<SandScriptBase>();
            _targetSand.IntakeStopDuration = 7f;
        }
        #endregion
    }

    public override void Enter()
    {
        #region Omit
        _progress = 0;
        AISM.Animator.CrossFade(BossCrabAnimation.MakeAntHell_Ready, .3f);
        #endregion
    }

    public override void Update()
    {
        #region Omit
        base.Update();

        /*****************************************
         *   상태 트리거가 참일 경우에만 로직 적용...
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch (_progress++){

                /**모래지옥을 생성한다...*/
                case (0):
                {
                    if (_targetSand != null)
                        _targetSand.IntakeSand(true);

                    FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_Roar);
                    CameraManager.GetInstance().CameraShake(
                        .3f, 
                        CameraManager.ShakeDir.ROTATE, 
                        14f,
                        .035f
                    );

                    break;
                }

                /**Idle로 복귀후 대기한다...*/
                case (1):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .4f);
                    _bossCrab.SetStateTrigger(6f);
                    break;
                }

                /**모래 지옥을 파괴한다.*/
                case (2):
                {
                    if (_targetSand != null)
                        _targetSand.IntakeSand(false);

                    _bossCrab.SetStateTrigger(.5f);
                    break;
                }

                /**해당 상태를 탈출한다...*/
                case (3):
                {
                    AISM.NextPattern();
                    break;
                }
        }

        _bossCrab.StateTrigger = false;
        #endregion
    }

    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
