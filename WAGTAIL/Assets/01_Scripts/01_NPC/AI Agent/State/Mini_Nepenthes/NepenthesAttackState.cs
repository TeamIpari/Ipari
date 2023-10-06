using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NepenthesAttackState : AIAttackState
{
    private float delayTime = 0.65f;
    private Vector3 lockOn;

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
        AISM.Animator.SetTrigger("isAttack");

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
        base.Update();
        curTimer += Time.deltaTime;
        if (delayTime > curTimer)
        {
            lockOn = AISM.Target.transform.position;
            AISM.character.AttackTimerReset();
            AISM.character.CAttack(lockOn);
            AISM.character.AiWait.SetNextState(AISM.character.AiIdle);
            AISM.ChangeState(AISM.character.AiWait);
        }
        else
        {
            LookatTarget();
        }
    }


    //===========================================
    /////           core methods           /////
    //===========================================

    private void LookatTarget()
    {
        Vector3 dir = AISM.character.RotatePoint.position - AISM.Target.transform.position;
        dir.y = AISM.character.RotatePoint.position.y;

        Quaternion quat = Quaternion.LookRotation(dir.normalized);

        Vector3 temp = quat.eulerAngles;
        Vector3 temp2 = AISM.character.RotatePoint.rotation.eulerAngles;

        AISM.character.RotatePoint.rotation = Quaternion.Euler(temp.x, temp.y - 180f, temp2.z);

        AISM.ChangeState(AISM.character.isAttack() ? AISM.character.AiAttack : AISM.CurrentState);
    }


}
