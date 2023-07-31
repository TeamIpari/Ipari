using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniNepenthes : Enemy
{


    // Start is called before the first frame update
    void Start()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new NepenthesIdleState(AiSM);
        AiAttack = new NepenthesAttackState(AiSM);
        AiSM.Initialize(AiIdle);
        



    }

    // Update is called once per frame
    void Update()
    {
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }
}
