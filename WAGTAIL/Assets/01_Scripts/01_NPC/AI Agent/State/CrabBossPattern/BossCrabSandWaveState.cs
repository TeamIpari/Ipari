using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabSandWaveState : AIAttackState
{
    //====================================
    /////           Fields            ////
    //====================================
    private SandWave[] _waves;
    private int        _waveCount = 0;
    private int        _waveLeft  = 0;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabSandWaveState(AIStateMachine stateMachine, params GameObject[] waves)
    : base(stateMachine)
    {
        /**�����յ��� �߰��Ѵ�...*/
        int length = waves.Length;
        if( length>0 )
        {
            _waves = new SandWave[length];
            for(int i=0; i<length; i++){

                if (waves[i] == null) continue;
                _waves[_waveCount++] = waves[i].GetComponent<SandWave>();
            }
        }
    }

    public override void Enter()
    {
        _waveLeft = _waveCount;
    }

    public override void Update()
    {
        base.Update();

        /*******************************************************
         *   TODO: ����, �ɰ� �ִϸ��̼ǿ� ���ؼ� �߻��ϵ��� ��������.
         * ***/
        if ((curTimer -= Time.deltaTime) <= 0f && _waveLeft > 0)
        {
            curTimer = 1f;
            _waves[--_waveLeft]?.StartWave();

            /**��� ����ĸ� �߻����״ٸ�, ���� �������� �Ѿ��...*/
            if (_waveLeft <= 0) AISM.NextPattern();
        }
    }

    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
