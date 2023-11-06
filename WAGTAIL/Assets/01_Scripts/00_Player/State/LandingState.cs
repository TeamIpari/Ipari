using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingState : State
{
    private float _timePassed;
    private float _landingTime;

    public LandingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;         
    }

    public override void Enter()
    {
        base.Enter();

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        cVelocity = Vector3.zero;
        
        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Landed,
            player.transform.position
        );

        _timePassed = 0f;
        player.animator.SetTrigger(Landing);
        _landingTime = 0.05f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.isDead && player.movementSM.currentState != player.death)
        {
            stateMachine.ChangeState(player.death);
            return;
        }
        
        if (_timePassed > _landingTime)
        {
            if (player.isCarry) stateMachine.ChangeState(player.carry);
            else if (player.isIdle) stateMachine.ChangeState(player.idle);
        }
        _timePassed += Time.deltaTime;
    }
}