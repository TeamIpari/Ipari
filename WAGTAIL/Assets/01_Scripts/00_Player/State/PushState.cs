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

        player.animator.SetTrigger("push");

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        push = player.isPush;

        // ========================================= //
        // TODO : 끄는 물체 만큼 collider 크기 조정 해줘야함
        // player.controller.center = new Vector3(0,0,0); << 수치 조절 후 적용
        // player.controller.radius = 0; << 수치 조절 후 적용
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

        // TODO : 애니메이션 세팅하기 Push To Idle
        player.animator.SetTrigger("move");
        // ========================================= //
    }

    public override void HandleInput()
    {
        base.HandleInput();

        push = player.isPush;
        // 오일러 y값이 90이거나 270일 경우? 무슨 뜻이지?
        if (player.transform.eulerAngles.y == 90 || player.transform.eulerAngles.y == 270)
        {
            input = pushXAxisAction.ReadValue<Vector2>();
            velocity = new Vector3(input.x, 0, 0);

            //velocity = velocity.x * player.cameraTransform.right.normalized + velocity.z * player.cameraTransform.forward.normalized;
            velocity.y = 0f;
        }

        // 그러니까 오일러 각으로 변환한 플레이어의 로테이션이 180이거나 0일 경우 실행을 한다는 이야기인 것 같은데... 왜?
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

        // 사다리에서 내려 왔을 때
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

        // 바닥과 닿아 있을 때는 중력 적용 X
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
