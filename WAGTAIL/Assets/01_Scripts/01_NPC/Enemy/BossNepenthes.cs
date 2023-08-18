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
    [Header("Bullet Prefab")]
    BossNepenthesProfile BossProfile;
    public GameObject BulletPrefab;
    public Transform ShotPosition;
    public GameObject ShotMarker;

    [Header("Attack2 Parameter")]
    public float time;

    [Header("Attack3 Parameter")]
    [Tooltip("�ѹ� �� �� �ִ� �� ���� �۶߸��°�?")]
    public int ShotCount = 0;
    [Tooltip("��ź�� ������ �� Player�߽����� ShotArea�� �� ���� �ȿ� �������� ��ô")]
    public int ShotArea;


    public GameObject LeftVine;
    public GameObject RightVine;



    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log($"{CurPhaseHpArray}");
        SetProfile();
        StateSetting();
        // list�� ������ ���� ������ �Է���.
        SettingPattern(CharacterMovementPattern[CurPhaseHpArray].EPatterns);
        //SettingPattern(Phase2);
        //SettingPattern(Phase3);
        AiSM.CurrentState = AiSM.Pattern[0];
    }
    public void SetProfile()
    {
        BossProfile.SetProfile(BulletPrefab, ShotPosition, ShotMarker);
    }

    public override void SetAttackPattern()
    {
        base.SetAttackPattern();

    }

    void StateSetting()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new BossNepenthesIdleState(AiSM, WaitRate);
        AiWait = new BossNepenthesWaitState (AiSM, WaitRate);
        AiAttack = new BossNepenthesAttack1(AiSM, LeftVine, RightVine);
        AiAttack2 = new BossNepenthesAttack2(AiSM, BossProfile, time);
        AiAttack3 = new BossNepenthesAttack3(AiSM, BossProfile, time, ShotCount, ShotArea);

        // �״� ���.
        AiHit = new BossNepenthesHitState(AiSM);
        AiDie = new BossNepenthesDieState(AiSM);


    }

    protected override void AddPattern(AIState curPattern)
    {
        base.AddPattern(curPattern);
    }

    public override void SettingPattern(MonsterPattern.Pattern[] _pattern)
    {
        base.SettingPattern(_pattern);
    }

    // Update is called once per frame
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

}
