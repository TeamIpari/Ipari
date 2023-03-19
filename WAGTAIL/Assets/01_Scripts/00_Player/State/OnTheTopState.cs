using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTheTopState : State
{
    float timePassed;
    float onTheTopTime;

    public OnTheTopState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        timePassed = 0f;
        //player.animator.SetTrigger("onthetop");
        // TODO : 꼭대기 기어오르는 모션 애니메이터와 상의 후 정하기
        // !!! onTheTopTime 반드시 수정해야함 !!! 
        onTheTopTime = 1.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > onTheTopTime)
        {
            //player.animator.SetTrigger("idle");
            stateMachine.ChangeState(player.idle);
        }
        timePassed += Time.deltaTime;
    }

}
