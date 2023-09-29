using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class AIIdleStateDummy : AIState
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
    


    public AIIdleStateDummy (AIStateMachine stateMachine, float moveTime,float SearchDistance) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.movingTime = moveTime;
        this.searchDistance = SearchDistance;
    }

    public override void Enter()
    {
        //stateMachine.NPCBase.stateName = "Idle";
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
        if(Current == 0)
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
        if (Children.Count > 0)
        {
            AISM.ChangeState(Children[Current]);
        }
        else
            AISM.ChangeState(Parent);
    }

    private void CheckMoveTime()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > movingTime)
            {
                if (Children.Count > 0)
                {
                    AISM.ChangeState(Children[Current]);
                }
                else if (Parent != null)
                    AISM.ChangeState(Parent);
                else if (AISM.Pattern.Count > 0)
                    AISM.NextPattern();
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
            Physics.OverlapSphere(AISM.Transform.position, searchDistance, LayerMask.GetMask("Player"));
        // Player�� üũ �Ǿ��°�?
        foreach (var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                Current++;   // ���� State ����
                AISM.SetTarget(c.gameObject);
                AISM.ChangeState(Children[Current]);
            }
        }
    }


}
