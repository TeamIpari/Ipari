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
         *   ���� Ʈ���Ű� ���� ��쿡�� ���� ����...
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch (_progress++){

                /**�������� �����Ѵ�...*/
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

                /**Idle�� ������ ����Ѵ�...*/
                case (1):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .4f);
                    _bossCrab.SetStateTrigger(6f);
                    break;
                }

                /**�� ������ �ı��Ѵ�.*/
                case (2):
                {
                    if (_targetSand != null)
                        _targetSand.IntakeSand(false);

                    _bossCrab.SetStateTrigger(.5f);
                    break;
                }

                /**�ش� ���¸� Ż���Ѵ�...*/
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
