using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DummyAttack1 : AIState
{
    float curTimer = 0;
    float changeTimer = 3;

    bool on = false;
    GameObject dangerousEffect;

    public DummyAttack1(AIStateMachine _stateMachine, GameObject _danger) : base(_stateMachine)
    {
        //curTimer = 0;
        stateMachine = _stateMachine;
        dangerousEffect = _danger;
    }

    public override void Enter()
    {

        Debug.Log("Start Attack1");
    }

    //IEnumerator FlashEffect()
    //{

    //    int count = 0;
    //    while(count < 5)
    //    {
    //        Debug.Log(on);
    //        if (!on)
    //        {
    //            on = true;
    //            dangerousEffect.SetActive(false);
    //        }
    //        else
    //        {
    //            on = false;
    //            dangerousEffect.SetActive(true);
    //        }

    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}

    public override void Exit()
    {
        Debug.Log("End Attack1");

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        if(curTimer > changeTimer)
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
