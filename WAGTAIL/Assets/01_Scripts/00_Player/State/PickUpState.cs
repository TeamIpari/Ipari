using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Profiling;
using UnityEngine;

public class PickUpState : State
{
    float timePassed;
    float pickUpTime;
    // �߰� ��ũ��Ʈ.
    //bool smallThrow;
    string animStr;

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

        // player.animator.SetTrigger("pickup");
        // TODO : ���� ��� �ð� �ִϸ����Ϳ� ���� �� ���ϱ�
        // !!! pickUpTime �ݵ�� �����ؾ��� !!! 
        // �� �ð� ���� �������� ��������.

        // �߰� ��ũ��Ʈ.
        //smallThrow = player.isSmallThrow;
        if (!/*smallThrow*/false)
        {
            player.animator.SetTrigger("pickup");
            animStr = "carry";
            pickUpTime = 1.5f;
        }
        else
        {
            player.animator.SetTrigger("pickup"); // ���� ������Ʈ �ݴ� �ִϸ��̼�
            animStr = "carry";  // ���� ������Ʈ ��� �ִ� �ִϸ��̼�
            pickUpTime = 1.5f;  // ���� ������Ʈ �ݴ� anim �ð�.
        }

        //pickUpTime = 1.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // ������ ��� ���� �ð� �� ���� ���¸� carry(��� �����̱�)�� �ٲ���.
        if (timePassed > pickUpTime)
        {
            // �߰� ��ũ��Ʈ
            player.animator.SetTrigger(animStr);

            stateMachine.ChangeState(player.carry);
        }
        timePassed += Time.deltaTime;
    }
}
