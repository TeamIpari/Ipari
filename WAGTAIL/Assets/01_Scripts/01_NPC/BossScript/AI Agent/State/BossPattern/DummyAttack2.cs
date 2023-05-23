using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAttack2 : AIState
{
    float curTimer = 0;
    float changeTimer = 3;
    public DummyAttack2(AIStateMachine _stateMachine) : base(_stateMachine)
    {

        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("Start Attack2");
    }

    public override void Exit()
    {
        Debug.Log("End Attack2");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        if (curTimer > changeTimer)
        {
            if (children.Count > 0)
                stateMachine.ChangeState(children[current]);
            else if (parent != null)
                stateMachine.ChangeState(parent);
            else if (stateMachine.pattern.Count > 0)
                stateMachine.NextPattern();
            else
                Debug.Log("연결된 State가 없음.");

            curTimer = 0;
        }
    }

}
