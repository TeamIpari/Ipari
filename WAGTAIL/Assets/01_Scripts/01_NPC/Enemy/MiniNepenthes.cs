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

        AttackTimer = 0;
    }


    // Update is called once per frame
    void Update()
    {
        isAttack();
        if (AiSM != null)
            AiSM.CurrentState.Update();
        
    }
}
