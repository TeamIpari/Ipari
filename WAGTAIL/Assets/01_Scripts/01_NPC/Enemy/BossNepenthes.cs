using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthes : Enemy
{
    [Header("Bullet Prefab")]
    public GameObject BulletPrefab;
    public Transform ShotPosition;
    public GameObject ShotMarker;

    [Header("Attack2 Parameter")]
    public float time;

    [Header("Attack3 Parameter")]
    [Tooltip("한번 쏠 때 최대 몇 개를 퍼뜨리는가?")]
    public int ShotCount = 0;
    [Tooltip("폭탄을 던졌을 때 Player중심으로 ShotArea의 원 범위 안에 랜덤으로 투척")]
    public int ShotArea;


    AIAttackState AiAttack2;
    AIAttackState AiAttack3;


    public override void SetAttackPattern()
    {
        base.SetAttackPattern();



    }

    void StateSetting()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new BossNepenthesIdleState(AiSM);
        AiWait = new NepenthesWaitState(AiSM);
        AiAttack = new BossNepenthesAttack1(AiSM);
        AiAttack2 = new BossNepenthesAttack2(AiSM, BulletPrefab, ShotPosition, ShotMarker, time);
        AiAttack3 = new BossNepenthesAttack3(AiSM, BulletPrefab, ShotPosition, ShotMarker, time, ShotCount, ShotArea);
        // 죽는 기능.

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
            Debug.LogWarning("보스의 공격 패턴이 지정되지 않았습니다.");
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
                    Debug.Log("Move가 존재하지 않음.");
                    break;
                case Pattern.WAIT:
                    //AddPattern(AiWait);
                    Debug.Log("Wait가 존재하지 않음.");
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
                    Debug.Log("Die가 존재하지 않음.");
                    break;
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // list로 설정된 공격 패턴을 입력함.
        SettingPattern();
        AiSM.CurrentState = AiSM.Pattern[0];
        //Debug.Log(AiSM.CurrentState != null);

    }

    // Update is called once per frame
    void Update()
    {
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }
}
