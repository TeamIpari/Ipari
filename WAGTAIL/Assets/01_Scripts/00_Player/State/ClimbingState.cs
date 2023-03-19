using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : State
{
    float playerSpeed;
    bool isGrounded;
    //float gravityValue;
    Vector3 currentVelocity;
    Vector3 cVelocity;
    GameObject ladder;
    float ladderHeight;
    float playerHeight;

    // TODO : ��ٸ� �� �ö󰣰� üũ�ϱ�
    public bool isTop;

    public ClimbingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        isTop = false;
        // �ִϸ��̼� ����
        // player.animator.SetTrigger("climing");
        gravityVelocity.y = 0;
        ladder = player.currentInteractable;
        ladderHeight = CalcHeight(ladder) + 1;
        playerHeight = 0f;

        // TODO : ������ �ӵ� �������� �ӵ� �����ؾߵ�
        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        //gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = climbingAction.ReadValue<Vector2>();
        velocity = new Vector3(0, input.y, 0);

        if (playerHeight >= ladderHeight)
        {
            isTop = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��ٸ����� ���� ���� ��
        if (isGrounded && input.y < 0) 
        {
            player.isClimbing = false;
            stateMachine.ChangeState(player.idle);
        }

        // ��ٸ� ���� �ö� ���� ��
        if (isTop && input.y > 0)
        {
            player.isClimbing = false;
            stateMachine.ChangeState(player.idle);
        }
    }

    public override void PhysicsUpdate()
    {

        base.PhysicsUpdate();

        isGrounded = player.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        player.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);
        playerHeight = player.transform.position.y;

        // TODO : ��ٸ� ���� ���� �ӵ� ����
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
        currentVelocity = new Vector3(0, 0, 0);
        //player.animator.SetTrigger("move");
    }

    // ���� ���
    public float CalcHeight(GameObject _calcTarget)
    {
        float _height = 0f;

        MeshFilter _mf = _calcTarget.GetComponent<MeshFilter>();

        Vector3[] _vertices = _mf.mesh.vertices;

        foreach (var _vertice in _vertices)
        {
            Vector3 _pos = ladder.transform.TransformPoint(_vertice);

            if(_pos.y > _height)
            {
                _height = _pos.y;
            }
        }

        return _height;
    }
}
