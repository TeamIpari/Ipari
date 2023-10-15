using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
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
    public GameObject FX_Hitprefab;
    private GameObject FX_Hit;
    public Transform hitTrasnform;

    [Header("Attack1 Parameter")]
    public GameObject LeftVine;
    public GameObject RightVine;
    public GameObject VinePrefab;

    [Header("Attack2 Parameter")]
    private const float flyTime = 2f;
    private const float bigSize = 3f;
    private const float SmallSize = 1f;

    [Header("Attack3 Parameter")]
    [Tooltip("�ѹ� �� �� �ִ� �� ���� �۶߸��°�?")]
    public int ShotCount = 0;
    [Tooltip("��ź�� ������ �� Player�߽����� ShotArea�� �� ���� �ȿ� �������� ��ô")]
    public int ShotArea;
    
    [Header("Next Chapter")]
    [Tooltip("������ �׾��� �� �� ���� �� �̸�")]
    public string nextChapterName;
    private Stack<GameObject> HpCanvas = new Stack<GameObject>();

    //[Header("Attack4 Parameter")]

    //==========================================
    /////           Magic Method            ////
    //==========================================
    void Awake()
    {
        SetProfile(ShotMarker);
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        FX_Hit = GameObject.Instantiate(FX_Hitprefab ,hitTrasnform.position, FX_Hitprefab.transform.rotation ,this.transform.parent);
        FX_Hit.SetActive(false);
        AiSM.CurrentState = AiSM.Pattern[0];
    }

    void Start()
    {
        initializeUI();
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

    public override void initializeUI()
    {
        GameObject obj = GameObject.Find("HPArea");
        //HpCanvas = new GameObject[obj.transform.childCount];
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            var piece = obj.transform.GetChild(i);
            if (piece != null)
            {
                Debug.Log($"{piece}");
                HpCanvas.Push(piece.gameObject);
            }
        }
    }

    public override void Hit()
    {
        base.Hit();
        GameObject hpGage = HpCanvas.Pop();
        hpGage.GetComponent<Animator>().SetTrigger("isDamaged");
        FX_Hit.SetActive(true);

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
        // �״� ���.
        AiHit = new BossNepenthesHitState(AiSM, LeftVine, RightVine);
        AiDie = new BossNepenthesDieState(AiSM, LeftVine, RightVine);
    }

    public void SetProfile(GameObject ShotMarker)
    {
        BossProfile.SetProfile(BulletPrefab, ShotPosition, ShotMarker);
    }
    
    public void GoNextChapter()
    {
        SceneLoader.GetInstance().LoadScene(nextChapterName);
    }

}
