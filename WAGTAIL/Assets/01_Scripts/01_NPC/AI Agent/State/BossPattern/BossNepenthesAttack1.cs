using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesAttack1 : AIAttackState
{
    private int curAnim = 0;


    private float curTimer = 0;
    private float changeTimer = 5f;

    private float curShowTimer = 0;
    private float showTimer = 0.5f;

    private bool on = false;
    private GameObject dangerousEffect;
    private int count = 0;

    private float DelayTime = 5f;
    private GameObject LeftVinePrefab;
    private GameObject RightVinePrefab;
    private GameObject Vine;


    public BossNepenthesAttack1(AIStateMachine stateMachine, GameObject LeftVine, GameObject RightVine) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.LeftVinePrefab = LeftVine;
        this.RightVinePrefab = RightVine;
        curAnim = 0;
    }

    public override void Enter()
    {
        curTimer = 0;
        // Player�� �����ϴ� Ÿ���� X��ǥ�� ����.
        ShowVine();
        // Idle ����
        //Debug.Log("Start Attack1");
    }


    public override void Exit()
    {
        //dangerousEffect.SetActive(false);
        GameObject.Destroy(Vine);
        //Debug.Log($"End {this.ToString()}");

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void ShowVine()
    {
        Vector3 spawnPos = BossRoomFildManager.Instance.PlayerOnTilePos;
        bool isLeft = spawnPos.x < BossRoomFildManager.Instance.TwitterSize / 2;

        // ���� ��� ���� ���� ���
        Vine = GameObject.Instantiate(isLeft ? RightVinePrefab : LeftVinePrefab, BossRoomFildManager.Instance.transform);
        Vine.transform.localPosition = new Vector3(spawnPos.x, -1.0f, 1.5f);

        // �� �� �� �������� �ϱ�.
        BossRoomFildManager.Instance.BrokenPlatform(spawnPos.x);

    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        if(curTimer > DelayTime)
            stateMachine.NextPattern();
    }
}
