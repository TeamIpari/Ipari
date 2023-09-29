using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    private bool _isGrounded;
    private bool _jump;
    private Vector3 _airVelocity;

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
        _isGrounded = false;
        _jump = false;
        gravityVelocity.y = 0;

        player.animator.SetFloat(Speed, 0);
        if(!player.isCarry) player.animator.SetTrigger("jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        
        if (interactAction.triggered && player.currentInteractable != null)
        {
            player.Interaction();
            player.animator.SetTrigger(Throw);
        }
        GetMoveInput();
        _airVelocity = new Vector3(input.x, 0, input.y);
        var temp = player.cameraTransform.forward;
        temp.y = 0f;
        _airVelocity = _airVelocity.x * player.cameraTransform.right.normalized +
                       _airVelocity.z * temp.normalized;
        _airVelocity.y = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.isDead)
        {
            stateMachine.ChangeState(player.death);
        }
        
        if (_jump)
        {
            stateMachine.ChangeState(player.jump);
        }
        
        else if (_isGrounded) 
        {
            //stateMachine.ChangeState(player.idle);
            stateMachine.ChangeState(player.landing);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!_isGrounded)
        {
            player.controller.Move(gravityVelocity * Time.deltaTime + 
                (_airVelocity * player.airControl + velocity * (1 - player.airControl)) * (player.playerSpeed * Time.deltaTime));

            gravityVelocity.y += player.gravityValue * Time.deltaTime;
            _isGrounded = player.controller.isGrounded;

            if (_airVelocity.sqrMagnitude > 0)
            {
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(_airVelocity),
                    player.rotationDampTime);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.isJump = false;
    }

    void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(player.jumpHeight * -3.0f * player.gravityValue);
    }

    public void Jumping()
    {
        _jump = true;
    }

}
