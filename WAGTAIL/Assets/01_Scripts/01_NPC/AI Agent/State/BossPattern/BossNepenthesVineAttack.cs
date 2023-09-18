using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesVineAttack : AIAttackState
{

    //=================================================
    /////           Property And Fields             ////
    //=================================================
    //private int curAnim = 0;
    private float curTimer = 0;
    //private float changeTimer = 5f;

    //private float curShowTimer = 0;
    //private float showTimer = 0.5f;

    //private bool on = false;

    private float delayTime = 5f;
    private GameObject vinePrefab;
    private GameObject vine;


    //=================================================
    /////               Magic Methods              /////
    //=================================================
    public BossNepenthesVineAttack(
        AIStateMachine stateMachine, GameObject vine) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.vinePrefab = vine;
        //curAnim = 0;
    }

    public override void Enter()
    {
        curTimer = 0;
        ShowVine();
    }


    public override void Exit()
    {
        //GameObject.Destroy(Vine);
        // ������Ʈ Ǯ �������� �ѹ��� ���� ���� �� �ٸ��� ��.
        vine.SetActive(false);

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        base.Update();  
        curTimer += Time.deltaTime;
        if (curTimer > delayTime)
            ChangeState();
    }
    protected override void ChangeState()
    {
        base.ChangeState();
    }

    //=================================================
    /////               Core Methods              /////
    //=================================================
    public void ShowVine()
    {
        Vector3 spawnPos = BossRoomFieldManager.Instance.PlayerOnTilePos;

        // ���� ��� ���� ���� ���
        if (vine == null)
            vine = GameObject.Instantiate(vinePrefab, BossRoomFieldManager.Instance.transform);
        else
            vine.SetActive(true);
        vine.transform.localPosition = new Vector3(spawnPos.x, -1.0f, 1.5f);

        // �� �� �� �������� �ϱ�.
        BossRoomFieldManager.Instance.BrokenPlatform(spawnPos.x, true);
    }
}
