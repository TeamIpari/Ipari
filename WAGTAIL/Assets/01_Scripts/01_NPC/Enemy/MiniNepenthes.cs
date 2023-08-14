using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniNepenthes : Enemy
{
    [Header("MiniNepenthes Inspector")]
    public Bullet BulletPrefab;
    public Transform ShotPosition;
    public float ShotSpeed;
    public int angle;


    // Start is called before the first frame update
    void Start()
    {
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new NepenthesIdleState(AiSM, angle);
        AiAttack = new NepenthesAttackState(AiSM);
        AiWait = new NepenthesWaitState(AiSM);
        
        AiSM.Initialize(AiIdle);

        AttackTimer = 0;
        if (ShotSpeed <= 0)
            ShotSpeed = 1.0f;
        if (WaitRate <= 0)
            WaitRate = 1.0f;
        if (AttackRange <= 0)
            AttackRange = 5.0f;
    }

    public override void CAttack()
    {
        //base.CAttack();
        Debug.Log("네펜데스의 공격");

        // 방향 벡터를 구하고 해당 방향으로 탄환을 쏨.
        Vector3 PlayerPos = new Vector3(AiSM.Target.transform.position.x, ShotPosition.position.y, AiSM.Target.transform.position.z);
        Vector3 direction = PlayerPos - ShotPosition.position;

        BulletPrefab.SetDirection(direction);
        GameObject Bomb = Instantiate(BulletPrefab.gameObject);
        Bomb.GetComponent<Bullet>().SetDirection(direction * ShotSpeed);
        Bomb.transform.position = ShotPosition.position;
    }


    // Update is called once per frame
    void Update()
    {
        isAttack();
        if (AiSM != null)
            AiSM.CurrentState.Update();
    }
}
