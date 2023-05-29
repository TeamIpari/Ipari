using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class AIBoundaryState : AIState
{
    /// <summary>
    /// 경계모드
    /// 외각 범위 내에 Player가 탐색이 될 경우 움직임을 멈추고 플레이어를 주시하며 경계를 함.
    /// 내각 범위로 Player가 들어올 경우 RunState로 변함.
    /// Player가 외각 범위를 나갈 경우 parent State를 불러옴
    /// Player가 내각 범위로 들어갈 경우 child State를 불러옴.
    /// </summary>

    private float changeTime;
    private float currentTime;

    private float comeDistance;
    private float runDistance;

    public AIBoundaryState(AIStateMachine stateMachine, float comeDistance, float runDistance) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.comeDistance = comeDistance;
        this.runDistance = runDistance;
    }

    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);
    }

    public override void Enter()
    {
        //Debug.Log("Start AI Boundary State");
        
        changeTime = Random.Range(2, 5);
        currentTime = 0;
    }

    public override void Exit()
    {

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        Search();
        Boundary();
        // 경계의 태세
        //Debug.Log(!stateMachine.Agent.isPathStale);
        //Debug.Log("경계 중...");

    }

    private void Boundary()
    {
        if (stateMachine.Target != null && !stateMachine.Agent.isOnOffMeshLink)
        {
            stateMachine.Transform.LookAt(stateMachine.Target.transform);
        }


    }

    private void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(stateMachine.Transform.position,
            stateMachine.Target.transform.position));
        if (distance > comeDistance)
        {
            // Player가 범위를 벗어남.
            stateMachine.SetTarget(null);
            parent.current--;
            //stateMachine.pause = true;
            stateMachine.ChangeState(parent);
        }
        else if(distance < runDistance)
        {
            stateMachine.ChangeState(children[current]);
        }
    }

    private void Change()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > changeTime)
            {
                if (children.Count > 0)
                {
                    stateMachine.ChangeState(children[current]);
                }
                else
                    stateMachine.ChangeState(parent);
            }
        }
        catch
        {
            Debug.Log("parent가 존재하지 않음.");
        }
    }
}
