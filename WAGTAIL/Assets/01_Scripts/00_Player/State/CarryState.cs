using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarryState : State
{
    float gravityValue;
    Vector3 currentVelocity;
    bool isGrounded;
    bool carry;
    bool smallThrow;
    float playerSpeed;
    bool jump;

    Vector3 cVelocity;

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

        jump = false;
        carry = player.isCarry;
        smallThrow = player.isSmallThrow;
        if(!smallThrow) playerSpeed = 3.5f;
        else playerSpeed = player.playerSpeed;

        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered)
        {
            jump = true;
        }

        carry = player.isCarry;
        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // TODO : animator 적용
        player.animator.SetFloat("speed", input.magnitude, player.speedDampTime, Time.deltaTime);

        if (!carry)
        {
            // 상태에 따른 state 변화 시켜주기
            // TODO : Drop 상태로 이동하기
            
            stateMachine.ChangeState(player.drop);
        }

        //if (jump)
        //{
        //    stateMachine.ChangeState(player.jump);
        //}
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
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

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
    }
}
