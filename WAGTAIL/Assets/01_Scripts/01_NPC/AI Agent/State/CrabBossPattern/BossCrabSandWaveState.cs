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
         *   TODO: ����, �ɰ� �ִϸ��̼ǿ� ���ؼ� �߻��ϵ��� ��������.
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**�ӵ��� �����Ѵ�....*/
                case (0):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**����ǳ�� �����Ѵ�....*/
                case (1):
                {
                    curTimer = 1f;
                    _waves[--_waveLeft]?.StartWave();
                    break;
                }

                /**������ ������ �Ǿ��ٸ� Ż����...*/
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
