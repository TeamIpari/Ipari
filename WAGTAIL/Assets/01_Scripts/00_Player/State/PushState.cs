using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PushState : State
{
    float gravityValue;
    Vector3 currentVelocity;
    bool isGrounded;
    bool push;
    float playerSpeed;

    Vector3 cVelocity;

    Vector3 objectPos;

    public PushState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        // ========================================= //
        // TODO : �ִϸ��̼� �����ϱ� Idle To Push
        // player.animator.SetTrigger("push");
        // ========================================= //

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        push = player.isPush;

        // ========================================= //
        // TODO : ���� ��ü ��ŭ collider ũ�� ���� �������
        // player.controller.center = new Vector3(0,0,0); << ��ġ ���� �� ����
        // player.controller.radius = 0; << ��ġ ���� �� ����
        // ========================================= //

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void Exit()
    {
        base.Exit();
        player.controller.height = player.normalColliderHeight;
        player.controller.center = player.normalColliderCenter;
        player.controller.radius = player.normalColliderRadius;
        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);

        // ========================================= //
        // TODO : �ִϸ��̼� �����ϱ� Push To Idle
        // player.animator.SetTrigger("Idle");
        // ========================================= //
    }

    public override void HandleInput()
    {
        base.HandleInput();

        push = player.isPush;
        if (player.transform.eulerAngles.y == 90 || player.transform.eulerAngles.y == 270)
        {
            input = pushXAxisAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, 0);
        }

        else if (player.transform.eulerAngles.y == 180 || player.transform.eulerAngles.y == 0)
        {
            input = pushZAxisAction.ReadValue<Vector2>();
            velocity = new Vector3(0, 0, input.y);
        }
        Debug.Log(player.transform.eulerAngles.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��ٸ����� ���� ���� ��
        if (!push)
        {
            stateMachine.ChangeState(player.idle);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        // �ٴڰ� ��� ���� ���� �߷� ���� X
        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        //if (velocity.sqrMagnitude > 0)
        //{
        //    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
        //        player.rotationDampTime);
        //}
    }
}
