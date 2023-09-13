using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BossNepenthesAttack4 : AIAttackState
{
    // 필요한 데이터 
    // 생성할 갯수
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
        // 과일 위치 세팅
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

        // 생성 하는 기능.
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
        // n개의 좌표를 지정하고 해당 좌표에 생성해줌.
        
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
