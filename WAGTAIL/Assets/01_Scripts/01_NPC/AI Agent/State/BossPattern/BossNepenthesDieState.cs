using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesDieState : AIDieState
{

    //==========================================
    /////        properties Methods         ////
    //==========================================
    private GameObject leftVine;
    private GameObject rightVine;
    private Animator animLeftVine;
    private Animator animRightVine;
    private float brokenTime;
    private bool oneChance;
    //==========================================
    /////           magic Methods           ////
    //==========================================
    public BossNepenthesDieState(AIStateMachine stateMachine, GameObject LeftVine, GameObject RightVine) : base(stateMachine)
    {

        this.leftVine = LeftVine;
        this.rightVine = RightVine;
        animLeftVine = LeftVine.GetComponent<Animator>();
        animRightVine = rightVine.GetComponent<Animator>();

    }

    public override void Enter()
    {
        base.Enter();
        AISM.Animator.SetTrigger("isDeath");
        animLeftVine.SetTrigger("isDeath");
        animRightVine.SetTrigger("isDeath");
        curTimer = 0;
        brokenTime = 1.5f;
        oneChance = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        base.OntriggerEnter(other);
    }

    public override void Update()
    {
        base.Update();
        curTimer += Time.deltaTime;
        if(curTimer > brokenTime && !oneChance)
        {
            BossRoomFieldManager.Instance.EnableBrokenPlatformComponent();
            AISM.character.GetComponent<Enemy>().GoNextChapter();
            oneChance = true;
        }
    }
}
