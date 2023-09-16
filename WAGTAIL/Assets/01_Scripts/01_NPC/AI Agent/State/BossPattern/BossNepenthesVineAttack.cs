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

    private float DelayTime = 5f;
    private GameObject VinePrefab;
    private GameObject Vine;


    //=================================================
    /////               Magic Methods              /////
    //=================================================
    public BossNepenthesVineAttack(
        AIStateMachine stateMachine, GameObject Vine) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.VinePrefab = Vine;
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
        if (curTimer > DelayTime)
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
        if (Vine == null)
            Vine = GameObject.Instantiate(VinePrefab, BossRoomFieldManager.Instance.transform);
        else
            Vine.SetActive(true);
        Vine.transform.localPosition = new Vector3(spawnPos.x, -1.0f, 1.5f);

        // �� �� �� �������� �ϱ�.
        BossRoomFieldManager.Instance.BrokenPlatform(spawnPos.x, true);
    }
}
