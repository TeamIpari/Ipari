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
    private static readonly int Push1 = Animator.StringToHash("push");
    private static readonly int Move = Animator.StringToHash("move");

    public PushState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetTrigger(Push1);

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        push = player.isPush;

        // ========================================= //
        // TODO : 占쏙옙占쏙옙 占쏙옙체 占쏙옙큼 collider 크占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占
        // player.controller.center = new Vector3(0,0,0); << 占쏙옙치 占쏙옙占쏙옙 占쏙옙 占쏙옙占쏙옙
        // player.controller.radius = 0; << 占쏙옙치 占쏙옙占쏙옙 占쏙옙 占쏙옙占쏙옙
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

        // TODO : 占쌍니몌옙占싱쇽옙 占쏙옙占쏙옙占싹깍옙 Push To Idle
        player.animator.SetTrigger(Move);
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

            //velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
            velocity.y = 0f;
        }

        else if (player.transform.eulerAngles.y == 180 || player.transform.eulerAngles.y == 0)
        {
            input = pushZAxisAction.ReadValue<Vector2>();
            velocity = new Vector3(0, 0, input.y);

            //velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
            velocity.y = 0f;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 占쏙옙摸占쏙옙占쏙옙占 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙
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

        // 占쌕닥곤옙 占쏙옙占 占쏙옙占쏙옙 占쏙옙占쏙옙 占쌩뤄옙 占쏙옙占쏙옙 X
        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(currentVelocity * (Time.deltaTime * playerSpeed) + gravityVelocity * Time.deltaTime);

        //if (velocity.sqrMagnitude > 0)
        //{
        //    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
        //        player.rotationDampTime);
        //}
    }
}
