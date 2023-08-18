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

    private bool isShoot;
    private float DelayTime;
    private float time;



    public BossNepenthesAttack2(AIStateMachine stateMachine, BossNepenthesProfile profile, float time) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.circleObj = profile.ShotMarker;
        this.shootPoint = profile.ShotPosition;
        this.bullet = profile.BulletPrefab;
        this.time = time;
        this.DelayTime = 0.8f;
    }

    public override void Enter()
    {
        // Å¸°Ù ¼³Á¤.
        stateMachine.Animator.SetTrigger("isAttack");
        isShoot = false;
        curTimer = 0;
    }

    public override void Exit()
    {
        GameObject.Destroy(marker);
        marker = null;
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void ShootDelay()
    {
        if (curTimer > DelayTime && !isShoot)
        {
            CreateMarker();
            PositionLuncher();
            curTimer = 0;
            isShoot = true;
        }
    }
    
    protected override void ChangeState()
    {
        if (curTimer > changeTimer && isShoot)
        {
            base.ChangeState();
            curTimer = 0;
        }
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        ShootDelay();
        ChangeState();
    }

    private void CreateMarker()
    {
        target = new Vector3(Player.Instance.transform.position.x,
            Player.Instance.transform.position.y + 0.1f,
            Player.Instance.transform.position.z);
        //target = BossRoomFildManager.Instance.TargetPos;
        
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
