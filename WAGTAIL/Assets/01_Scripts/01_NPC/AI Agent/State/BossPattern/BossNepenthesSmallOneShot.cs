using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BossNepenthesSmallOneShot : AIAttackState
{
    // 필요한 데이터 
    // 생성할 갯수
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
        // 과일 위치 세팅
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
        // 생성 하는 기능.
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
        // n개의 좌표를 지정하고 해당 좌표에 생성해줌.
        
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
