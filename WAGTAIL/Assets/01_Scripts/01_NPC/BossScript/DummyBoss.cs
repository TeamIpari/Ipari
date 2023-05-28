using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;


/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public enum STATES
{
    IDLE,
    MOVE,
    ATTCK1,
    ATTCK2,
    ATTCK3,
}
public class DummyBoss : MonoBehaviour
{
    private AIStateMachine aiStateMachine;
    private AIIdleState idle;
    private AILineMove move;
    private DummyAttack1 attack1;
    private DummyAttack2 attack2;
    private DummyAttack3 attack3;

    public List<STATES> Pattern;

    
    // Effect를 어떻게 표시할까?

    public bool Die = false;
    [Header("Idle")]
    public float MoveTimer;
    public float SearchDistance;

    [Header("Move")]
    public Transform[] Points;

    [Header("Attack1")]
    public GameObject Effect;
    public GameObject Tentacle;

    [Header("Attack3")]
    public Transform ShootPoint;
    public int Count;
    public float SearchRadius;
    public GameObject Bullet;
    public float FlightTime;


    // Start is called before the first frame update
    private void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);
        idle = new AIIdleState(aiStateMachine, MoveTimer, SearchDistance);
        move = new AILineMove(aiStateMachine, Points);
        attack1 = new DummyAttack1(aiStateMachine, Tentacle, Effect);
        attack3 = new DummyAttack3(aiStateMachine, Bullet, ShootPoint, Effect, FlightTime,  Count, SearchRadius);
        attack2 = new DummyAttack2(aiStateMachine, Bullet, ShootPoint, Effect, FlightTime);

        //aiStateMachine.
        //idle.SetChildren(move);
        

        aiStateMachine.Initialize(idle);
        if (Pattern.Count > 0)
        {
            for(int i = 0; i < Pattern.Count; i++)
            { 
                switch (Pattern[i])
                {
                    case STATES.IDLE:
                        aiStateMachine.AddPatern(idle);
                        break;
                    case STATES.MOVE:
                        aiStateMachine.AddPatern(move);
                        break;
                    case STATES.ATTCK1:
                        aiStateMachine.AddPatern(attack1);
                        break;
                    case STATES.ATTCK2:
                        aiStateMachine.AddPatern(attack2);
                        break;
                    case STATES.ATTCK3:
                        aiStateMachine.AddPatern(attack3);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //// Update is called once per frame
    //private void Update()
    //{
    //    //aiStateMachine.currentState.Update();
    //}

    private void FixedUpdate()
    {
        aiStateMachine.CurrentState.Update();
    }
}
