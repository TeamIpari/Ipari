using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrolState : AIState
{
    /// <summary>
    /// ���� �ð����� IdleState���� ȣ���� �Ǹ�
    /// �⺻������ Player Search�� �̵��� ���ÿ� �����ϴ� State
    /// 
    /// </summary>

    // ���� ��� �ð�
    float changeTime = 0;
    float currentTime = 0;

    float searchDistance = 0;
    public AIPatrolState(AIStateMachine _stateMachine, float searchDistance) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
        this.searchDistance = searchDistance;
    }
    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);
    }

    public override void Enter()
    {
        Debug.Log("Start AI Patrol State" );
        changeTime = Random.Range(1, 5);
        //changeTime = 1;
        currentTime = 0;
    }

    public override void Exit()
    {
        Debug.Log("End AI Patrol State");
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

    void Move()
    {
        // �̵��� ������ ����� ����.
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
            Debug.Log("parent�� �������� ����.");
        }
    }

    // �ֺ� n�� �������� ã����.
    void Search()
    {
        Collider[] cols =
            Physics.OverlapSphere(stateMachine.transform.position, searchDistance, LayerMask.GetMask("Player"));
        // Player�� üũ �Ǿ��°�?
        foreach(var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                parent.current++;   // ���� State ����
                stateMachine.SetTarget(c.gameObject);
                stateMachine.ChangeState(parent);
            }
        }
    }
}
