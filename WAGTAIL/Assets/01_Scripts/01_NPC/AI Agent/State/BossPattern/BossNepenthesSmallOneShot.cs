using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BossNepenthesSmallOneShot : AIAttackState
{
    // �ʿ��� ������ 
    // ������ ����
    //====================================================
    /////               Properties                    /////
    //====================================================
    private GameObject fruitPrefab;
    private GameObject[] fruitPools;
    private int fruitCount;
    private float curTimer;
    

    //====================================================
    /////               Magic Methods                    /////
    //====================================================
    public BossNepenthesSmallOneShot(AIStateMachine stateMachine, GameObject fruit, int fruitCount) : base(stateMachine)
    {
        this.fruitPrefab = fruit;
        this.fruitCount = fruitCount;
        CreateFruits();
    }

    //====================================================
    /////               Override                    /////
    //====================================================
    public override void Enter()
    {
        //base.Enter();
        // ���� ��ġ ����
        curTimer = 0;
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
        fruitPools = new GameObject[fruitCount];
        // ���� �ϴ� ���.
        for(int i = 0; i < fruitCount; i++) 
        {
            GameObject obj = GameObject.Instantiate<GameObject>(fruitPrefab);
            obj.transform.position = Vector3.zero;
            fruitPools[i] = obj;
            fruitPools[i].SetActive(false);
        }
    }
    
    private void FruitSetting()
    {
        // n���� ��ǥ�� �����ϰ� �ش� ��ǥ�� ��������.
        
        int x, z;
        if (fruitPools[0] == null)
            CreateFruits();
        foreach(var fruit in fruitPools)
        {
            x = Random.Range(0, BossRoomFieldManager.Instance.XSize);
            z = Random.Range(0, BossRoomFieldManager.Instance.YSize);

            fruit.SetActive(true);
            Debug.Log($"x : {x} , z :{z} ");
            fruit.transform.position = BossRoomFieldManager.Instance.GetTilePos(x, z);
        }
    }

}
