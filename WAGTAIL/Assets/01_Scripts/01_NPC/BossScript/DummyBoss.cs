using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;


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
    AIStateMachine aiStateMachine;
    AIIdleState idle;
    AILineMove move;
    DummyAttack1 attack1;
    DummyAttack2 attack2;
    DummyAttack3 attack3;

    public List<STATES> Pattern;

    public bool Die = false;
    [Header("Idle")]
    public float moveTimer;
    public float SearchDistance;

    [Header("Move")]
    public Transform[] points;
    //public Transform curPoint;
        
    // Start is called before the first frame update
    void Start()
    {
        aiStateMachine = AIStateMachine.CreateFormGameObject(gameObject);
        idle = new AIIdleState(aiStateMachine, moveTimer, SearchDistance);
        move = new AILineMove(aiStateMachine, points);
        attack1 = new DummyAttack1(aiStateMachine);
        attack2 = new DummyAttack2(aiStateMachine);
        attack3 = new DummyAttack3(aiStateMachine);

        //aiStateMachine.
        //idle.SetChildren(move);
        

        aiStateMachine.Initialize(idle);
        if (Pattern.Count > 0)
        {
            for(int i = 1; i < Pattern.Count; i++)
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

    // Update is called once per frame
    void Update()
    {
        //aiStateMachine.currentState.Update();
    }

    public void FixedUpdate()
    {
        aiStateMachine.currentState.Update();
    }
}
