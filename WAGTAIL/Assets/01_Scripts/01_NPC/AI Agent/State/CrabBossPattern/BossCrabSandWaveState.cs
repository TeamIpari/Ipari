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
        /**프리팹들을 추가한다...*/
        int length = waves.Length;
        if (!(length > 0)) return;

        /**각 프리팹들로부터 SandWave Component를 추출한다.*/
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
         *   TODO: 추후, 꽃게 애니메이션에 의해서 발생하도록 수정예정.
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**준비동작 애니메이션을 재생한다...*/
                case (-1):
                {
                    AISM.Animator.speed = (Random.Range(0f,1f)>.5f?1f:2f);
                    AISM.Animator.CrossFade(BossCrabAnimation.MakeSandWave_TongsRise, .4f);
                    break;
                }

                /**속도를 조절한다....*/
                case (0):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**빠르게 내려친다...*/
                case (1):
                {
                    AISM.Animator.speed = 2f;
                    AISM.Animator.CrossFade(BossCrabAnimation.MakeSandWave_Smash, .3f);
                    break;
                }

                /**모래폭풍을 생성한다....*/
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

                /**패턴이 마무리 되었다면 */
                case (3):
                {
                    AISM.Animator.speed = 1f;
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .3f);

                    _bossCrab.SetStateTrigger(1f);
                    break;
                }

                /*다음 패턴으로 넘어간다....*/
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
