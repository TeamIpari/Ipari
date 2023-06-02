using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 작성자 : 성지훈
/// 추가 작성
/// </summary>
public class AIStateMachine
{
    public NPCBase NPCBase;
    public Transform Transform;
    public Animator Animator;
    public Rigidbody Physics;
    public NavMeshAgent Agent;
    public SphereCollider SphereCollider;
    public BoxCollider BoxCollider;
    public CapsuleCollider CapsuleCollider;
    public CharacterController CharacterController;
    public GameObject Target;

    public List<AIState> Pattern = new List<AIState>();


    public AIState CurrentState;

    private int cur = 0;

    public static AIStateMachine CreateFormGameObject(GameObject gameObject)
    {
        AIStateMachine ai = new AIStateMachine();
        ai.NPCBase = gameObject.GetComponent<NPCBase>() ;
        ai.Transform = gameObject.transform;
        ai.Animator = gameObject.GetComponent<Animator>();
        ai.Physics = gameObject.GetComponent<Rigidbody>();
        ai.Agent = gameObject.GetComponent<NavMeshAgent>();
        ai.SphereCollider = gameObject.GetComponent<SphereCollider>();
        ai.BoxCollider = gameObject.GetComponent<BoxCollider>();
        ai.CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        ai.CharacterController = gameObject.GetComponent<CharacterController>();
        
        return ai;
    }

    public void Initialize(AIState startState)
    {
        CurrentState = startState;
        //pauseTimer = 1f;
        //currentTime = 0f;
        Pattern.Clear();
        cur = 0;

        CurrentState.Enter();
    }

    public void ChangeState(AIState newState)
    {
        CurrentState.Exit();
        if (CurrentState != newState)
        {
            CurrentState = newState;
            CurrentState.Enter();
        }
        else
        {
            //Debug.Log("같은 State를 호출함.");
        }
    }

    public void SetTarget(GameObject obj)
    {
        Target = obj;
    }

    public void NextPattern()
    {
        cur++;
        if (cur >= Pattern.Count)
        {
            cur = 0;
        }
        //cur++;
        //Debug.Log("cur = " + cur.ToString() + " / patton = " + pattern.Count.ToString());
        ChangeState(Pattern[cur]);

    }

    public void AddPatern(AIState state)
    {
        Pattern.Add(state);
    }
}
