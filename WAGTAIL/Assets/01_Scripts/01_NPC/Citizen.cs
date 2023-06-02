using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class Citizen : NPCBase
{
    // State Init
    private AIStateMachine aiStateMachine;
    private AIIdleState idle;
    private AIPatrolState patrol;
    private AIBoundaryState boundary;
    private AIRunState run;


    [Header("Patrol Data")]
    public float ChangeTimer;

    [Header("Boundary")]
    public float ComeDistance;
    public float RunDistance;


    // Start is called before the first frame update
    private void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);

        idle = new AIIdleState(aiStateMachine, NextStateTimer, SearchDistance);
        patrol = new AIPatrolState(aiStateMachine, SearchDistance);
        boundary = new AIBoundaryState(aiStateMachine, ComeDistance, RunDistance);
        run = new AIRunState(aiStateMachine, RunDistance, MoveSpeed);

        // count == 0;
        idle.SetChildren(patrol);
        // count == 1;
        idle.SetChildren(boundary);

        boundary.SetChildren(run);

        aiStateMachine.Initialize(idle);

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        aiStateMachine.CurrentState.Update();
    }
}
