using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NepenthesAttackState : AIAttackState
{
    float Timer;
    float DelayTime = 0.65f;

    public NepenthesAttackState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        Timer = 0;

        stateMachine.Animator.SetTrigger("isAttack");
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
        if (AttackCheck())
        {
            stateMachine.character.AttackTimerReset();
            stateMachine.character.CAttack();
            stateMachine.character.AiWait.SetNextState(stateMachine.character.AiIdle);
            stateMachine.ChangeState(stateMachine.character.AiWait);
        }
    }

}
