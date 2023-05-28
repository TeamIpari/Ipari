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
    public float MoveTimer;
    public float SearchDistance;

    [Header("Patrol Data")]
    public float ChangeTimer;

    [Header("Boundary")]
    public float ComeDistance;
    public float RunDistance;


    // Start is called before the first frame update
    private void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);

        idle = new AIIdleState(aiStateMachine, MoveTimer, SearchDistance);
        patrol = new AIPatrolState(aiStateMachine, SearchDistance);
        boundary = new AIBoundaryState(aiStateMachine, ComeDistance, RunDistance);
        run = new AIRunState(aiStateMachine, RunDistance);

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
