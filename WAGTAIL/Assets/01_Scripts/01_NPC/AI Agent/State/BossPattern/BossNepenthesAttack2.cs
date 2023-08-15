using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossNepenthesAttack2 : AIAttackState
{
    private float curTimer = 0;
    private float changeTimer = 3;

    private Transform shootPoint;
    private GameObject bullet;
    private GameObject circleObj;
    private Vector3 target;
    private GameObject marker;

    private float time;


    public BossNepenthesAttack2(AIStateMachine stateMachine, GameObject bullet, Transform sp, GameObject obj, float time) : base(stateMachine)
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
        curTimer = 0;
        Debug.Log("Start Attack2");
    }

    public override void Exit()
    {
        GameObject.Destroy(marker);
        marker = null;
//Debug.Log("End Attack1");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        //marker.transform.localScale = Vector3.one  * Time.deltaTime / changeTimer;
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
        Debug.Log(obj.GetComponent<Bullet>());
        obj.GetComponent<AcidBomb>().ShotDirection(pos);

    }



}
