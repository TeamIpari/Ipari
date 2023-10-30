using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossCrab;

public class BossCrabSandWaveState : AIAttackState
{
    //====================================
    /////           Fields            ////
    //====================================
    private SandWave[] _waves;
    private BossCrab   _bossCrab;
    private int        _waveCount = 0;
    private int        _waveLeft  = 0;
    private int        _progress  = 0;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabSandWaveState(AIStateMachine stateMachine, BossCrab bossCrab, params GameObject[] waves)
    : base(stateMachine)
    {
        #region Omit
        _bossCrab = bossCrab; 
        /**�����յ��� �߰��Ѵ�...*/
        int length = waves.Length;
        if (!(length > 0)) return;

        /**�� �����յ�κ��� SandWave Component�� �����Ѵ�.*/
        _waves = new SandWave[length];
        for (int i = 0; i < length; i++)
        {
            if (waves[i] == null) continue;
            _waves[_waveCount++] = waves[i].GetComponent<SandWave>();
        }
        #endregion
    }

    public override void Enter()
    {
        _waveLeft = _waveCount;
        _progress = 0;
        curTimer  = 0f;
        AISM.Animator.speed = .5f;
        AISM.Animator.CrossFade(BossCrabAnimation.MakeSandWave_TongsRise, .4f);
    }

    public override void Update()
    {
        #region Omit
        base.Update();

        /*******************************************************
         *   TODO: ����, �ɰ� �ִϸ��̼ǿ� ���ؼ� �߻��ϵ��� ��������.
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**�غ��� �ִϸ��̼��� ����Ѵ�...*/
                case (-1):
                {
                    AISM.Animator.speed = (Random.Range(0f,1f)>.5f?1f:2f);
                    AISM.Animator.CrossFade(BossCrabAnimation.MakeSandWave_TongsRise, .4f);
                    break;
                }

                /**�ӵ��� �����Ѵ�....*/
                case (0):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**������ ����ģ��...*/
                case (1):
                {
                    AISM.Animator.speed = 2f;
                    AISM.Animator.CrossFade(BossCrabAnimation.MakeSandWave_Smash, .3f);
                    break;
                }

                /**����ǳ�� �����Ѵ�....*/
                case (2):
                {
                    FModEventInstance waveIns = FModAudioManager.CreateInstance(FModSFXEventType.Crab_SandWave);
                    waveIns.Volume = 2f;
                    waveIns.Play();
                    FModAudioManager.ApplyInstanceFade(waveIns, 0f, 2f, 0, true);

                    FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_Smash);
                    CameraManager.GetInstance().CameraShake(.4f,CameraManager.ShakeDir.ROTATE,.5f);

                    _waves[--_waveLeft]?.StartWave();

                    if(_waveLeft>0)
                    {
                        AISM.Animator.speed = 0f;
                        _progress = -1;
                        _bossCrab.SetStateTrigger(Random.Range(.1f, .2f));
                    }
                    break;
                }

                /**������ ������ �Ǿ��ٸ� */
                case (3):
                {
                    AISM.Animator.speed = 1f;
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .3f);

                    _bossCrab.SetStateTrigger(1f);
                    break;
                }

                /*���� �������� �Ѿ��....*/
                case (4):
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
