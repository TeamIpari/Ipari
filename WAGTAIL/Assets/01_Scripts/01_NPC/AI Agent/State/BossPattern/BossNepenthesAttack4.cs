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
    public int aa;

    //====================================================
    /////               Magic Methods                    /////
    //====================================================
    public BossNepenthesAttack4(AIStateMachine stateMachine, GameObject Fruit, int FruitCount) : base(stateMachine)
    {
        this.FruitPrefab = Fruit;
        aa = FruitCount;
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
        //base.Update();/
    }

    protected override void ChangeState()
    {
        //base.ChangeState();
    }


    //====================================================
    /////               Core Methods               /////
    //====================================================
    private void CreateFruits()
    {
        FruitPools = new GameObject[aa];

        // ���� �ϴ� ���.
        for(int i = 0; i < aa; i++) 
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
