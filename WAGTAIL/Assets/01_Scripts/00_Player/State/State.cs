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
    public InputAction pushZAxisAction; // ����
    public InputAction pushXAxisAction; // �¿�

    // ������
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

    // State �ٲ� �� ���� ���
    public virtual void Enter()
    {
        Debug.Log("enter state: " + this.ToString());

    }

    // Input üũ
    public virtual void HandleInput()
    {

    }

    // State ��ü Logic
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
