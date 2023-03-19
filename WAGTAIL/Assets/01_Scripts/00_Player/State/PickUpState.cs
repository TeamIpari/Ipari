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
        // TODO : 물건 드는 시간 애니메이터와 상의 후 정하기
        // !!! pickUpTime 반드시 수정해야함 !!! 
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
