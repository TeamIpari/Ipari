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
        // 물건을 들고 일정 시간 후 현재 상태를 carry(들고 움직이기)로 바꿔줌.
        if (_timePassed > _pickUpTime)
        {
            // 추가 스크립트
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
