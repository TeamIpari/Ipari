using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NepenthesAttackState : AIAttackState
{
    private float delayTime = 0.65f;
    private Vector3 lockOn;
    private AIState NextState;

    //===========================================
    /////           magic methods           /////
    //===========================================
    public NepenthesAttackState(AIStateMachine stateMachine, AIState nextState = null) : base(stateMachine)
    {
        NextState = nextState;
        if(NextState == null)
        {
            AISM.Target = GameObject.Instantiate(new GameObject(), AISM.Transform.position + AISM.Transform.forward * 5f, Quaternion.identity);
            AISM.Target.name = "View Target";
        }
    }

    //===========================================
    /////               overrides              /////
    //===========================================

    public override void Enter()
    {
        curTimer = 0;
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
        //base.Update();
        curTimer += Time.deltaTime;
        if (delayTime < curTimer)
        {
            lockOn = AISM.Target.transform.position;
            AISM.character.AttackTimerReset();
            AISM.character.CAttack(lockOn);
            AISM.character.AiWait.SetNextState(NextState == null ? this : NextState);
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
        Vector3 dir;
        dir = AISM.character.RotatePoint.position - AISM.Target.transform.position;

        dir.y = AISM.character.RotatePoint.position.y;

        Quaternion quat = Quaternion.LookRotation(dir.normalized);

        Vector3 temp = quat.eulerAngles;
        Vector3 temp2 = AISM.character.RotatePoint.rotation.eulerAngles;
        
        if(NextState != null)   
            AISM.character.RotatePoint.rotation = Quaternion.Euler(temp.x, temp.y - 180f, temp2.z);

        //if(AISM.character.isAttack())
        //    AISM.ChangeState(AISM.character.AiAttack);
    }


}
