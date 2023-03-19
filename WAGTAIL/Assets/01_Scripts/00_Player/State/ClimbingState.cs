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

    // TODO : 사다리 끝 올라간거 체크하기
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
        // 애니메이션 세팅
        // player.animator.SetTrigger("climing");
        gravityVelocity.y = 0;
        ladder = player.currentInteractable;
        ladderHeight = CalcHeight(ladder) + 1;
        playerHeight = 0f;

        // TODO : 오르는 속도 내려가는 속도 조절해야됨
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

        // 사다리에서 내려 왔을 때
        if (isGrounded && input.y < 0) 
        {
            player.isClimbing = false;
            stateMachine.ChangeState(player.idle);
        }

        // 사다리 끝에 올라 갔을 때
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

        // TODO : 사다리 내려 갈때 속도 증가
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
        currentVelocity = new Vector3(0, 0, 0);
        //player.animator.SetTrigger("move");
    }

    // 높이 계산
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
