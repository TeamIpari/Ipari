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
    /// ���� �⺻�� �Ǵ� ����� RootNode
    /// IdleState�̸� ������ �ִ� ���¸� ������ ����.
    /// Search����� ������ ������ ���� � ���³Ŀ� ���� child State�� ���������� ������.
    /// </summary>

    // ���� ��� �ð�
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
            // �̵� �ð� ����
            CheckMoveTime();
            // �ǽð� ��ġ
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
            Debug.Log("State�� �������� ����.");
        }
    }

    void SearchTarget()
    {
        Collider[] cols =
            Physics.OverlapSphere(stateMachine.transform.position, 5f, LayerMask.GetMask("Player"));
        // Player�� üũ �Ǿ��°�?
        foreach (var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                current++;   // ���� State ����
                stateMachine.SetTarget(c.gameObject);
                stateMachine.ChangeState(children[current]);
            }
        }
    }


}
