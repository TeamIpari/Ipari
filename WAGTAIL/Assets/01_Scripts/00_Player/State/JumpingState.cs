using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    bool isGrounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;

    public JumpingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        gravityValue = player.gravityValue;
        jumpHeight = player.jumpHeight;
        playerSpeed = player.playerSpeed;
        gravityVelocity.y = 0;

        //==========================================//
        //player.animator.SetFloat("speed",0);
        //player.animator.SetTrigger("jump");
        //==========================================//
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();


        input = moveAction.ReadValue<Vector2>();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(isGrounded) 
        {
            
            stateMachine.ChangeState(player.idle);
            //stateMachine.ChangeState(player.landing);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isGrounded)
        {
            velocity = player.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            velocity.y = 0f;
            airVelocity.y = 0f;

            player.controller.Move(gravityVelocity * Time.deltaTime + (airVelocity * player.airControl + velocity * (1 - player.airControl)) * playerSpeed * Time.deltaTime);

            gravityVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = player.controller.isGrounded;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

}
