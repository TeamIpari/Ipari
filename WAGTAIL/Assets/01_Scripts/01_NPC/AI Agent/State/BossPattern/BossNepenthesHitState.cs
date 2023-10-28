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
        animLeftVine = leftVine.GetComponent<Animator>();
        animRightVine = rightVine.GetComponent<Animator>();
    }


    public override void Enter()
    {
        #region Omit
        base.Enter();
        // �´� �������� ����
        AISM.Animator.SetTrigger("isHit");
        if (animLeftVine != null) animLeftVine.SetTrigger("isHit");
        if (animRightVine != null) animRightVine.SetTrigger("isHit");
        if (AISM.character.IsHit)
            AISM.character.HP -= 10;
        if (AISM.character.HP <= nextPhaseHp)
        {
            // ���� ����
            SetPhaseHp();
            SetNextPhase();
        }
        AISM.character.IsHit = false;
        curTimer = 0;
        #endregion
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
