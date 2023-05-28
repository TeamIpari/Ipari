using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class Citizen : MonoBehaviour
{
    // State Init
    private AIStateMachine aiStateMachine;
    private AIIdleState idle;
    private AIPatrolState patrol;
    private AIBoundaryState boundary;
    private AIRunState run;

    [Header("Idle Data")]
    public float moveTimer;
    public float searchDistance;

    [Header("Patrol Data")]
    public float changeTimer;

    [Header("Boundary")]
    public float comeDistance;
    public float runDistance;


    // Start is called before the first frame update
    private void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);

        idle = new AIIdleState(aiStateMachine, moveTimer, searchDistance);
        patrol = new AIPatrolState(aiStateMachine, searchDistance);
        boundary = new AIBoundaryState(aiStateMachine, comeDistance, runDistance);
        run = new AIRunState(aiStateMachine, runDistance);

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
