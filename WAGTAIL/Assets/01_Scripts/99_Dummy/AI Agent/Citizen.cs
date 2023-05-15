using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    // State Init
    public AIStateMachine aiStateMachine;
    public AIIdleState idle;
    public AIPatrolState patrol;
    public AIBoundaryState boundary;
    public AIRunState run;


    //[Header("Standard")]
    public bool Search = false;


    // Start is called before the first frame update
    void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);

        idle = new AIIdleState(aiStateMachine);
        patrol = new AIPatrolState(aiStateMachine);
        boundary = new AIBoundaryState(aiStateMachine);
        run = new AIRunState(aiStateMachine);

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
