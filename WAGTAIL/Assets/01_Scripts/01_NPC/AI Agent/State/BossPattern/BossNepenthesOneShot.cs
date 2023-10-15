using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossNepenthesOneShot: AIAttackState
{

    //========================================
    //////      Property And Fields      /////
    //========================================
    private float changeTimer = 2;

    private Transform shootPoint;
    private GameObject bullet;
    private GameObject circleObj;
    private Vector3 target;
    private GameObject marker;

    private bool isShoot;
    private float delayTime;
    private float time;
    private float bombSize;


    //========================================
    /////       Magic Methods             ////
    //========================================
    public BossNepenthesOneShot(AIStateMachine stateMachine, BossNepenthesProfile profile, float size,float time) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.circleObj = profile.ShotMarker;
        this.shootPoint = profile.ShotPosition;
        this.bullet = profile.BulletPrefab;
        this.time = time;
        this.bombSize = size;
        this.delayTime = 1f;
    }

    public override void Enter()
    {
        // Å¸°Ù ¼³Á¤.
        AISM.Animator.SetTrigger("isAttack");
        
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
        base.Update();
        if (Player.Instance.isDead == true) return;
        curTimer += Time.deltaTime;
        ShootDelay();
        ChangeState();
    }


    //==============================================
    /////             Function methods              ////
    //==============================================
    public void ShootDelay()
    {
        if (curTimer > delayTime && !isShoot)
        {
            CreateMarker();
            PositionLuncher();
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Nepenthes_Shoot);
            curTimer = 0;
            isShoot = true;
        }
    }
    
    private void CreateMarker()
    {
        target = new Vector3(Player.Instance.transform.position.x,
            -0.5f,
            Player.Instance.transform.position.z);
        
        GameObject _obj = GameObject.Instantiate(circleObj);
        _obj.transform.localScale = Vector3.one * bombSize;
        _obj.transform.position = new Vector3(target.x , -0.5f, target.z);
        _obj.transform.rotation = Quaternion.Euler(-90, 0, 0);
        marker = _obj;
    }

    private void PositionLuncher()
    {
        //Vector3 pos = CaculateVelocity(target, shootPoint.position, time);
        Vector3 pos = IpariUtility.CaculateVelocity(target, shootPoint.position, time);
        GameObject obj = GameObject.Instantiate(bullet, shootPoint.position, Quaternion.identity);
        obj.transform.localScale = Vector3.one * bombSize;
        Debug.Log(obj.GetComponent<Bullet>());
        obj.GetComponent<Bullet>().ShotDirection(pos);
        obj.GetComponent<Bullet>().SetMarker(marker);

    }



}
