using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class NepenthesIdleState : AIIdleState
{
    int DetectionAngle;
    //float RadianRange;
    bool isSearch ;
    //Vector3 DefualtRot;
    public NepenthesIdleState(AIStateMachine stateMachine, int angle) : base(stateMachine)
    {
        //DetectionAngle = angle;
        //RadianRange = Mathf.Cos((DetectionAngle / 2) * Mathf.Deg2Rad);
        //DefualtRot = stateMachine.character.RotatePoint.rotation.eulerAngles;
        
    }

    public override void Enter()
    {
        base.Enter();
        //isSearch = Search();
        //isSearch = false;
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
        Collider[] cols = Physics.OverlapSphere(stateMachine.Transform.position, stateMachine.character.AttackRange);


        // Layer가 Player인 캐릭터를 서치 
        if(cols.Length  > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                float targetRadian = Vector3.Dot(stateMachine.Transform.forward, (cols[i].transform.position - stateMachine.Transform.position).normalized);

                
                if (/*targetRadian > RadianRange &&*/ cols[i].CompareTag("Player"))
                {
                    stateMachine.Target = cols[i].gameObject;
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
            if(Vector3.Distance(stateMachine.Transform.position, stateMachine.Target.transform.position) > stateMachine.character.AttackRange)
            {
                isSearch = false;
                return;
            }
            
            Vector3 dir = stateMachine.character.RotatePoint.position - stateMachine.Target.transform.position;
            dir.y = stateMachine.character.RotatePoint.position.y;
            
            Quaternion quat = Quaternion.LookRotation(dir.normalized);

            Vector3 temp = quat.eulerAngles;
            Vector3 temp2 = stateMachine.character.RotatePoint.rotation.eulerAngles;

            stateMachine.character.RotatePoint.rotation = Quaternion.Euler(temp.x, temp.y - 180f, temp2.z);

            stateMachine.ChangeState(stateMachine.character.isAttack() ? stateMachine.character.AiAttack : stateMachine.CurrentState);
        }
        else
        {
            Search();
            //stateMachine.character.RotatePoint.rotation = stateMachine.character.RotatePoint.rotation == Quaternion.Euler(DefualtRot) ? stateMachine.character.RotatePoint.rotation : Quaternion.Euler(DefualtRot);
            //stateMachine.character.RotatePoint.rotation = Quaternion.Euler(0f, 0f, 0f);
            //stateMachine.character.RotatePoint.transform.rotation;
        }

    }
}
