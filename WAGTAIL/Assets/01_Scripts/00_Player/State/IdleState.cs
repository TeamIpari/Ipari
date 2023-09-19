using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : State
{
    private float _gravityValue;
    private bool _climbing;
    private bool _push;
    private bool _carry;
    private bool _jump;
    private bool _pull;
    private bool _flight;
    private bool _slide;
    private bool _dead;
    private Vector3 _currentVelocity;
    private bool _isGrounded;
    private float _playerSpeed;
    private float _slopeSpeed;

    private bool _isSliding;
    private Vector3 _slopeSlideVelocity;

    private Vector3 _cVelocity;
    private GameObject _FXMove;

    // 슬라이딩 관련 변수
    private Vector3 hitPointNormal;

    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.isIdle = true;
        _jump = false;
        _climbing = player.isClimbing;
        _push = player.isPush;
        _pull = player.isPull;
        _carry = player.isCarry;
        _flight = player.isFlight;
        _dead = player.isDead;
        
        //slide = player.isSlide;
        
        input = Vector2.zero;
        velocity = Vector3.zero;
        _currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        _playerSpeed = player.playerSpeed;
        _isGrounded = player.controller.isGrounded;
        _gravityValue = player.gravityValue;
        _slopeSpeed = player.slopeSpeed;

        // trigger 초기화
        player.animator.ResetTrigger("flight");


        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        _FXMove = player.MoveFX;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered) _jump = true;
        if (interactAction.triggered) player.Interaction();
        

        _climbing = player.isClimbing;
        _push = player.isPush;
        _carry = player.isCarry;
        _pull = player.isPull;
        _dead = player.isDead;

        input = moveAction.ReadValue<Vector2>();
        
        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        // ========================================================
        if(input.x != 0 || input.y != 0)
        {
            _FXMove.SetActive(true);
            //player.SoundHandler.SetBool("isWalk",true);
        }

        if(input.x == 0 && input.y == 0)
        {
            _FXMove.SetActive(false);
            //player.SoundHandler.SetBool("isWalk",false);
        }
        // ========================================================
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // TODO : animator 적용
        player.animator.SetFloat("speed", input.magnitude, player.speedDampTime, Time.deltaTime);

        if (_climbing)
        {
            stateMachine.ChangeState(player.climbing);
        }

        if (_carry)
        {
            stateMachine.ChangeState(player.pickup);
        }

        if (_push)
        {
            stateMachine.ChangeState(player.push);
        }

        if (_jump)
        {
            stateMachine.ChangeState(player.jump);
        }

        if (_pull)
        {
            stateMachine.ChangeState(player.pull);
        }

        if (_flight)
        {
            stateMachine.ChangeState(player.flight);
        }

        if (_dead)
        {
            stateMachine.ChangeState(player.death);
        }

        // TODO : Idle 상태일때 추락 후 땅에 닿았을 때 landingState호출 해주기
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        gravityVelocity.y += _gravityValue * Time.deltaTime;
        _isGrounded = player.controller.isGrounded;

        // 바닥과 닿아 있을 때는 중력 적용 X
        if(_isGrounded && gravityVelocity.y < 0 )
        {
            gravityVelocity.y = 0f;
        }

        else if (player.controller.velocity.y < -0.5f && !IsCheckGrounded())
        {
            _flight = true;
        }
        
        // Movement Logic
        velocity = AdjustVelocityToSlope(velocity); // 경사로 내려갈 때 velocity 조정
        _currentVelocity = Vector3.SmoothDamp(_currentVelocity, velocity, ref _cVelocity, player.velocityDampTime);
        player.controller.Move(_currentVelocity * (Time.deltaTime * _playerSpeed) + gravityVelocity * Time.deltaTime);
        
        // Player Rotation 선형보간
        if (velocity.sqrMagnitude > 0)
        {
            var rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
                player.rotationDampTime);
            rotation.x = 0f;
            rotation.z = 0f;
            player.transform.rotation = rotation;
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.isIdle = false;
        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        // ========================================================
        _FXMove.SetActive(false);
        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Walk,
            player.transform.position
        );

        //player.SoundHandler.SetBool("isWalk",false);
        // ========================================================

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    // 바닥 체크
    private bool IsCheckGrounded()
    {
        //if (isGrounded) return true;

        var ray = new Ray(player.transform.position + Vector3.up * 0.1f, Vector3.down);

        var maxDistance = 1.5f;
        
        Debug.DrawRay(player.transform.position + Vector3.up * 0.1f, Vector3.down * maxDistance);

        return Physics.Raycast(ray, maxDistance);
    }

    public void Jumping()
    {
        _jump = true;
    }
    
    // 경사로 내려갈 때 velocity 조정
    private Vector3 AdjustVelocityToSlope(Vector3 vel)
    {
        var ray = new Ray(player.transform.position, Vector3.down);
        
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * vel;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return vel;
    }
    
    // 슬라이딩 로직
    private void SetSlopeSlideVelocity()
    {
        if (Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 5f))
        {
            var angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            
            if(angle >= player.controller.slopeLimit)
            {
                _slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _gravityValue * Time.deltaTime,0), hitInfo.normal);
                return;
            }
        }
        
        _slopeSlideVelocity = Vector3.zero;
    }
}
