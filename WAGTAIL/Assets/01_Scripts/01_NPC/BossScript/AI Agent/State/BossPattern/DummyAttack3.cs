using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>

public class DummyAttack3 : AIState
{
    private int targetCount = 0;
    private float curTimer = 0;
    private float changeTimer = 2;
    private float rad;
    private float time = 2f;
    private Transform shootPoint;
    private GameObject blackBullet;
    private GameObject circleObj;

    private List<Vector3> targets = new List<Vector3>();
    private List<GameObject> marker = new List<GameObject>();

    public DummyAttack3(AIStateMachine stateMachine, GameObject bullet, Transform sp, GameObject obj, float flightTime, int count, float rad) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        shootPoint = sp;
        targetCount = count;
        blackBullet = bullet;
        time = flightTime;
        this.rad = rad;
        circleObj = obj;
    }

    public override void Enter()
    {
        CreateMarker();
        PositionLuncher();
        Debug.Log("Start Attack2");
    }

    public override void Exit()
    {
        foreach (var m in marker)
        {
            GameObject.Destroy(m);
        }
        Debug.Log("End Attack2");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        // Bullet이 충돌할 경우 다음 스테이트로 이동.
        curTimer += Time.deltaTime;
        if (curTimer > changeTimer)
        {
            if (children.Count > 0)
                stateMachine.ChangeState(children[current]);
            else if (parent != null)
                stateMachine.ChangeState(parent);
            else if (stateMachine.Pattern.Count > 0)
                stateMachine.NextPattern();
            else
                Debug.Log("연결된 State가 없음.");

            curTimer = 0;
        }
    }

    void CreateMarker()
    {
        targets.Clear();
        // Player 기준 원 범위 서치
        for (int i = 0; i < targetCount; i++)
            targets.Add(Search());

        foreach (var t in targets)
        {
            GameObject _obj = GameObject.Instantiate(circleObj);

            _obj.transform.rotation = Quaternion.Euler(90, 0, 0);
            _obj.transform.position = t;
            //Debug.Log(t);
            marker.Add(_obj);
        }
    }

    private void PositionLuncher()
    {
        foreach (var t in targets)
        {
            Vector3 pos = CaculateVelocity(t, shootPoint.position, time);

            GameObject obj = GameObject.Instantiate(blackBullet, shootPoint.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().velocity = pos;

        }
    }

    private Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        // define the distance x and y first;
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
        distanceXZ.y = 0f; // y는 0으로 설정.

        // Create a float the represent our distance
        float Sy = distance.y;      // 세로 높이의 거리를 지정.
        float Sxz = distanceXZ.magnitude;

        // 속도 추가
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }

    Vector3 Search()
    {
        // Random.onUnitSphere : 반경 1을 갖는 구의 표면상에서 임의의 지점을 반환함
        Vector3 getPoint = Random.onUnitSphere;
        getPoint.y = 0.1f;

        // 0.0f 부터 지정한 반지름의 길이 사이의 랜덤 값을 산출함.
        float r = Random.Range(0.0f, rad);
        Vector3 vec = (getPoint * r) + Player.Instance.transform.position;

        return new Vector3(vec.x, 0.1f, vec.z);
    }

}
