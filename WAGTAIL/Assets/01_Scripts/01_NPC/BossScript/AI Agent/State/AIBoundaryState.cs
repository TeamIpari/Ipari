using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class AIBoundaryState : AIState
{
    /// <summary>
    /// �����
    /// �ܰ� ���� ���� Player�� Ž���� �� ��� �������� ���߰� �÷��̾ �ֽ��ϸ� ��踦 ��.
    /// ���� ������ Player�� ���� ��� RunState�� ����.
    /// Player�� �ܰ� ������ ���� ��� parent State�� �ҷ���
    /// Player�� ���� ������ �� ��� child State�� �ҷ���.
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
        // ����� �¼�
        //Debug.Log(!stateMachine.Agent.isPathStale);
        //Debug.Log("��� ��...");

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
            Debug.Log("parent�� �������� ����.");
        }
    }
}
