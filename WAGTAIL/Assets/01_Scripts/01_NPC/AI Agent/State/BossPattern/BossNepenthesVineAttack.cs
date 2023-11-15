using System.Collections;
using System.Threading;
using Unity.VisualScripting;
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

    private Thread myThread;
    private VineState myState = VineState.STATE_NONE;

    //=================================================
    // data type Property
    //=================================================
    private float delayTime = 4f;
    private float attackPoint = 0;
    private bool isLeft = false;
    private bool isThread = false;

    //================================================
    // Property
    //================================================
    private BossNepenthes boss;
    private GameObject rightVine;
    private GameObject leftVine;
    private GameObject vine;
    private GameObject bomb;
    private GameObject fx;
    private ChildPlatformsShaker shaker;

    private Vector3 rightOriginPos;
    private Vector3 leftOriginPos;
    private Vector3 movingPos;
    private Collider[] colliders = new Collider[10];
    private GameObject[] BombPools = new GameObject[3];



    private Animator vineAnim;

    #endregion

    //=================================================
    /////               Magic Methods              /////
    //=================================================
    public BossNepenthesVineAttack(
        AIStateMachine stateMachine, BossNepenthes boss, GameObject leftVine, GameObject rightVine ,GameObject fx, GameObject bomb, ChildPlatformsShaker shaker) : base(stateMachine)
    {
        #region Omit
        this.AISM = stateMachine;
        this.leftVine = leftVine;
        leftOriginPos = leftVine.transform.position;
        this.rightVine = rightVine;
        rightOriginPos = rightVine.transform.position;
        this.shaker = shaker;
        this.boss = boss;
        this.fx = fx;
        this.bomb = bomb;
        #endregion
    }

    public override void Enter()
    {
        curTimer = 0;
        ShowVine();
        myState = VineState.STATE_MOVE;
        vineAnim = vine.GetComponent<Animator>();
    }


    public override void Exit()
    {
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        #region Omit
        base.Update();
        if (Player.Instance.isDead == true) return;
        VineStateChange();
        ChangeState();
        #endregion
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

    private void VineStateChange()
    {
        switch (myState)
        {
            case VineState.STATE_NONE:
                break;
            case VineState.STATE_MOVE:
                {
                    MovementVine(vine.transform.position, movingPos);
                    break;
                }
            case VineState.STATE_ATTACK:
                {
                    //// 공격하게 함.
                    //// 바로 공격
                    if (!isThread)
                    {
                        isThread = true;
                        myThread = new Thread(new ThreadStart(ThreadFunction));
                        myThread.Start();

                        int Count = Physics.OverlapBoxNonAlloc(
                            vine.transform.position,
                            new Vector3(0.5f, 1.5f, 20f),
                            colliders,
                            vine.transform.rotation,
                            (1 << LayerMask.NameToLayer("Platform")),
                            QueryTriggerInteraction.Ignore
                        );
                        if (Count > 0)
                        {
                            boss.CoroutineFunc(ShakePlatforms, 0.3f);
                        }
                    }
                }
                break;
            case VineState.STATE_ORIGINBACK:
                {
                    GotoMoveOrigin();
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator ShakePlatforms(float time)
    {
        vineAnim.SetBool("isAttack", true);
        vineAnim.speed *= 1.7f;
        float x, z;
        yield return new WaitForSeconds(time);
        vineAnim.SetBool("isAttack", false);
        shaker.MakeWave(colliders);
        Vector3 vec = new Vector3(movingPos.x, movingPos.y, -4.5f);
        for (int i = 0; i < 4; i++)
        {
            GameObject tempFx = GameObject.Instantiate(fx, vec, fx.transform.rotation);
            GameObject.Destroy(tempFx, 1f);
            vec += Vector3.forward * -3f;
        }

        if (BombPools[0] == null)
            CreateReActionObject();
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_VineSmash, Vector3.zero, 2f);
        CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, 0.8f, .05f);

        foreach (var fruit in BombPools)
        {
            x = Random.Range(-6f, 6f);
            z = Random.Range(-6f, -12f);

            fruit.SetActive(true);
            fruit.transform.position = new Vector3(x, 13f, z);
            yield return new WaitForSeconds(0.050f);
        }
        vineAnim.speed = 1f;

    }

    private void CreateReActionObject()
    {
        BombPools = new GameObject[3];

        // 생성 하는 기능.
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(bomb);
            obj.transform.position = Vector3.zero;
            BombPools[i] = obj;
            BombPools[i].SetActive(false);
        }
    }


    private void GotoMoveOrigin()
    {
        MovementVine(vine.transform.position, isLeft == true ? leftOriginPos : rightOriginPos);
        isThread = false;
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
        movingPos = Player.Instance.transform.position;

        vine = movingPos.x < AISM.Transform.position.x ? leftVine : rightVine;
        isLeft = movingPos.x < AISM.Transform.position.x ? true : false;

        movingPos = shaker.transform.position + new Vector3(movingPos.x, vine.transform.position.y + 2.8f, vine.transform.position.z - 2f);
    }
}

// 몰래 넣은 스레드
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
