using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

public class AILineMove : AIState
{
    // ���� �������� �������� �����̴� ����� ������ ����. 
    // �ټ��� ����Ʈ�� ������ ������ �������� ����Ʈ�� ���� ��
    // ��ǥ ������ ���� �����̴� ����� ����.

    Transform[] movingPoint;
    float currentTimer = 0;
    float changeTimer = 5;
    // Start Point�� ������ 0
    int cur = 0;
    public AILineMove(AIStateMachine _stateMachine, Transform[] points) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
        movingPoint = points;
        if(movingPoint.Length < 1)
        {
            Debug.Log("�̵� �� ������ ����.");
        }
        //_stateMachine
    }

    public override void Enter()
    {
        
        //Debug.Log("Start Ai Line Move State");
        int num;
        do
        { 
            num = Random.Range(0, movingPoint.Length);
        }
        while (num == cur);
        cur = num;
    }

    public override void Exit()
    {
        //Debug.Log("End Ai Line Move State");s
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        currentTimer += Time.deltaTime;
        Move();
        if (currentTimer > changeTimer)
        {
            if (children.Count > 0)
                stateMachine.ChangeState(children[current]);
            else if (parent != null)
                stateMachine.ChangeState(parent);
            else
                Debug.Log("����Ǿ����� ����.");

            currentTimer = 0;
        }
    }

    // ��ǥ �������� �̵��ϴ� ��ũ��Ʈ.
    void Move()
    {
        Vector3 v = movingPoint[cur].position - stateMachine.transform.position;
        stateMachine.transform.Translate(v.normalized * 1.5f * Time.deltaTime);
        //Debug.Log(movingPoint[cur].name);
        //Debug.Log(movingPoint[cur].position);
    }
}
