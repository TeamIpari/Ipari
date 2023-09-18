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
    private float changeTime = 0;
    private float currentTime = 0;

    private float searchDistance = 0;
    public AIPatrolState(AIStateMachine stateMachine, float searchDistance) : base(stateMachine)
    {
        this.AISM = stateMachine;
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
        // �̵��� ������ ����� ����.
    }

    private void Change()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > changeTime)
            {
                if (Children.Count > 0)
                {
                    //if (children.Count <= current)
                    //{
                    //    Debug.Log("Out of range");
                    //    current -= 1;
                    //}
                    AISM.ChangeState(Children[Current]);
                }
                else
                    AISM.ChangeState(Parent);
            }
        }
        catch
        {
            Debug.Log("parent�� �������� ����.");
        }
    }

    // �ֺ� n�� �������� ã����.
    private void Search()
    {
        Collider[] cols =
            Physics.OverlapSphere(AISM.Transform.position, searchDistance, LayerMask.GetMask("Player"));
        // Player�� üũ �Ǿ��°�?
        foreach(var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                Parent.Current++;   // ���� State ����
                AISM.SetTarget(c.gameObject);
                AISM.ChangeState(Parent);
            }
        }
    }
}
