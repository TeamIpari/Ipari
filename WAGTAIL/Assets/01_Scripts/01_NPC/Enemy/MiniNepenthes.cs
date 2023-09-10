using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using IPariUtility;

public class MiniNepenthes : Enemy
{
    [Header("MiniNepenthes Inspector")]
    public Bullet BulletPrefab;
    public Transform ShotPosition;
    public float ShotSpeed;
    public int angle;

    [SerializeField] private bool m_bDebugMode = false;

    [Header("View Config")]
    [Range(0f, 360f)]
    [SerializeField] private float m_horizontalViewAngle = 0f;
    [Range(-180f, 180f)]
    [SerializeField] private float m_viewRotateZ = 0f;

    [SerializeField] private LayerMask m_viewTargetMask;
    [SerializeField] private LayerMask m_viewObstacleMask;


    private float m_horizontalViewHalfAngle = 0f;

    private void Awake()
    {
        m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;
    }


    private void OnDrawGizmos()
    {
        if (m_bDebugMode)
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, AttackRange);

            Vector3 horizontalRightDir = IpariUtility.AngleToDirY(transform,-m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = IpariUtility.AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = IpariUtility.AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * AttackRange, Color.cyan);
            Debug.DrawRay(originPos, lookDir * AttackRange, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * AttackRange, Color.cyan);
        }
        else
            Gizmos.DrawWireSphere(transform.position, base.AttackRange);

    }
    // Start is called before the first frame update
    void Start()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new NepenthesIdleState(AiSM, m_viewRotateZ, m_horizontalViewHalfAngle, m_viewTargetMask,m_viewObstacleMask);
        AiAttack = new NepenthesAttackState(AiSM);
        SetAttackPattern();
        AiWait = new NepenthesWaitState(AiSM, WaitRate);
        
        AiSM.Initialize(AiIdle);

        AttackTimer = 0;
        if (ShotSpeed <= 0)
            ShotSpeed = 1.0f;
        if (WaitRate <= 0)
            WaitRate = 1.0f;
        if (base.AttackRange <= 0)
            base.AttackRange = 5.0f;
    }

    public override void CAttack(Vector3 Pos)
    {
        //base.CAttack();
        //Debug.Log("네펜데스의 공격");

        // 방향 벡터를 구하고 해당 방향으로 탄환을 쏨.
        Vector3 PlayerPos = new Vector3(Pos.x, ShotPosition.position.y, Pos.z);
        Vector3 direction = PlayerPos - ShotPosition.position;

        GameObject Bomb = Instantiate(BulletPrefab.gameObject);
        Bomb.GetComponent<Bullet>().SetDirection(direction.normalized * ShotSpeed);
        Bomb.transform.position = ShotPosition.position;
        Destroy(Bomb, 2f);
    }


    // Update is called once per frame
    void Update()
    {
        isAttack();
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }
}
