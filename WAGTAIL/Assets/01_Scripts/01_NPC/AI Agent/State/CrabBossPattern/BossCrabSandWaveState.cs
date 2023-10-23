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
        AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsSandWave);
    }

    public override void Update()
    {
        #region Omit
        base.Update();

        if(curTimer>0f)
        {
            if((curTimer-=Time.deltaTime)<=0f){

                AISM.NextPattern();
            }
        }

        /*******************************************************
         *   TODO: 추후, 꽃게 애니메이션에 의해서 발생하도록 수정예정.
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**속도를 조절한다....*/
                case (0):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**모래폭풍을 생성한다....*/
                case (1):
                {
                    curTimer = 1f;
                    _waves[--_waveLeft]?.StartWave();
                    break;
                }

                /**패턴이 마무리 되었다면 탈출대기...*/
                case (2):
                {
                    AISM.Animator.speed = 1f;
                    curTimer = 2f;
                    break;
                }
        }

        _bossCrab.StateTrigger = false;
        #endregion
    }

    public override void Exit()
    {
        AISM.Animator.ResetTrigger(BossCrabAnimation.Trigger_IsSandWave);
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
