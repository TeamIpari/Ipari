using UnityEngine;

public class AIRunState : AIState
{
    private Vector3 destination;
    private float runDistance;
    private float moveSpeed = 0f;
    private bool isFlight = false;

    private bool isMovingPoint = false;

    private RaycastHit hit;

    private Vector3 vecRallyPoint;

    public AIRunState (AIStateMachine stateMachine, float runDistance, float moveSpeed) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.runDistance = runDistance + 2;
        this.moveSpeed = moveSpeed;
        if(this.moveSpeed <= 0)
        {
            this.moveSpeed = 1.0f;
        }
    }

    public override void Enter()
    {
        //stateMachine.NPCBase.stateName = "Run";

    }

    public override void Exit()
    {
        destination = Vector3.zero;
    }

    public override void OntriggerEnter(Collider other)
    {

    }

    public override void Update()
    {
        //if(stateMachine.Physics)

        if (!isMovingPoint)
        {
            // 자동 이동. 
            SetPoint();
            RayWallCheck();
            //
        }
        else
        {
            // 랠리포인트로 이동.
            Debug.Log("Start Rally Point");
            RayWallCheck();

            //RayWallCheck();
            //CreateRallyPoint();
            //
        }
        RayFloorCheck();

        Search();
    }


    /// <summary>
    /// 이동 할 좌표 세팅.
    /// </summary>
    private void SetPoint()
    {
        destination = new Vector3(AISM.Transform.position.x - AISM.Target.transform.position.x, 0f,
            AISM.Transform.position.z - AISM.Target.transform.position.z).normalized;
    }

    /// <summary>
    /// 플레이어가 다가오는 반대방향으로 이동하는 기능.
    /// </summary>
    private void Movement()
    {
        // 도망쳐!
        // Debug.Log("돔황챠!");
        // stateMachine.Agent.SetDestination(stateMachine.Transform.position + destination * 5f);
        // 물리로 움직이게 해보자.
        // 바라보는 방향으로 감.
        // Ray로 갈 수 있는 방향인지 체크
        if (!isMovingPoint)
        {
            AISM.Transform.position
            = Vector3.MoveTowards(AISM.Transform.position, AISM.Transform.position + destination, moveSpeed * Time.deltaTime);
        }
        else if (isMovingPoint)
        {
            AISM.Transform.position = Vector3.MoveTowards(AISM.Transform.position, vecRallyPoint, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(AISM.Transform.position, vecRallyPoint) < .1f)
                isMovingPoint = false;
        }
    }

    /// <summary>
    /// 회전 플레이어가 다가오는 반대방향을 향해 바라보는 기능.
    /// </summary>
    private void Rotate()
    {
        if (!isMovingPoint)
            AISM.Transform.LookAt(AISM.Transform.position + destination);
        else if (isMovingPoint)
            AISM.Transform.LookAt(vecRallyPoint);
    }


    private Vector3 CaculeateVelocity(Vector3 target, Vector3 origin , float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }

    /// <summary>
    /// 기능 - 가는 방향에 지형이 존재하지 않는 경우 아래로 뛰어 내림.
    /// </summary>
    private void JumpDown()
    {
        if(!isFlight)
        {
            Vector3 targetPos = AISM.Transform.position + (AISM.Transform.forward * 2f);
            targetPos -= AISM.Transform.up;
            Vector3 pos = CaculeateVelocity(targetPos, AISM.Transform.position , 1f);
            AISM.Physics.velocity = pos;
            isFlight = true;
        }
    }

    private void InitializeVelocity()
    {
        if (isFlight)
        {
            //Debug.Log("init" + stateMachine.Physics.velocity);
            AISM.Physics.velocity = Vector3.zero;
            isFlight = false;
        }
    }

    private void RayFloorCheck()
    {
        if (Physics.Raycast(AISM.Transform.position + (AISM.Transform.forward * .5f), -AISM.Transform.up, out hit, 1.1f))
        {
            Debug.DrawRay(AISM.Transform.position + (AISM.Transform.forward * 1f), -AISM.Transform.up * 1f, Color.red);

            InitializeVelocity();
            Movement();
            Rotate();
        }
        else
        {
            Debug.DrawRay(AISM.Transform.position + (AISM.Transform.forward * 1f), -AISM.Transform.up * 1f, Color.blue);

            JumpDown();
        }

    }

    private void RayWallCheck()
    {
        Debug.DrawRay(AISM.Transform.position, AISM.Transform.forward, Color.red);
        if (Physics.Raycast(AISM.Transform.position, AISM.Transform.forward, out hit, 1f))
        {
            isMovingPoint = true;
            // 랠리 포인트 설정. 
            vecRallyPoint = CreateRallyPoint();
            //CreateRallyPoint();

        }
        else
        {
            //Debug.Log("BB");

        }
    }

    private Vector3 CreateRallyPoint()
    {
        #region Function1
        //// 입사각 (충돌지점 - 출발지점)
        //Vector3 incidentVector = hit.transform.position - stateMachine.Transform.position;

        //// 법선 벡터.
        //Vector3 normalVec = hit.normal ;

        //// 반사각
        //Vector3 reflectVec = Vector3.Reflect(incidentVector, normalVec);


        //reflectVec.y = 0;

        //Debug.Log(reflectVec.y);
        //Debug.DrawRay(stateMachine.Transform.position + destination, reflectVec * 2f, Color.green);
        #endregion
        // 입사각
        Vector3 incidentVec = hit.transform.position - AISM.Transform.position;

        // 충돌한 면의 벡터
        Vector3 collisionVec = hit.transform.position;

        // xz축으로 계산.
        // 충돌한 면의 벡터를 각도로 변환
        float collisionAngle = Mathf.Atan2(collisionVec.z, collisionVec.x) * 90f / Mathf.PI;

        // 입사벡터를 각도로 변환
        float incidentAngle = Vector3.SignedAngle(collisionVec, incidentVec, -Vector3.forward);

        // 반사할 벡터의 각도를 구함 (충돌한 면의 벡터 기준)
        float reflectAngle = incidentAngle - 90 + collisionAngle;

        // 반사할 벡터의 각도를 라디안으로 변환
        float reflectionRadian = reflectAngle * Mathf.Deg2Rad;

        // 반사 벡터
        Vector3 reflectVector = new Vector3(Mathf.Cos(reflectionRadian), 0, Mathf.Sin(reflectionRadian));

        //Debug.DrawRay(stateMachine.Transform.position + destination, reflectVector * 2f, Color.green);


        return AISM.Transform.position + destination + reflectVector * 2.5f;


    }

    private void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(AISM.Transform.position,
            AISM.Target.transform.position));
        if (distance > runDistance)
        {
            AISM.ChangeState(Parent);
        }
    }
}
