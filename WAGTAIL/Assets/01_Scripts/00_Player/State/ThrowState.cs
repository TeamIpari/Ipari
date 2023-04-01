using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : State
{
    float gravityValue;
    Vector3 currentVelocity;
    bool isGrounded;
    bool carry;
    float playerSpeed;

    public ThrowState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
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
        carry = player.isCarry;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

}
