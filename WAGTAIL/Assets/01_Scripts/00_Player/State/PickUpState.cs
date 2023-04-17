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
        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("pickup");
        // TODO : 물건 드는 시간 애니메이터와 상의 후 정하기
        // !!! pickUpTime 반드시 수정해야함 !!! 
        // 이 시간 이후 움직임이 가능해짐.
        pickUpTime = 1.5f;      
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // 물건을 들고 일정 시간 후 현재 상태를 carry(들고 움직이기)로 바꿔줌.
        if (timePassed > pickUpTime)
        {
            player.animator.SetTrigger("carry");
            stateMachine.ChangeState(player.carry);
        }
        timePassed += Time.deltaTime;
    }
}
