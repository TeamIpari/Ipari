using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
//using UnityEditor.Rendering.LookDev;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class AIStateMachine
{
    public GameObject gameObject;
    public Transform transform;
    public Animator animator;
    public Rigidbody physics;
    public NavMeshAgent agent;
    public SphereCollider sphereCollider;
    public BoxCollider boxCollider;
    public CapsuleCollider capsuleCollider;
    public CharacterController characterController;
    public GameObject target;

    public List<AIState> pattern = new List<AIState>();


    public AIState currentState;

    private int cur = 0;

    public static AIStateMachine CreateFormGameObject(GameObject gameObject)
    {
        AIStateMachine ai = new AIStateMachine();
        ai.gameObject = gameObject;
        ai.transform = gameObject.transform;
        ai.animator = gameObject.GetComponent<Animator>();
        ai.physics = gameObject.GetComponent<Rigidbody>();
        ai.agent = gameObject.GetComponent<NavMeshAgent>();
        ai.sphereCollider = gameObject.GetComponent<SphereCollider>();
        ai.boxCollider = gameObject.GetComponent<BoxCollider>();
        ai.capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        ai.characterController = gameObject.GetComponent<CharacterController>();
        
        return ai;
    }

    public void Initialize(AIState startState)
    {
        currentState = startState;
        //pauseTimer = 1f;
        //currentTime = 0f;
        pattern.Clear();
        cur = 0;

        currentState.Enter();
    }

    public void ChangeState(AIState newState)
    {
        currentState.Exit();
        if (currentState != newState)
        {
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            //Debug.Log("같은 State를 호출함.");
        }
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
    }

    public void NextPattern()
    {
        cur++;
        if (cur >= pattern.Count)
        {
            cur = 0;
        }
        //cur++;
        //Debug.Log("cur = " + cur.ToString() + " / patton = " + pattern.Count.ToString());
        ChangeState(pattern[cur]);

    }

    public void AddPatern(AIState _state)
    {
        pattern.Add(_state);
    }
}
