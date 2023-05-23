using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class DummyBoss : MonoBehaviour
{
    AIStateMachine aiStateMachine;
    AIIdleState idle;
    AILineMove move;


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

        

        //aiStateMachine.
        idle.SetChildren(move);

        aiStateMachine.Initialize(idle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        aiStateMachine.currentState.Update();
    }
}
