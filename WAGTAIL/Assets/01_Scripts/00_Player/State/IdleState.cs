using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public class IdleState : State
{
    float gravityValue;
    bool climbing;
    bool push;
    bool carry;
    bool jump;
    bool pull;
    bool flight;
    bool slide;
    Vector3 currentVelocity;
    bool isGrounded;
    float playerSpeed;
    float slopeSpeed;

    Vector3 cVelocity;

    private Vector3 hitPointNormal;
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
        jump = false;
        climbing = player.isClimbing;
        push = player.isPush;
        pull = player.isPull;
        carry = player.isCarry;
        flight = player.isFlight;
        /////
        slide = player.isSlide;
        /////
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
        slopeSpeed = player.slopeSpeed;

        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        _FXMove = player.MoveFX;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if(jumpAction.triggered)
        {
            jump = true;
        }

        climbing = player.isClimbing;
        push = player.isPush;
        carry = player.isCarry;
        pull = player.isPull;

        input = moveAction.ReadValue<Vector2>();

        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        // ========================================================
        if(input.x != 0 || input.y != 0)
        {
            _FXMove.SetActive(true);
        }

        if(input.x == 0 && input.y == 0)
        {
            _FXMove.SetActive(false);
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

        if (climbing)
        {
            stateMachine.ChangeState(player.climbing);
        }

        if (carry)
        {
            stateMachine.ChangeState(player.pickup);
        }

        if (push)
        {
            stateMachine.ChangeState(player.push);
        }

        if (jump)
        {
            stateMachine.ChangeState(player.jump);
        }

        if (pull)
        {
            stateMachine.ChangeState(player.pull);
        }

        if (flight)
        {
            stateMachine.ChangeState(player.flight);
        }

        // TODO : Idle 상태일때 추락 후 땅에 닿았을 때 landingState호출 해주기
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        // 바닥과 닿아 있을 때는 중력 적용 X
        if(isGrounded && gravityVelocity.y < 0 )
        {
            gravityVelocity.y = 0f;
        }

        else if (player.controller.velocity.y < -0.5f && !IsCheckGrounded())
        {
            //player.animator.SetTrigger("flight");
            flight = true;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);

        ////
        if (slide && IsSliding)
        {
            currentVelocity = new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;

            //Debug.Log(hitPointNormal);
        }
        ////
        ///
        player.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);
        
        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
                player.rotationDampTime);
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
        // ========================================================

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }


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
        jump = true;
    }

    private bool IsSliding
    {
        get
        {
            Debug.DrawRay(player.transform.position, Vector3.down, Color.red);
            if (player.controller.isGrounded 
                && Physics.Raycast(player.transform.position, 
                Vector3.down, out RaycastHit slopeHit, 2f, LayerMask.GetMask("Platform"))
                )
            {   
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > player.controller.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

}
