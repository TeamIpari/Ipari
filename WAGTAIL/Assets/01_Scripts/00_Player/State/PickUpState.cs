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
        // TODO : ���� ��� �ð� �ִϸ����Ϳ� ���� �� ���ϱ�
        // !!! pickUpTime �ݵ�� �����ؾ��� !!! 
        // �� �ð� ���� �������� ��������.
        pickUpTime = 1.5f;      
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // ������ ��� ���� �ð� �� ���� ���¸� carry(��� �����̱�)�� �ٲ���.
        if (timePassed > pickUpTime)
        {
            player.animator.SetTrigger("carry");
            stateMachine.ChangeState(player.carry);
        }
        timePassed += Time.deltaTime;
    }
}
