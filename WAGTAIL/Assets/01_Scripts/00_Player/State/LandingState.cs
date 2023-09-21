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

        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Landed,
            player.transform.position
        );

        _timePassed = 0f;
        player.animator.SetTrigger("land");
        _landingTime = 0.05f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (_timePassed > _landingTime)
        {
            if (player.isIdle) stateMachine.ChangeState(player.idle);
            else if (player.isCarry) stateMachine.ChangeState(player.carry);
        }
        _timePassed += Time.deltaTime;
    }
}