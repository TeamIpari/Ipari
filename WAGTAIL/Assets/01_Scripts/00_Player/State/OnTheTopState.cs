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
        // TODO : ����� �������� ��� �ִϸ����Ϳ� ���� �� ���ϱ�
        // !!! onTheTopTime �ݵ�� �����ؾ��� !!! 
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
