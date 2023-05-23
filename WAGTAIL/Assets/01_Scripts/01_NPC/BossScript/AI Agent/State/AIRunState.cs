using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRunState : AIState
{
    Vector3 destination;
    float runDistance;

    public AIRunState(AIStateMachine _stateMachine, float runDistance) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
        this.runDistance = runDistance + 2;
    }

    public override void Enter()
    {
        //Debug.Log("Start AI Run State");
        destination = new Vector3(stateMachine.transform.position.x - stateMachine.target.transform.position.x, 0f,
            stateMachine.transform.position.z - stateMachine.target.transform.position.z).normalized;

    }

    public override void Exit()
    {
        //Debug.Log("End AI Run State");
    }

    public override void OntriggerEnter(Collider other)
    {

    }

    public override void Update()
    {
        Run();
        Search();
    }

    void Run()
    {
        // µµ¸ÁÃÄ!
        //Debug.Log("µ¼È²Ã­!");
        stateMachine.agent.SetDestination(stateMachine.transform.position + destination * 5f);

    }

    void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(stateMachine.transform.position,
            stateMachine.target.transform.position));
        if (distance > runDistance)
        {
            
            stateMachine.ChangeState(parent);
        }
    }
}
