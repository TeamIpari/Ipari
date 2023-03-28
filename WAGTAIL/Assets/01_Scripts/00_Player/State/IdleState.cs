using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : State
{
    float gravityValue;
    bool climbing;
    bool push;
    bool carry;
    bool jump = false;
    Vector3 currentVelocity;
    bool isGrounded;
    float playerSpeed;

    Vector3 cVelocity;

    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        jump = false;
        climbing = player.isClimbing;
        push = player.isPush;
        carry = player.isCarry;

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
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

        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // TODO : animator ����

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

        // TODO : Idle �����϶� �߶� �� ���� ����� �� landingStateȣ�� ���ֱ�
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        // �ٴڰ� ��� ���� ���� �߷� ���� X
        if(isGrounded && gravityVelocity.y < 0 )
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0 )
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

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
