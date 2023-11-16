using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightState : State
{
    private float _gravityValue;
    private bool _jump;
    private bool _dead;
    private bool _isGrounded;
    private float _playerSpeed;
    private Vector3 _airVelocity;
    private Vector3 _cVelocity;
    
    public FlightState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        _isGrounded = false;
        _jump = false;

        _playerSpeed = player.playerSpeed;
        _isGrounded = player.controller.isGrounded;
        _gravityValue = player.gravityValue;
        gravityVelocity.y = 0;

        player.animator.SetFloat(Speed, 0);
        player.animator.SetTrigger(Flight);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered)
        {
            _jump = true;
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
        
        if (player.isDead && player.movementSM.currentState != player.death)
        {
            stateMachine.ChangeState(player.death);
            return;
        }
        
        if (_jump)
        {
            stateMachine.ChangeState(player.jump);
        }
        
        else if (_isGrounded)
        {
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
        player.isFlight = false;
    }
    
    public void Jumping()
    {
        _jump = true;
    }
}