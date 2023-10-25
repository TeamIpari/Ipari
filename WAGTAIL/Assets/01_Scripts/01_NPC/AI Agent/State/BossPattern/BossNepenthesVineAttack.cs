using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossNepenthesVineAttack : AIAttackState
{
    #region Properties
    //=================================================
    /////           Property And Fields             ////
    //=================================================

    public enum VineState : int
    {
        STATE_NONE = 0,
        STATE_MOVE = 1,
        STATE_ATTACK = 2,
        STATE_ORIGINBACK = 3,
        STATE_END = 4,
    }

    public delegate void AddListItem(String myString);
    AddListItem myDelegate;
    private Thread myThread;
    private VineState myState = VineState.STATE_NONE;

    //=================================================
    // data type Property
    //=================================================
    private float delayTime = 5f;
    private float attackPoint = 0;
    private bool isLeft = false;
    private bool isThread = false;

    //================================================
    // Unity Property
    //================================================
    private GameObject rightVine;
    private GameObject leftVine;
    private GameObject vine;

    private Vector3 rightOriginPos;
    private Vector3 leftOriginPos;
    private Vector3 spawnPos;

    private Animator vineAnim;

    #endregion

    //=================================================
    /////               Magic Methods              /////
    //=================================================
    public BossNepenthesVineAttack(
        AIStateMachine stateMachine, GameObject leftVine, GameObject rightVine) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.leftVine = leftVine;
        leftOriginPos = leftVine.transform.position;
        this.rightVine = rightVine;
        rightOriginPos = rightVine.transform.position;
        //curAnim = 0;
    }

    public override void Enter()
    {
        curTimer = 0;
        ShowVine();
        myState = VineState.STATE_MOVE;
        vineAnim = vine.GetComponent<Animator>();
        //vineAnim.SetTrigger("isReady");

    }


    public override void Exit()
    {
        //GameObject.Destroy(Vine);
        // 오브젝트 풀 개념으로 한번의 생성 이후 그 다리만 씀.
        // move to transform // 어떻게 움직일 것인가?
        

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        base.Update();
        if (Player.Instance.isDead == true) return;
        switch (myState)
        {
            case VineState.STATE_NONE:
                break;
            case VineState.STATE_MOVE:
                {
                    vineAnim.SetBool("isAttack", true);
                    MovementVine(vine.transform.position, spawnPos);
                    break;
                }
            case VineState.STATE_ATTACK:
                {
                    vineAnim.SetBool("isAttack", false);
                    //// 공격하게 함.
                    //// 바로 공격
                    if (!isThread)
                    {
                        isThread = true;
                        myThread = new Thread(new ThreadStart(ThreadFunction));
                        myThread.Start();
                        BossRoomFieldManager.Instance.BreakingPlatform(spawnPos.x, true);
                    }
                }
                break;
            case VineState.STATE_ORIGINBACK:
                {
                    GotoMoveOrigin();
                    isThread = false;
                }
                break;
            default:
                break;
        }

        ChangeState();
    }

    protected override void ChangeState()
    {
        if(myState != VineState.STATE_ATTACK && myState != VineState.STATE_MOVE)
            curTimer += Time.deltaTime;
        if (curTimer > delayTime)
            base.ChangeState();
    }

    //=================================================
    /////               Core Methods              /////
    //=================================================

    // Thread
    private void ThreadFunction() 
    {
        MyThreedClass myThreadClassObject = new MyThreedClass(this);
        myThreadClassObject.Run();
    }

    private void VineAttack()
    {
        BossRoomFieldManager.Instance.BreakingPlatform(attackPoint, true);
    }

    private void GotoMoveOrigin()
    {
        MovementVine(vine.transform.position, isLeft == true ? leftOriginPos : rightOriginPos);
    }


    private void MovementVine(Vector3 originPos, Vector3 targetPos)
    {
        if (vine != null && Vector3.Distance(vine.transform.position, targetPos) >= 0.001f)
        {
            vine.transform.position = Vector3.MoveTowards(originPos, targetPos, Time.deltaTime * 7f);
            if (Vector3.Distance(vine.transform.position, targetPos) < 0.001f)
            {
                eStateChange();
            }
        }
    }

    public void eStateChange()
    {
        myState += 1;
        myState = myState == VineState.STATE_END ? VineState.STATE_NONE : myState;
    }

    private void ShowVine()
    {
        spawnPos = BossRoomFieldManager.Instance.PlayerOnTilePos;
        float xpos = (spawnPos.x - 1.5f) / 3;
        // 작을 경우 왼쪽 덩쿨 출력

        // 생성이 아닌 지정으로 해야 함.

        vine = xpos < 3 ? leftVine : rightVine;
        isLeft = xpos < 3 ? true : false;

        //if (vine == null)
        //    vine = GameObject.Instantiate(rightVine, BossRoomFieldManager.Instance.transform);
        //else
        //    vine.SetActive(true);
        vine.transform.parent = BossRoomFieldManager.Instance.transform;



        // 몇 초 후 떨어지게 하기.
        spawnPos = BossRoomFieldManager.Instance.transform.position + new Vector3(spawnPos.x, vine.transform.position.y + 2.8f, vine.transform.position.z);
    }
}

public class MyThreedClass
{
    BossNepenthesVineAttack vineAttack;
    public MyThreedClass(BossNepenthesVineAttack vineAttack)
    {
        this.vineAttack = vineAttack;
    }

    public void Run()
    {
        Thread.Sleep(1000);
        vineAttack.eStateChange();
        
    }
}
