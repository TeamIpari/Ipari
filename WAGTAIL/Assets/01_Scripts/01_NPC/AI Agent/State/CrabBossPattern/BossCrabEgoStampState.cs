using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BossCrabEgoStampState : AIAttackState
{
    //======================================
    ////            Fields              ////
    //======================================
    private EgoCrabHand handIns;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabEgoStampState(AIStateMachine stateMachine, EgoCrabHand handIns)
    : base(stateMachine)
    {
        this.handIns = handIns;
    }

    public override void Enter()
    {
        if(handIns!=null)
        {
            handIns.gameObject.SetActive(true);
            handIns.targetTransform = Player.Instance.transform;
            handIns?.StartCrabHand(AISM.character.transform.position);

            AISM.NextPattern();
        }
    }

    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
