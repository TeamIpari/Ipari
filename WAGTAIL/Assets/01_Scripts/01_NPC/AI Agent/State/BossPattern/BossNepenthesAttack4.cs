using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BossNepenthesAttack4 : AIAttackState
{
    // �ʿ��� ������ 
    // ������ ����
    //====================================================
    /////               Properties                    /////
    //====================================================
    private GameObject FruitPrefab;
    private GameObject[] FruitPools;
    private int FruitCount;
    private float curTimer;
    

    //====================================================
    /////               Magic Methods                    /////
    //====================================================
    public BossNepenthesAttack4(AIStateMachine stateMachine, GameObject Fruit, int FruitCount) : base(stateMachine)
    {
        this.FruitPrefab = Fruit;
        this.FruitCount = FruitCount;
        CreateFruits();
    }

    //====================================================
    /////               Override                    /////
    //====================================================
    public override void Enter()
    {
        //base.Enter();
        // ���� ��ġ ����
        FruitSetting();
        Debug.Log($"Create Fruit Bomb");
    }

    public override void Exit()
    {
        //base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        //base.OntriggerEnter(other);
    }

    public override void Update()
    {
        base.Update();
        curTimer += Time.deltaTime;
        if(curTimer > 3f)   
            ChangeState();
    }

    protected override void ChangeState()
    {
        base.ChangeState();
    }


    //====================================================
    /////               Core Methods               /////
    //====================================================
    private void CreateFruits()
    {
        FruitPools = new GameObject[FruitCount];

        // ���� �ϴ� ���.
        for(int i = 0; i < FruitCount; i++) 
        {
            GameObject obj = GameObject.Instantiate<GameObject>(FruitPrefab);
            obj.transform.position = Vector3.zero;
            FruitPools[i] = obj;
            FruitPools[i].SetActive(false);
        }
    }
    
    private void FruitSetting()
    {
        // n���� ��ǥ�� �����ϰ� �ش� ��ǥ�� ��������.
        
        int x, z;
        if (FruitPools[0] == null)
            CreateFruits();
        foreach(var fruit in FruitPools)
        {
            x = Random.Range(0, BossRoomFildManager.Instance.XSize);
            z = Random.Range(0, BossRoomFildManager.Instance.YSize);

            fruit.SetActive(true);
            Debug.Log($"x : {x} , z :{z} ");
            fruit.transform.position = BossRoomFildManager.Instance.GetTilePos(x, z);
        }
    }

}
