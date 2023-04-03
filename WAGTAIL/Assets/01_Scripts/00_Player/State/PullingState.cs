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

        playerSpeed = player.playerSpeed;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {

        base.HandleInput();
        isPull = player.isPull;
        input = moveAction.ReadValue<Vector2>();
        // 3���� ���������� y��(���� ����)���� �̵����� �ʱ� ������
        // 2���������� �̵� ��ǥ���� y���� z������ �����.
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * player.cameraTransform.right.normalized + 
            velocity.z * player.cameraTransform.forward.normalized;
        velocity.y = 0;
        //isPull = player.isPull;

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //Debug.Log("State == Pulling");
        if(!isPull)
        {
            stateMachine.ChangeState(player.drop);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }



        if (RopeHead != null)
        {
            float curSpeed = (playerSpeed - (playerSpeed * subtract()) / 100);
            currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);
            player.controller.Move(currentVelocity * Time.deltaTime * curSpeed + gravityVelocity * Time.deltaTime);  // �ӵ� ����
            player.transform.LookAt(RopeHead.GetComponent<Node>().GetParent()._TailRope.transform.position);
            if (subtract() >= 90)        // 10% �̸��� ��� ���� �ı�
            {
                RopeHead.GetComponent<Node>().GetParent().BrokenRope(); ;
                player.isPull = false ;
            }
        }

        //subtract();

        //if (velocity.sqrMagnitude > 0)
        //{
        //    //player.transform.rotation =
        //    //    Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation());


        //    //player.transform.rotation =
        //    //    Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity), player.rotationDampTime);
        //}
    }

    public int subtract()
    {
        // �������� ������ ���Ͽ� �� ��ġ�� ���� �󸶳� ���������� �ۼ��������� ���. 

        return RopeHead.GetComponent<Node>().GetParent().Percent();
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
}
