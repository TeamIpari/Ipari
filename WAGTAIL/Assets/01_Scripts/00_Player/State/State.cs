using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class State
{
    protected Player player;
    protected StateMachine stateMachine;

    public Vector3 gravityVelocity;
    public Vector3 velocity;
    public Vector3 currentVelocity;
    public Vector3 cVelocity;
    protected Vector2 input;
    
    protected readonly InputAction moveAction;
    protected readonly InputAction climbingAction;
    protected readonly InputAction jumpAction;
    protected readonly InputAction interactAction;
    protected readonly InputAction pushZAxisAction; // 상하
    protected readonly InputAction pushXAxisAction; // 좌우
    
    // 애니메이션 관련 Parameters StringToHash
    protected static readonly int Speed = Animator.StringToHash("speed");
    // Animator Parameter Trigger StringToHash
    protected static readonly int Move = Animator.StringToHash("move");
    protected static readonly int Jumping = Animator.StringToHash("jump");
    protected static readonly int Flight = Animator.StringToHash("flight");
    protected static readonly int Landing = Animator.StringToHash("land");
    protected static readonly int PickUp = Animator.StringToHash("pickup");
    protected static readonly int Carry = Animator.StringToHash("carry");
    protected static readonly int Throw = Animator.StringToHash("throw");

    // 생성자
    public State(Player _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;

        //PlayerInput
        moveAction = player.playerInput.actions["Move"];
        jumpAction = player.playerInput.actions["jump"];
        interactAction = player.playerInput.actions["Interaction"];
        climbingAction = player.playerInput.actions["Climbing"];
        pushXAxisAction = player.playerInput.actions["MoveXAxis"];
        pushZAxisAction = player.playerInput.actions["MoveZAxis"];
    }
    
    #region MovementLogic
    protected void Movement(float playerSpeed)
    {
        gravityVelocity.y += player.gravityValue * Time.deltaTime;
        
        // 바닥과 닿아 있을 때는 중력 적용 X
        if (player.controller.isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }
        
        else if (player.controller.velocity.y < -0.5f && !IsCheckGrounded())
        {
            player.isFlight = true;
        }
        
        // Slope 조정
        velocity = AdjustVelocityToSlope(velocity);
        // Movement Logic
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(currentVelocity * (Time.deltaTime * playerSpeed) + gravityVelocity * Time.deltaTime);
        
        // Player Rotation 보간
        if (velocity.sqrMagnitude > 0)
        {
            var rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
                player.rotationDampTime);
            rotation.x = 0f;
            rotation.z = 0f;
            player.transform.rotation = rotation;
            
        }
    }
    
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
    
    private bool IsCheckGrounded()
    {
        //if (isGrounded) return true;

        var ray = new Ray(player.transform.position + Vector3.up * 0.1f, Vector3.down);

        var maxDistance = 1.5f;
        
        Debug.DrawRay(player.transform.position + Vector3.up * 0.1f, Vector3.down * maxDistance);

        return Physics.Raycast(ray, maxDistance);
    }
    
    protected void GetMoveInput()
    {
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);
        var temp = player.cameraTransform.forward;
        temp.y = 0f;
        velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * temp.normalized;
        velocity.y = 0f;
    }
    #endregion

    protected void ResetAnimatorTrigger()
    {
        player.animator.ResetTrigger("jump");
        player.animator.ResetTrigger("landing");
        player.animator.ResetTrigger("interaction");
        player.animator.ResetTrigger("death");
    }
    
    // State 바뀔 때 마다 출력
    public virtual void Enter()
    {
#if UNITY_EDITOR
        Debug.Log("enter state: " + this.ToString());
#endif
    }

    // Input 체크
    public virtual void HandleInput()
    {

    }

    // State 교체 Logic
    public virtual void LogicUpdate()
    {
        if (player.isDead)
        {
            stateMachine.ChangeState(player.death);
            return;
        }
    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}
