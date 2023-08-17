using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightState : State
{
    private float gravityValue;
    private bool jump;
    private bool dead;
    private bool isGrounded;
    private float playerSpeed;
    private Vector3 airVelocity;
    private Vector3 cVelocity;
    
    public FlightState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        jump = false;

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
        gravityVelocity.y = 0;
        
        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("flight");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered)
        {
            jump = true;
        }

        dead = player.isDead;
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * player.cameraTransform.right.normalized +
                   velocity.z * player.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (dead)
        {
            stateMachine.ChangeState(player.death);
        }

        if (jump)
        {
            stateMachine.ChangeState(player.jump);
        }
        
        else if (isGrounded)
        {
            stateMachine.ChangeState(player.landing);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isGrounded)
        {
            velocity = player.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);
            
            velocity = velocity.x * player.cameraTransform.right.normalized + 
                       velocity.z * player.cameraTransform.forward.normalized;
            velocity.y = 0f;

            airVelocity = airVelocity.x * player.cameraTransform.right.normalized +
                          airVelocity.z * player.cameraTransform.forward.normalized;
            airVelocity.y = 0;

            player.controller.Move(gravityVelocity * Time.deltaTime + 
                                   (airVelocity * player.airControl + velocity * (1 - player.airControl)) * playerSpeed * Time.deltaTime);

            gravityVelocity.y += gravityValue * Time.deltaTime;
            isGrounded = player.controller.isGrounded;

            if (airVelocity.sqrMagnitude > 0)
            {
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(airVelocity),
                    player.rotationDampTime);
            }
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    public void Jumping()
    {
        jump = true;
    }
}