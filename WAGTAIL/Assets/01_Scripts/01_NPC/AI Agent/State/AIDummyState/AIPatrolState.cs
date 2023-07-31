using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolState : AIState
{
    /// <summary>
    /// 일정 시간마다 IdleState에서 호출이 되며
    /// 기본적으로 Player Search와 이동을 동시에 진행하는 State
    /// 
    /// </summary>

    // 랜덤 대기 시간
    private float changeTime = 0;
    private float currentTime = 0;

    private float searchDistance = 0;
    public AIPatrolState(AIStateMachine stateMachine, float searchDistance) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.searchDistance = searchDistance;
    }
    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);
    }

    public override void Enter()
    {
        //Debug.Log("Start AI Patrol State" );
        changeTime = Random.Range(1, 5);
        //changeTime = 1;
        currentTime = 0;
    }

    public override void Exit()
    {
        //Debug.Log("End AI Patrol State");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        Change();
        Search();
    }

    private void Move()
    {
        // 이동과 관련한 기능을 삽입.
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

    // 주변 n의 방향으로 찾아줌.
    private void Search()
    {
        Collider[] cols =
            Physics.OverlapSphere(stateMachine.Transform.position, searchDistance, LayerMask.GetMask("Player"));
        // Player가 체크 되었는가?
        foreach(var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                parent.current++;   // 다음 State 진행
                stateMachine.SetTarget(c.gameObject);
                stateMachine.ChangeState(parent);
            }
        }
    }
}
