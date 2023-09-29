using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabAntHellState : AIAttackState
{
    //========================================
    //////          Property            //////
    //========================================
    private SandScript _targetSand;
    private float      _currTime = 0f;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabAntHellState(AIStateMachine stateMachine, GameObject andhellPrefab)
    : base(stateMachine)
    {
        #region Omit
        if (andhellPrefab != null){

            _targetSand = andhellPrefab.GetComponent<SandScript>();
        }
        #endregion
    }

    public override void Enter()
    {
        #region Omit
        AISM.Animator.SetTrigger("");
        _currTime = 0f;
        
        if(_targetSand!=null)
        {
            _targetSand.SandCenterIntakeOffset.y = -7f;

            _targetSand.SandCenterIdleOffset = _targetSand.SandCenterIntakeOffset;
            _targetSand.SandCenterIdleOffset.y = -4.5f;

            _targetSand.IntakeSand(true);
        }
        #endregion
    }

    public override void Update()
    {
        #region Omit
        base.Update();

        _currTime += Time.deltaTime;
        if(_currTime>5f) {

            _targetSand?.IntakeSand(false);
            AISM.NextPattern();
        }
        #endregion
    }

    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
