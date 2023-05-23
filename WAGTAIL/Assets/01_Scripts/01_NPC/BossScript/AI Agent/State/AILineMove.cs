using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

public class AILineMove : AIState
{
    // 일정 방향으로 직선으로 움직이는 기능을 가지고 있음. 
    // 다수의 포인트를 가지고 있으며 랜덤으로 포인트를 지정 후
    // 목표 지점을 향해 움직이는 기능을 가짐.

    Transform[] movingPoint;
    float currentTimer = 0;
    float changeTimer = 5;
    // Start Point는 무조건 0
    int cur = 0;
    public AILineMove(AIStateMachine _stateMachine, Transform[] points) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
        movingPoint = points;
        if(movingPoint.Length < 1)
        {
            Debug.Log("이동 할 지점이 없음.");
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
                Debug.Log("연결되어있지 않음.");

            currentTimer = 0;
        }
    }

    // 목표 지점으로 이동하는 스크립트.
    void Move()
    {
        Vector3 v = movingPoint[cur].position - stateMachine.transform.position;
        stateMachine.transform.Translate(v.normalized * 1.5f * Time.deltaTime);
        //Debug.Log(movingPoint[cur].name);
        //Debug.Log(movingPoint[cur].position);
    }
}
