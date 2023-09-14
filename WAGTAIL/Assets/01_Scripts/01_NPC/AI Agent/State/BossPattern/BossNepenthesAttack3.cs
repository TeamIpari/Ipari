using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;


public class BossNepenthesAttack3 : AIAttackState
{
    private int targetCount = 0;
    private float curTimer = 0;
    private float changeTimer = 2f;
    private float rad;
    private float time = 2f;
    private float DelayTime = 0.8f;
    private bool isShoot = false;

    private Transform shootPoint;
    private GameObject AcidBullet;
    private GameObject circleObj;

    private List<Vector3> targets = new List<Vector3>();
    private List<GameObject> marker = new List<GameObject>();

    //====================================================
    /////               magic Methods               /////
    //====================================================
    public BossNepenthesAttack3(AIStateMachine stateMachine,BossNepenthesProfile Profile, float flightTime, int count, float rad) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.shootPoint = Profile.ShotPosition;
        this.targetCount = count;
        this.AcidBullet = Profile.BulletPrefab;
        this.time = flightTime;
        this.rad = rad;
        this.circleObj = Profile.ShotMarker;
        this.DelayTime = 0.8f;
    }

    //====================================================
    /////                   override                   /////
    //====================================================

    public override void Enter()
    {
        stateMachine.Animator.SetTrigger("isAttack");
        curTimer = 0;
        isShoot = false;
    }

    public override void Exit()
    {
        foreach (var m in marker)
        {
            GameObject.Destroy(m);
        }
        marker.Clear();
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }
    
    
    protected override void ChangeState()
    {
        foreach (var m in marker)
        {
            m.transform.localScale += Vector3.one * Time.deltaTime / changeTimer;
        }
        if (curTimer > changeTimer && isShoot)
        {
            base.ChangeState();
            curTimer = 0;
        }
    }


    public override void Update()
    {
        base.Update();
        // Bullet이 충돌할 경우 다음 스테이트로 이동.
        curTimer += Time.deltaTime;
        ShootDelay();
        ChangeState();
    }

    //====================================================
    /////                   CoreMethods                 /////
    //====================================================
    void CreateMarker()
    {
        targets.Clear();
        // Player 기준 원 범위 서치
        for (int i = 0; i < targetCount; i++)
            targets.Add(Search());

        foreach (var t in targets)
        {
            GameObject _obj = GameObject.Instantiate(circleObj);
            _obj.GetComponentInChildren<Transform>().localScale = Vector3.one * .3f;
            _obj.transform.rotation = Quaternion.Euler(-90, 0, 0);
            _obj.transform.position = t;
            marker.Add(_obj);
        }
    }

    private void PositionLuncher()
    {
        foreach (var t in targets)
        {
            Vector3 pos = IpariUtility.CaculateVelocity(t, shootPoint.position, time);
            GameObject obj = GameObject.Instantiate(AcidBullet, shootPoint.position, Quaternion.identity);
            obj.GetComponent<Bullet>().ShotDirection(pos);
        }
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

    public void ShootDelay()
    {
        if (curTimer > DelayTime && !isShoot)
        {
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Nepenthes_Shoot);
            CreateMarker();
            PositionLuncher();
            curTimer = 0;
            isShoot = true;
        }
    }
}
