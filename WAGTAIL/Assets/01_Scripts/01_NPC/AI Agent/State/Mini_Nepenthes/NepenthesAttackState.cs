using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NepenthesAttackState : AIAttackState
{
    private float Timer;
    private float DelayTime = 0.65f;
    private Vector3 LockOn;

    //===========================================
    /////           magic methods           /////
    //===========================================
    public NepenthesAttackState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    //===========================================
    /////               overrides              /////
    //===========================================
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


    public override void Update()
    {
        if (AttackCheck())
        {
            LockOn = stateMachine.Target.transform.position;
            stateMachine.character.AttackTimerReset();
            stateMachine.character.CAttack(LockOn);
            stateMachine.character.AiWait.SetNextState(stateMachine.character.AiIdle);
            stateMachine.ChangeState(stateMachine.character.AiWait);
        }
        else
        {
            LookatTarget();
        }
    }


    //===========================================
    /////           core methods           /////
    //===========================================
    private bool AttackCheck()
    {
        if (DelayTime > Timer)
        {
            Timer += Time.deltaTime;
            return false;
        }
        return true;

    }
    private void LookatTarget()
    {
        Vector3 dir = stateMachine.character.RotatePoint.position - stateMachine.Target.transform.position;
        dir.y = stateMachine.character.RotatePoint.position.y;

        Quaternion quat = Quaternion.LookRotation(dir.normalized);

        Vector3 temp = quat.eulerAngles;
        Vector3 temp2 = stateMachine.character.RotatePoint.rotation.eulerAngles;

        stateMachine.character.RotatePoint.rotation = Quaternion.Euler(temp.x, temp.y - 180f, temp2.z);

        stateMachine.ChangeState(stateMachine.character.isAttack() ? stateMachine.character.AiAttack : stateMachine.CurrentState);
    }


}
