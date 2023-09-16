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

    [Header("Attack1 Parameter")]
    public GameObject VinePrefab;

    [Header("Attack2 Parameter")]
    public float Time;
    public float BigSize = 3f;
    public float SmallSize = 1f;

    [Header("Attack3 Parameter")]
    [Tooltip("�ѹ� �� �� �ִ� �� ���� �۶߸��°�?")]
    public int ShotCount = 0;
    [Tooltip("��ź�� ������ �� Player�߽����� ShotArea�� �� ���� �ȿ� �������� ��ô")]
    public int ShotArea;

    //[Header("Attack4 Parameter")]

    //==========================================
    /////           Magic Method            ////
    //==========================================
    void Awake()
    {
        Debug.Log($"{CurPhaseHpArray}");
        SetProfile();
        StateSetting();
        SettingPattern(CharacterMovementPattern[CurPhaseHpArray].EPatterns);
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

    public override void SettingPattern(MonsterPattern.Pattern[] _pattern)
    {
        base.SettingPattern(_pattern);
    }

    void Update()
    {
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        // ������ ����.
        if (other.CompareTag("Bullet"))
        {
            // �������� �󸶳�?
            HP -= other.GetComponent<Bullet>().Damage;
            if (HP < 0)
            {    // State �ٲ��ֱ�.
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
        AiAttack = new BossNepenthesVineAttack(AiSM, VinePrefab);
        AiAttack2 = new BossNepenthesOneShot(AiSM, BossProfile, BigSize, Time);
        AiAttack3 = new BossNepenthesSmallShotGun(AiSM, BossProfile, Time, ShotCount, ShotArea);
        AiAttack4 = new BossNepenthesOneShot(AiSM, BossProfile, SmallSize, Time);
        // �״� ���.
        AiHit = new BossNepenthesHitState(AiSM);
        AiDie = new BossNepenthesDieState(AiSM);
    }

    public void SetProfile()
    {
        BossProfile.SetProfile(BulletPrefab, ShotPosition, ShotMarker);
    }
}
