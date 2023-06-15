using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// �ۼ��� : ������
/// �߰� �ۼ�
/// </summary>
public class AIIdleState : AIState
{
    /// <summary>
    /// ���� �⺻�� �Ǵ� ����� RootNode
    /// IdleState�̸� ������ �ִ� ���¸� ������ ����.
    /// Search����� ������ ������ ���� � ���³Ŀ� ���� child State�� ���������� ������.
    /// </summary>

    // ���� ��� �ð�
    private float movingTime = 0;
    private float currentTime = 0;

    private float searchDistance = 0;
    


    public AIIdleState (AIStateMachine stateMachine, float moveTime,float SearchDistance) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.movingTime = moveTime;
        this.searchDistance = SearchDistance;
    }

    public override void Enter()
    {
        stateMachine.NPCBase.stateName = "Idle";
        currentTime = 0;
    }

    public override void Exit()
    {
        //stateMachine.NPCBase.stateName = "None";
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

    private void ChangeState()
    {
        if (children.Count > 0)
        {
            stateMachine.ChangeState(children[current]);
        }
        else
            stateMachine.ChangeState(parent);
    }

    private void CheckMoveTime()
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
                else if (stateMachine.Pattern.Count > 0)
                    stateMachine.NextPattern();
                else
                     ;
                currentTime = 0;
            }
        }
        catch
        {
            Debug.Log("State�� �������� ����.");
        }
    }

    private void SearchTarget()
    {
        Collider[] cols =
            Physics.OverlapSphere(stateMachine.Transform.position, searchDistance, LayerMask.GetMask("Player"));
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
