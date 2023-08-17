using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    public Player player;
    public StateMachine stateMachine;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;
    

    public InputAction moveAction;
    public InputAction interactionAction;
    public InputAction climbingAction;
    public InputAction jumpAction;
    public InputAction pushZAxisAction; // 상하
    public InputAction pushXAxisAction; // 좌우

    // 생성자
    public State(Player _player, StateMachine _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;

        //PlayerInput
        moveAction = player.playerInput.actions["Move"];
        jumpAction = player.playerInput.actions["jump"];
        interactionAction = player.playerInput.actions["Interaction"];
        climbingAction = player.playerInput.actions["Climbing"];
        pushXAxisAction = player.playerInput.actions["MoveXAxis"];
        pushZAxisAction = player.playerInput.actions["MoveZAxis"];
    }

    // State 바뀔 때 마다 출력
    public virtual void Enter()
    {
        Debug.Log("enter state: " + this.ToString());

    }

    // Input 체크
    public virtual void HandleInput()
    {

    }

    // State 교체 Logic
    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}
