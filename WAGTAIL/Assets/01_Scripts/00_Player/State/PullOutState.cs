using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullOutState : State
{
    float timePassed;
    float dropTime;

    public PullOutState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        timePassed = 0f;
        player.animator.SetTrigger("pullout");
        player.currentInteractable.GetComponent<Pulling>().Drop();
        // TODO : ���� �������� �ð� �ִϸ����Ϳ� ���� �� ���ϱ�
        // !!! dropTime �ݵ�� �����ؾ��� !!! 
        dropTime = 1.5f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > dropTime)
        {
            player.animator.SetTrigger("move");
            stateMachine.ChangeState(player.idle);
        }
        timePassed += Time.deltaTime;
    }
}
