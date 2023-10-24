using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabAntHellState : AIAttackState
{
    //========================================
    //////          Property            //////
    //========================================
    private SandScriptBase _targetSand;
    private float          _duration = 0f;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabAntHellState(AIStateMachine stateMachine, GameObject andhellPrefab, float duration)
    : base(stateMachine)
    {
        #region Omit
        _duration = duration;

        if (andhellPrefab != null){

            _targetSand = andhellPrefab.GetComponent<SandScriptBase>();
        }
        #endregion
    }

    public override void Enter()
    {
        #region Omit
        AISM.Animator.SetTrigger("");
        curTimer = 0f;
        
        if(_targetSand!=null)
        {
            #region Deprecate
            //_targetSand.SandIntakeCenterOffset.y = -7f;
            //_targetSand.SandIdleCenterOffset = _targetSand.SandIntakeCenterOffset;
            //_targetSand.SandIdleCenterOffset.y = -4.5f;
            #endregion

            _targetSand.IntakeSand(true);
        }
        #endregion
    }

    public override void Update()
    {
        #region Omit
        base.Update();

        curTimer += Time.deltaTime;
        if(curTimer > _duration) {

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
