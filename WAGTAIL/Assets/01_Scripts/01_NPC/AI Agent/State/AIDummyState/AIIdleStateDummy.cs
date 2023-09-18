using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class AIIdleStateDummy : AIState
{
    /// <summary>
    /// 가장 기본이 되는 어찌보면 RootNode
    /// IdleState이며 가만히 있는 상태를 가지고 있음.
    /// Search기능을 가지고 있으며 현재 어떤 상태냐에 따라 child State를 선택적으로 가져옴.
    /// </summary>

    // 랜덤 대기 시간
    private float movingTime = 0;
    private float currentTime = 0;

    private float searchDistance = 0;
    


    public AIIdleStateDummy (AIStateMachine stateMachine, float moveTime,float SearchDistance) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.movingTime = moveTime;
        this.searchDistance = SearchDistance;
    }

    public override void Enter()
    {
        //stateMachine.NPCBase.stateName = "Idle";
        currentTime = 0;
    }

    public override void Exit()
    {
        //stateMachine.NPCBase.stateName = "None";
        //Debug.Log("End AI Idle State");
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);

    }

    public override void Update()
    {
        if(Current == 0)
        {
            // 이동 시간 결정
            CheckMoveTime();
            // 0이 아닐 경우 실시간 서치 0이면 서치 기능이 없는거.
            if (searchDistance > 0)
                SearchTarget();
        }
        else
        {
            ChangeState();
        }
    }

    private void ChangeState()
    {
        if (Children.Count > 0)
        {
            AISM.ChangeState(Children[Current]);
        }
        else
            AISM.ChangeState(Parent);
    }

    private void CheckMoveTime()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > movingTime)
            {
                if (Children.Count > 0)
                {
                    AISM.ChangeState(Children[Current]);
                }
                else if (Parent != null)
                    AISM.ChangeState(Parent);
                else if (AISM.Pattern.Count > 0)
                    AISM.NextPattern();
                else
                     ;
                currentTime = 0;
            }
        }
        catch
        {
            Debug.Log("State가 존재하지 않음.");
        }
    }

    private void SearchTarget()
    {
        Collider[] cols =
            Physics.OverlapSphere(AISM.Transform.position, searchDistance, LayerMask.GetMask("Player"));
        // Player가 체크 되었는가?
        foreach (var c in cols)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                Current++;   // 다음 State 진행
                AISM.SetTarget(c.gameObject);
                AISM.ChangeState(Children[Current]);
            }
        }
    }


}
