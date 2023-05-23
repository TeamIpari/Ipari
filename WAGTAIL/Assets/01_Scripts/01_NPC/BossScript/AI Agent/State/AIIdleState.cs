using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using Unity.PlasticSCM.Editor.WebApi;
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

    float searchDistance = 0;
    


    public AIIdleState(AIStateMachine _stateMachine, float _moveTime,float SearchDistance) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
        movingTime = _moveTime;
        this.searchDistance = SearchDistance;
    }

    public override void Enter()
    {
        currentTime = 0;
    }

    public override void Exit()
    {
        //Debug.Log("End AI Idle State");
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
            // 0�� �ƴ� ��� �ǽð� ��ġ 0�̸� ��ġ ����� ���°�.
            if (searchDistance > 0)
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
                if (children.Count > 0)
                {
                    stateMachine.ChangeState(children[current]);
                }
                else if (parent != null)
                    stateMachine.ChangeState(parent);
                else
                    Debug.Log("����Ǿ����� ����.") ;
                currentTime = 0;
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
            Physics.OverlapSphere(stateMachine.transform.position, searchDistance, LayerMask.GetMask("Player"));
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
