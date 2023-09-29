using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabSandWaveState : AIAttackState
{
    //====================================
    /////           Fields            ////
    //====================================
    private SandWave wave;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabSandWaveState(AIStateMachine stateMachine)
    : base(stateMachine)
    {
        wave = AISM.character.GetComponent<SandWave>();
    }

    public override void Enter()
    {
        wave?.StartWave();
        AISM.NextPattern();
    }

    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
