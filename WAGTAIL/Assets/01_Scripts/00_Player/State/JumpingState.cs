using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    bool isGrounded;

    bool jump;
    private bool dead;
    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;

    public JumpingState(Player player, StateMachine stateMachine) : base(player, stateMachine)
    {
        base.player = player;
        base.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Player_Jump);
        //player.SoundHandler.SetTrigger("isJump");
        isGrounded = false;
        jump = false;
        gravityValue = player.gravityValue;
        jumpHeight = player.jumpHeight;
        playerSpeed = player.playerSpeed;
        gravityVelocity.y = 0;

        //AnimManager.Instance.AnimFloat("speed");
        player.animator.SetFloat("speed", 0);
        //AnimManager.Instance.AnimTrigger("jump");
        player.animator.SetTrigger("jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        dead = player.isDead;
        input = moveAction.ReadValue<Vector2>();
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
            //stateMachine.ChangeState(player.idle);
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

    void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public void Jumping()
    {
        jump = true;
    }

}
