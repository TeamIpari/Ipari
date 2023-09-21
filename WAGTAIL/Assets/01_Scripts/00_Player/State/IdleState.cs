using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : State
{
    private Vector3 _slopeSlideVelocity;
    private GameObject _FXMove;

    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.isIdle = true;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        
        player.animator.SetTrigger(Move);
        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        _FXMove = player.MoveFX;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered) player.isJump = true;
        if (interactAction.triggered) player.Interaction();
        GetMoveInput();

        _FXMove.SetActive(input != Vector2.zero);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        player.animator.SetFloat(Speed, input.magnitude, player.speedDampTime, Time.deltaTime);

        if (player.isJump)
        {
            stateMachine.ChangeState(player.jump);
        }

        if (player.isPickup)
        {
            stateMachine.ChangeState(player.pickup);
        }

        if (player.isPull)
        {
            stateMachine.ChangeState(player.pull);
        }

        if (player.isFlight)
        {
            stateMachine.ChangeState(player.flight);
        }

        if (player.isDead)
        {
            stateMachine.ChangeState(player.death);
        }

        // TODO : Idle 상태일때 추락 후 땅에 닿았을 때 landingState호출 해주기
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
        
        _FXMove.SetActive(false);
        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Walk,
            player.transform.position
        );

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    public void Jumping()
    {
        player.isJump = true;
    }
    
    // 슬라이딩 로직
    private void SetSlopeSlideVelocity()
    {
        if (Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 5f))
        {
            var angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            
            if(angle >= player.controller.slopeLimit)
            {
                _slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, player.gravityValue * Time.deltaTime,0), hitInfo.normal);
                return;
            }
        }
        
        _slopeSlideVelocity = Vector3.zero;
    }
}
