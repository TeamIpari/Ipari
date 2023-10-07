using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Profiling;
using UnityEngine;

public class PickUpState : State
{
    private float _timePassed;
    private float _pickUpTime;

    public PickUpState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        
        _timePassed = 0f;
        _pickUpTime = 0.5f;
        player.animator.SetFloat(Speed, 0);
        //player.animator.SetLayerWeight(1,1f);
        player.animator.SetTrigger(PickUp);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // ������ ��� ���� �ð� �� ���� ���¸� carry(��� �����̱�)�� �ٲ���.
        if (_timePassed > _pickUpTime)
        {
            // �߰� ��ũ��Ʈ
            //player.animator.SetTrigger(Carry);
            stateMachine.ChangeState(player.carry);
        }
        _timePassed += Time.deltaTime;
    }
    
    public override void Exit()
    {
        base.Exit();
        player.isPickup = false;
    }
}
