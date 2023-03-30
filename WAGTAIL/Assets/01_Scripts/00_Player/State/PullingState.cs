using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PullingState : State
{
    float gravityValue;
    Vector3 currentVelocity;

    bool isGrounded;
    float playerSpeed;

    bool isPull;

    public PullingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        // input ฐüทร
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        isPull = player.isPull;

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void Exit()
    {
        base.Exit();

        //player.controller.height = player.normalColliderHeight;
        //player.controller.center = player.normalColliderCenter;
        //player.controller.radius = player.normalColliderRadius;
        //gravityVelocity.y = 0f;
        //player.playerVelocity = new Vector3(input.x, 0, input.y);


    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
