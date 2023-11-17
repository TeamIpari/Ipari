using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarryState : State
{
    public CarryState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
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
        gravityVelocity.y = 0;
        
        player.animator.SetTrigger(Move);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered) player.isJump = true;
        if (interactAction.triggered && player.currentInteractable != null)
        {
            var bomb = player.currentInteractable.GetComponent<BombObject>();
            if (bomb != null)
            {
                bomb.StopAllCoroutines();
            }
            player.Interaction();
            
            player.animator.SetLayerWeight(1, 1);
            player.animator.Play("LiftThrow", 1, 0f);
            player.animator.SetTrigger(Throw);
        }
        GetMoveInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.isDead && player.movementSM.currentState != player.death)
        {
            stateMachine.ChangeState(player.death);
            return;
        }
        
        if(player.currentInteractable == null) stateMachine.ChangeState(player.idle);
        player.animator.SetFloat(Speed, input.magnitude, player.speedDampTime, Time.deltaTime);

        if (player.isThrow)
        {
            stateMachine.ChangeState(player.idle);
        }

        if (player.isJump)
        {
            stateMachine.ChangeState(player.jump);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Movement(player.playerSpeed);
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
    }
}
