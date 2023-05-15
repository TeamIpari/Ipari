using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AIBoundaryState : AIState
{
    /// <summary>
    /// 경계모드
    /// 외각 범위 내에 Player가 탐색이 될 경우 움직임을 멈추고 플레이어를 주시하며 경계를 함.
    /// 내각 범위로 Player가 들어올 경우 RunState로 변함.
    /// Player가 외각 범위를 나갈 경우 parent State를 불러옴
    /// Player가 내각 범위로 들어갈 경우 child State를 불러옴.
    /// </summary>

    public float changeTime;
    public float currentTime;

    public float comeDistance;
    public float runDistance;

    public AIBoundaryState(AIStateMachine _stateMachine, float comeDistance, float runDistance) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
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
        //Debug.Log("End AI Boundary State");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        Search();
        // 경계의 태세
        Debug.Log(!stateMachine.agent.isPathStale);
        if(stateMachine.target != null && !stateMachine.agent.isOnOffMeshLink) 
        {
            Boundary();
        }
        //Debug.Log("경계 중...");

    }

    void Boundary()
    {
        
        stateMachine.transform.LookAt(stateMachine.target.transform);

    }

    void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(stateMachine.transform.position,
            stateMachine.target.transform.position));
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

    void Change()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > changeTime)
            {
                if (children.Count > 0)
                {
                    //if (children.Count <= current)
                    //{
                    //    Debug.Log("Out of range");
                    //    current -= 1;
                    //}
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
