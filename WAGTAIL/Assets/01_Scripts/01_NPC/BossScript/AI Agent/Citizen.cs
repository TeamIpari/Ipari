using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    // State Init
    AIStateMachine aiStateMachine;
    AIIdleState idle;
    AIPatrolState patrol;
    AIBoundaryState boundary;
    AIRunState run;

    [Header("Idle Data")]
    public float moveTimer;
    public float searchDistance;

    [Header("Patrol Data")]
    public float changeTimer;

    [Header("Boundary")]
    public float comeDistance;
    public float runDistance;

    //[Header("Run")]
    //public float runCancelDistance;
    //[Header("Standard")]
    //public bool Search = false;



    // Start is called before the first frame update
    void Start()
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
    void FixedUpdate()
    {
        aiStateMachine.currentState.Update();
    }
}
