using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    protected Player player;
    protected StateMachine stateMachine;

    public Vector3 gravityVelocity;
    public Vector3 velocity;
    protected Vector2 input;
    
    protected readonly InputAction moveAction;
    protected readonly InputAction climbingAction;
    protected readonly InputAction jumpAction;
    protected readonly InputAction interactAction;
    protected readonly InputAction pushZAxisAction; // 상하
    protected readonly InputAction pushXAxisAction; // 좌우

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

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {

    }
}
