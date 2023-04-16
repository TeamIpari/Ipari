using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnState : State
{
    float gravityValue;

    Vector3 currentVelocity;

    bool isGrounded; // ������ üũ��.
    bool Lock;
    float playerSpeed;

    [Range(10f, 50f)]
    [SerializeField]float DetectionAngle = 45.0f;
    [SerializeField, Range(1f, 5f)]
    private float m_SearchDistance = 2.0f;

    // auto target;
    List<GameObject> targetList = new List<GameObject>();

    public LockOnState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        // �ִϸ��̼� ����

        // start
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;

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
    }


public override void HandleInput()
    {
        base.HandleInput();

        Lock = player.isLockOn;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ������ Ÿ���� �����
        if(targetSearch())
        {
            // ī�޶� ���� ����
            Debug.Log("Search");
        }

        if(!Lock )
        {
            stateMachine.ChangeState(player.idle);
        }
    }

    public bool targetSearch()
    {
        // �÷��̾ �ٶ󺸴� �������� ���������� ��ġ�� ����.
        Collider[] obj = Physics.OverlapSphere(player.transform.position, m_SearchDistance);

        targetList.Clear();

        float RadianRange = Mathf.Cos((DetectionAngle / 2) * Mathf.Deg2Rad);

        for (int i = 0; i < obj.Length; i++)
        {
            float targetRadian = Vector3.Dot(player.transform.forward,
    (obj[i].transform.position - player.transform.position).normalized);

            // ���� ������ ������
            if (targetRadian > RadianRange)
            {
                // �����Ѵ�.
                if (obj[i].tag == "Target")
                    targetList.Add(obj[i].gameObject);

                Debug.DrawLine(player.transform.position,
                    obj[i].transform.position, Color.red);
            }
        }

        if (targetList.Count != 0)
            return true;

        return false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();


    }

}