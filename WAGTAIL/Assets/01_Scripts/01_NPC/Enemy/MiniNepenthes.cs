using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using IPariUtility;
using UnityEngine.InputSystem.XR;

public class MiniNepenthes : Enemy
{
    [Header("MiniNepenthes Inspector")]
    public Bullet BulletPrefab;
    public Transform ShotPosition;
    public float ShotSpeed;
    public int Angle;

    [SerializeField] private bool DebugMode = false;

    [Header("View Config")]
    [Range(0f, 360f)]
    [SerializeField] private float horizontalViewAngle = 0f;
    [Range(-180f, 180f)]
    [SerializeField] private float viewRotateZ = 0f;

    [SerializeField] private LayerMask viewTargetMask;
    [SerializeField] private LayerMask viewObstacleMask;

    private int layer = 0;
    private float horizontalViewHalfAngle = 0f;

    private void Awake()
    {
        horizontalViewHalfAngle = horizontalViewAngle * 0.5f;
    }


    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            horizontalViewHalfAngle = horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, AttackRange);

            Vector3 horizontalRightDir = IpariUtility.AngleToDirY(transform,-horizontalViewHalfAngle + viewRotateZ);
            Vector3 horizontalLeftDir = IpariUtility.AngleToDirY(transform, horizontalViewHalfAngle + viewRotateZ);
            Vector3 lookDir = IpariUtility.AngleToDirY(transform, viewRotateZ);

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

        AiIdle = new NepenthesIdleState(AiSM, viewRotateZ, horizontalViewHalfAngle, viewTargetMask,viewObstacleMask);
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

    bool GetPlayerFloorInfo(out RaycastHit hit, CharacterController cc, float downMovespeed = 0f)
    {
        #region Ommision
        //CharacterController controller = GetComponent<CharacterController>();
        float heightHalf = cc.height;
        float radius = cc.radius;
        float heightHalfOffset = (heightHalf * .5f) - radius;
        Vector3 playerPos = cc.transform.position;
        Vector3 center = (playerPos + cc.center);

        return Physics.SphereCast(
            center,
            radius,
            Vector3.down,
            out hit,
            heightHalf + .1f,
            layer
        );
        #endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("AA");
        CharacterController cc = collision.gameObject.GetComponent<CharacterController>();
        if (collision.collider.CompareTag("Player") && cc.velocity.y <= 0)
        {
            Debug.Log($"Hello1");
            RaycastHit hit;
            GetPlayerFloorInfo(out hit,cc);
            bool isSaameObject = (hit.transform.gameObject.Equals(gameObject));
            bool isLanded = (hit.normal.y > 0);
            if( isSaameObject && isLanded)
            {
                Debug.Log($"Hello2");
            }
            collision.collider.GetComponent<Player>().isDead = true;
        }
    }
}
