using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingState : State
{
    float timePassed;
    float landingTime;

    public LandingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;         
    }

    public override void Enter()
    {
        base.Enter();

        timePassed = 0f;
        player.animator.SetTrigger("land");
        landingTime = 0.2f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > landingTime)
        {
            // TODO : Animator Setting //
            player.animator.SetTrigger("move");
            stateMachine.ChangeState(player.idle);
        }
        timePassed += Time.deltaTime;
    }
}