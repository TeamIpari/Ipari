using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpState : State
{
    float timePassed;
    float pickUpTime;

    public PickUpState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        timePassed = 0f;
        //player.animator.SetTrigger("PickUp");
        // TODO : ���� ��� �ð� �ִϸ����Ϳ� ���� �� ���ϱ�
        // !!! pickUpTime �ݵ�� �����ؾ��� !!! 
        pickUpTime = 1.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > pickUpTime)
        {
            //player.animator.SetTrigger("Carry");
            stateMachine.ChangeState(player.carry);
        }
        timePassed += Time.deltaTime;
    }
}
