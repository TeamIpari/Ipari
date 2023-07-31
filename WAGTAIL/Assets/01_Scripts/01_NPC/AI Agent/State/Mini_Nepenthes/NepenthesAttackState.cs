using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NepenthesAttackState : AIAttackState
{
    float Timer;
    float DelayTime = 2;

    public NepenthesAttackState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        Timer = 0;
        
        //base.Enter();
    }

    public override void Exit()
    {
        //base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        //base.OntriggerEnter(other);
    }

    public bool AttackCheck()
    {
        if(DelayTime > Timer)
        {
            Timer += Time.deltaTime;
            return false;
        }
        return true;

    }

    public override void Update()
    {
        //base.Update();
        Debug.Log("공겨어어어어억!");
        if(AttackCheck())
        {
            stateMachine.character.AttackTimerReset();
            stateMachine.ChangeState(stateMachine.character.AiIdle);
        }
    }

}
