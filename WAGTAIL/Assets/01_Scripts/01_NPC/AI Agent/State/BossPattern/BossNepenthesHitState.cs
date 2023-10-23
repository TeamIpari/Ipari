using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNepenthesHitState : AIHitState
{
    //=========================================
    /////       Property And Fields         /////
    //=========================================
    private GameObject leftVine;
    private GameObject rightVine;
    private Animator animLeftVine;
    private Animator animRightVine;
    private float delayTimer = 5.0f;
    private int nextPhaseHp = 0;
  
    //=========================================
    /////       Magic Mathods               /////
    //=========================================
    public BossNepenthesHitState(AIStateMachine stateMachine, GameObject LeftVine = null, GameObject RightVine = null) : base(stateMachine)
    {
        nextPhaseHp = stateMachine.GetNextPhaseTargetHp();
        this.leftVine = LeftVine;
        this.rightVine = RightVine;
        animLeftVine = LeftVine.GetComponent<Animator>();
        animRightVine = rightVine.GetComponent<Animator>();
    }


    public override void Enter()
    {
        base.Enter();
        // �´� �������� ����
        AISM.Animator.SetTrigger("IsHit");
        if(animLeftVine != null) animLeftVine.SetTrigger("IsHit");
        if(animRightVine != null) animRightVine.SetTrigger("IsHit");
        if (AISM.character.IsHit)
            AISM.character.HP -= 10;
        if(AISM.character.HP <= nextPhaseHp)
        {
            // ���� ����
            SetPhaseHp();
            SetNextPhase();
        }
        AISM.character.IsHit = false;
        curTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        curTimer += Time.deltaTime;
        if(AISM.character.HP <= 0)
        {
            // ü���� 0 ������ ��� Death�� �ٷ� ���.
            AISM.ChangeState(AISM.character.AiDie);
        }
        if(curTimer > delayTimer)
        {
            AISM.NextPattern();
        }
    }
    //===========================================
    /////           Core Methods            /////
    //===========================================
    public void SetPhaseHp()
    {
        Debug.Log($"���� ������ ����.");
        if (AISM.IsNextTargetPhaseHp())
        {
            nextPhaseHp = AISM.GetNextPhaseTargetHp();
            AISM.character.GetCurPhaseHpArray++;
        }
        else
            // ���� ����� ����.
            nextPhaseHp = 0;
    }
    public void SetNextPhase()
    {
        AISM.character.SettingPattern(AISM.EPattern);
    }
}
