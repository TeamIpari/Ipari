using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// 작성자 : 성지훈
/// 추가 작성
/// </summary>
public class DummyAttack2 : AIState
{
    private float curTimer = 0;
    private float changeTimer = 3;

    private Transform shootPoint;
    private GameObject bullet;
    private GameObject circleObj;
    private Vector3 target;
    private GameObject marker;

    private float time;


    public DummyAttack2(AIStateMachine stateMachine, GameObject bullet, Transform sp, GameObject obj, float time) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.circleObj = obj;
        this.shootPoint = sp;
        this.bullet = bullet;
        this.time = time;
    }

    public override void Enter()
    {
        // 타겟 설정.
        CreateMarker();
        PositionLuncher();
        Debug.Log("Start Attack2");
    }

    public override void Exit()
    {
        GameObject.Destroy(marker);
        Debug.Log("End Attack1");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        //base.Update();
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
    private void CreateMarker()
    {
        target = new Vector3(Player.Instance.transform.position.x,
            Player.Instance.transform.position.y + 0.1f,
            Player.Instance.transform.position.z);

        GameObject _obj = GameObject.Instantiate(circleObj);
        _obj.transform.localScale = Vector3.one * 3f;
        _obj.transform.position = target;
        _obj.transform.rotation = Quaternion.Euler(90, 0, 0);
        marker = _obj;
    }
    private void PositionLuncher()
    {
        Vector3 pos = CaculateVelocity(target, shootPoint.position, time);

        GameObject obj = GameObject.Instantiate(bullet, shootPoint.position, Quaternion.identity);
        obj.transform.localScale = Vector3.one * 3f;
        obj.GetComponent<Rigidbody>().velocity = pos;
        
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

}
