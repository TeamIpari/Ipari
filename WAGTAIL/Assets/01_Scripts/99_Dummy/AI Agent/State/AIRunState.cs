using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRunState : AIState
{
    public AIRunState(AIStateMachine _stateMachine) : base(_stateMachine)
    {
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("Start AI Run State");
    }

    public override void Exit()
    {
        Debug.Log("End AI Run State");
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
        //stateMachine.agent.Move();

    }

    void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(stateMachine.transform.position,
            stateMachine.target.transform.position));
        if (distance > 4)
        {
            // Player°¡ ¹üÀ§¸¦ ¹ş¾î³².
            //stateMachine.SetTarget(null);
            //parent.current--;
            //stateMachine.pause = true;
            stateMachine.ChangeState(parent);
        }
    }
}
