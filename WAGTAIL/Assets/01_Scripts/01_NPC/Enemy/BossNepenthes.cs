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

    AIAttackState AiAttack2;
    AIAttackState AiAttack3;


    // Start is called before the first frame update
    void Awake()
    {
        SetProfile();
        // list�� ������ ���� ������ �Է���.
        SettingPattern();
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
        AiWait = new NepenthesWaitState(AiSM);
        AiAttack = new BossNepenthesAttack1(AiSM, LeftVine, RightVine);
        AiAttack2 = new BossNepenthesAttack2(AiSM, BossProfile, time);
        AiAttack3 = new BossNepenthesAttack3(AiSM, BossProfile, time, ShotCount, ShotArea);
        // �״� ���.

    }

    void AddPattern(AIState curPattern)
    {
        AiSM.AddPatern(curPattern);
    }

    void SettingPattern()
    {
        StateSetting();
        if (CharacterMovementPattern.Length <= 0)
        {
            Debug.LogWarning("������ ���� ������ �������� �ʾҽ��ϴ�.");
        }

        for (int i = 0; i < CharacterMovementPattern.Length; i++)
        {
            switch (CharacterMovementPattern[i])
            {
                case Pattern.IDLE:
                    AddPattern(AiIdle);
                    break;
                case Pattern.MOVE:
                    //AiSM.AddPatern(AiMove);
                    Debug.Log("Move�� �������� ����.");
                    break;
                case Pattern.WAIT:
                    //AddPattern(AiWait);
                    Debug.Log("Wait�� �������� ����.");
                    break;
                case Pattern.SPECAIL1:
                    AddPattern(AiAttack);
                    break;
                case Pattern.SPECAIL2:
                    AddPattern(AiAttack2);
                    break;
                case Pattern.SPECAIL3:
                    AddPattern(AiAttack3);
                    break;
                case Pattern.DIE:
                    //AddPattern(Die);
                    Debug.Log("Die�� �������� ����.");
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }
}
