using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class NepenthesIdleState : AIIdleState
{
    bool isSearch ;
    public NepenthesIdleState(AIStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        isSearch = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        base.OntriggerEnter(other);
    }

    private bool Search()
    {
        // 원 범위 내에 플레이어가 들어오면 바라보고 일정 주기가 지나면 Attack
        Collider[] cols = Physics.OverlapSphere(stateMachine.Transform.position, 5.0f);

        // Layer가 Player인 캐릭터를 서치 
        if(cols.Length  > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].CompareTag("Player"))
                {
                    Target = cols[i].gameObject;
                    isSearch = true;
                    return true;
                }
            }
        }
        return false;
    }


    public override void Update()
    {
        base.Update();
        if (isSearch)
        {
            stateMachine.Transform.LookAt(new Vector3(Target.transform.position.x, stateMachine.Transform.position.y, Target.transform.position.z));
            stateMachine.ChangeState(stateMachine.character.isAttack() ? stateMachine.character.AiAttack : stateMachine.CurrentState);
        }
        else
        {
            Search();
        }
       
    }
}
