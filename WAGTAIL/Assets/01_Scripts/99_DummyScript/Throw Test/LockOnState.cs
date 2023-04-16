using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnState : State
{
    float gravityValue;

    Vector3 currentVelocity;

    bool isGrounded; // 점프를 체크함.
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
        // 애니메이션 적용

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

        // 정해진 타겟이 생기면
        if(targetSearch())
        {
            // 카메라 관련 로직
            Debug.Log("Search");
        }

        if(!Lock )
        {
            stateMachine.ChangeState(player.idle);
        }
    }

    public bool targetSearch()
    {
        // 플레이어가 바라보는 기준으로 원뿔형으로 서치를 시작.
        Collider[] obj = Physics.OverlapSphere(player.transform.position, m_SearchDistance);

        targetList.Clear();

        float RadianRange = Mathf.Cos((DetectionAngle / 2) * Mathf.Deg2Rad);

        for (int i = 0; i < obj.Length; i++)
        {
            float targetRadian = Vector3.Dot(player.transform.forward,
    (obj[i].transform.position - player.transform.position).normalized);

            // 라디안 범위에 들어오면
            if (targetRadian > RadianRange)
            {
                // 수집한다.
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