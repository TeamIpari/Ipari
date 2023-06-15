using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// ���� ��� �ִ� current Object�� SkinnedLenderer? �� Blend Scale üũ�� �ʿ䰡 ����.
/// </summary>

public class PullingState : State
{
    float gravityValue;
    Vector3 currentVelocity;

    bool isGrounded;
    float playerSpeed;
    bool isPull;

    GameObject RopeHead;

    Vector3 cVelocity;


    public PullingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;   

    }

    public override void Enter()
    {
        base.Enter();
        // input ����
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        isPull = player.isPull;
        RopeHead = player.currentInteractable;

        playerSpeed = player.pullingSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
        player.animator.SetTrigger("pulling");
    }

    public override void HandleInput()
    {

        base.HandleInput();
        isPull = player.isPull;
        float _val =0;
        input = pushZAxisAction.ReadValue<Vector2>();
        if(input.y == 1)
        {
            return;
        }
        if (player.transform.eulerAngles.y == 90)
        {
            velocity = new Vector3(input.y, 0, 0);
        }

        else if (player.transform.eulerAngles.y == 270)
        {
            velocity = new Vector3(-input.y, 0, 0);
        }

        else if (player.transform.eulerAngles.y == 180)
        {
            velocity = new Vector3(0, 0, -input.y);
        }

        else if (player.transform.eulerAngles.y == 0)
        {
            velocity = new Vector3(0, 0, input.y);
        }
        else
            velocity = Vector3.zero;
        _val = -input.y;
        velocity.y = 0f;
        //Debug.Log("input Y : " + input.y + " //// " + "Input -Y : " + -input.y);

        if (player.currentInteractable.GetComponent<Pulling>().GetMeshfloat() < 90)
            player.currentInteractable.GetComponent<Pulling>().SetMeshfloat(_val);

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.animator.SetFloat("speed", input.magnitude, player.speedDampTime, Time.deltaTime);

        if (!isPull)
        {
            stateMachine.ChangeState(player.pullOut);
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

        float curSpeed = playerSpeed - (playerSpeed * player.currentInteractable.GetComponent<Pulling>().GetMeshfloat() / 100);     // ���⿡ (n - 100 ) /   ; �ۼ�Ʈ ���
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
        //Debug.Log(curSpeed);
        player.controller.Move(currentVelocity * Time.deltaTime * curSpeed + gravityVelocity * Time.deltaTime);

        if (player.currentInteractable.GetComponent<Pulling>().IsTarget())
        {
            player.isPull = false;
        }
    }

    public override void Exit()
    {
        base.Exit();

    }
}