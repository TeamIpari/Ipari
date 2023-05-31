using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
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
        // Bullet�� �浹�� ��� ���� ������Ʈ�� �̵�.
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
                Debug.Log("����� State�� ����.");

            curTimer = 0;
        }
    }

    void CreateMarker()
    {
        targets.Clear();
        // Player ���� �� ���� ��ġ
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
        Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
        distanceXZ.y = 0f; // y�� 0���� ����.

        // Create a float the represent our distance
        float Sy = distance.y;      // ���� ������ �Ÿ��� ����.
        float Sxz = distanceXZ.magnitude;

        // �ӵ� �߰�
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }

    Vector3 Search()
    {
        // Random.onUnitSphere : �ݰ� 1�� ���� ���� ǥ��󿡼� ������ ������ ��ȯ��
        Vector3 getPoint = Random.onUnitSphere;
        getPoint.y = 0.1f;

        // 0.0f ���� ������ �������� ���� ������ ���� ���� ������.
        float r = Random.Range(0.0f, rad);
        Vector3 vec = (getPoint * r) + Player.Instance.transform.position;

        return new Vector3(vec.x, 0.1f, vec.z);
    }

}
