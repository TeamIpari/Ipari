using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct BossNepenthesProfile
{
    public GameObject BulletPrefab;
    public Transform ShotPosition;
    public GameObject ShotMarker;

    public void SetProfile(GameObject Bullet, Transform point, GameObject ShotMarker)
    {
        this.BulletPrefab = Bullet;
        this.ShotPosition = point;
        this.ShotMarker = ShotMarker;
    }
}

public class BossNepenthes : Enemy
{
    //==============================================
    /////       Propertys and Fields            ////
    //==============================================
    [Header("Bullet Prefab")]
    BossNepenthesProfile BossProfile;
    public GameObject BulletPrefab;
    public Transform ShotPosition;
    public GameObject ShotMarker;
    public GameObject MiniShotMarker;

    [Header("Attack1 Parameter")]
    public GameObject LeftVine;
    public GameObject RightVine;
    public GameObject VinePrefab;

    [Header("Attack2 Parameter")]
    private const float flyTime = 2f;
    private const float bigSize = 3f;
    private  const float SmallSize = 1f;

    [Header("Attack3 Parameter")]
    [Tooltip("한번 쏠 때 최대 몇 개를 퍼뜨리는가?")]
    public int ShotCount = 0;
    [Tooltip("폭탄을 던졌을 때 Player중심으로 ShotArea의 원 범위 안에 랜덤으로 투척")]
    public int ShotArea;

    //[Header("Attack4 Parameter")]

    //==========================================
    /////           Magic Method            ////
    //==========================================
    void Awake()
    {
        SetProfile(ShotMarker);
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        AiSM.CurrentState = AiSM.Pattern[0];
    }

    public override void SetAttackPattern()
    {
        base.SetAttackPattern();
    }

    protected override void AddPattern(AIState curPattern)
    {
        base.AddPattern(curPattern);
    }

    public override void SettingPattern(MonsterPattern.Pattern[] pattern)
    {
        base.SettingPattern(pattern);
    }

    void Update()
    {
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 공격을 받음.
        if (other.CompareTag("Bullet"))
        {
            // 데미지는 얼마나?
            HP -= other.GetComponent<Bullet>().Damage;
            if (HP < 0)
            {    // State 바꿔주기.
                AiSM.ChangeState(AiHit);
            }
            else
            {
                AiSM.ChangeState(AiDie);
            }
        }
    }

    // ============================================
    /////           Core Methods            ///
    // ============================================
    void StateSetting()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new BossNepenthesIdleState(AiSM, IdleRate);
        AiWait = new BossNepenthesWaitState(AiSM, WaitRate);
        AiAttack = new BossNepenthesVineAttack(AiSM, LeftVine, RightVine);
        AiAttack2 = new BossNepenthesOneShot(AiSM, BossProfile, bigSize, flyTime);
        SetProfile(MiniShotMarker);
        AiAttack3 = new BossNepenthesSmallShotGun(AiSM, BossProfile, flyTime, ShotCount, ShotArea);
        AiAttack4 = new BossNepenthesOneShot(AiSM, BossProfile, SmallSize, flyTime);
        // 죽는 기능.
        AiHit = new BossNepenthesHitState(AiSM);
        AiDie = new BossNepenthesDieState(AiSM);
    }

    public void SetProfile(GameObject ShotMarker)
    {
        BossProfile.SetProfile(BulletPrefab, ShotPosition, ShotMarker);
    }
}
