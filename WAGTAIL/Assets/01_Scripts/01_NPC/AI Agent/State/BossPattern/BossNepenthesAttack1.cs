using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BossNepenthesAttack1 : AIAttackState
{

    //=================================================
    /////           Property And Fields             ////
    //=================================================
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


    //=================================================
    /////               Magic Methods              /////
    //=================================================
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
        ShowVine();
    }


    public override void Exit()
    {
        //GameObject.Destroy(Vine);
        // 오브젝트 풀 개념으로 한번의 생성 이후 그 다리만 씀.
        Vine.SetActive(false);

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        base.Update();  
        curTimer += Time.deltaTime;
        if(curTimer > DelayTime)
            stateMachine.NextPattern();
    }

    //=================================================
    /////               Core Methods              /////
    //=================================================
    public void ShowVine()
    {
        Vector3 spawnPos = BossRoomFildManager.Instance.PlayerOnTilePos;
        bool isLeft = spawnPos.x < BossRoomFildManager.Instance.XSize / 2;

        // 작을 경우 왼쪽 덩쿨 출력
        if (Vine == null)
        {
            Vine = GameObject.Instantiate(isLeft ? RightVinePrefab : LeftVinePrefab, BossRoomFildManager.Instance.transform);
            Vine.transform.localPosition = new Vector3(spawnPos.x, -1.0f, 1.5f);
        }
        else
        {
            Vine.SetActive(true);
            Vine.transform.localPosition = new Vector3(spawnPos.x, -1.0f, 1.5f);
        }
        // 몇 초 후 떨어지게 하기.
        BossRoomFildManager.Instance.BrokenPlatform(spawnPos.x);
    }
}
