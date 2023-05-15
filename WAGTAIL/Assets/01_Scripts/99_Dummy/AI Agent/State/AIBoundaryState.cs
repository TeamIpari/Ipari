using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AIBoundaryState : AIState
{
    /// <summary>
    /// �����
    /// �ܰ� ���� ���� Player�� Ž���� �� ��� �������� ���߰� �÷��̾ �ֽ��ϸ� ��踦 ��.
    /// ���� ������ Player�� ���� ��� RunState�� ����.
    /// Player�� �ܰ� ������ ���� ��� parent State�� �ҷ���
    /// Player�� ���� ������ �� ��� child State�� �ҷ���.
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
        // ����� �¼�
        Debug.Log(!stateMachine.agent.isPathStale);
        if(stateMachine.target != null && !stateMachine.agent.isOnOffMeshLink) 
        {
            Boundary();
        }
        //Debug.Log("��� ��...");

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
            // Player�� ������ ���.
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
            Debug.Log("parent�� �������� ����.");
        }
    }
}
