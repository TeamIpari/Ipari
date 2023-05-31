using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRunState : AIState
{
    private Vector3 destination;
    private float runDistance;

    public AIRunState (AIStateMachine stateMachine, float runDistance) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.runDistance = runDistance + 2;
    }

    public override void Enter()
    {
        //Debug.Log("Start AI Run State");
        destination = new Vector3(stateMachine.Transform.position.x - stateMachine.Target.transform.position.x, 0f,
            stateMachine.Transform.position.z - stateMachine.Target.transform.position.z).normalized;

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

    private void Run()
    {
        // µµ¸ÁÃÄ!
        //Debug.Log("µ¼È²Ã­!");
        stateMachine.Agent.SetDestination(stateMachine.Transform.position + destination * 5f);

    }

    private void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(stateMachine.Transform.position,
            stateMachine.Target.transform.position));
        if (distance > runDistance)
        {
            
            stateMachine.ChangeState(parent);
        }
    }
}
