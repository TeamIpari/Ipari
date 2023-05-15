using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Rendering;

public class AIIdleState : AIState
{
    /// <summary>
    /// 가장 기본이 되는 어찌보면 RootNode
    /// IdleState이며 가만히 있는 상태를 가지고 있음.
    /// Search기능을 가지고 있으며 현재 어떤 상태냐에 따라 child State를 선택적으로 가져옴.
    /// </summary>

    // 랜덤 대기 시간
    float movingTime = 0;
    float currentTime = 0;
    


    public AIIdleState(AIStateMachine _stateMachine) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        movingTime = Random.Range(2, 5);
        //movingTime = 1;
        currentTime = 0;
        Debug.Log("Start AI Idle State");
    }

    public override void Exit()
    {
        Debug.Log("End AI Idle State");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);

    }

    public override void Update()
    {
        if(current == 0)
        {
            // 이동 시간 결정
            CheckMoveTime();
            // 실시간 서치
            SearchTarget();
        }
        else
        {
            ChangeState();
        }
    }

    void ChangeState()
    {
        if (children.Count > 0)
        {
            stateMachine.ChangeState(children[current]);
        }
        else
            stateMachine.ChangeState(parent);
    }

    void CheckMoveTime()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > movingTime)
            {
                if (children.Count > 0 )
                {
                    stateMachine.ChangeState(children[current]);
                }
                else
                    stateMachine.ChangeState(parent);
            }
        }
        catch
        {
            Debug.Log("State가 존재하지 않음.");
        }
    }

    void SearchTarget()
    {
        Collider[] cols =
            Physics.OverlapSphere(stateMachine.transform.position, 5f, LayerMask.GetMask("Player"));
        // Player가 체크 되었는가?
        foreach (var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                current++;   // 다음 State 진행
                stateMachine.SetTarget(c.gameObject);
                stateMachine.ChangeState(children[current]);
            }
        }
    }


}
