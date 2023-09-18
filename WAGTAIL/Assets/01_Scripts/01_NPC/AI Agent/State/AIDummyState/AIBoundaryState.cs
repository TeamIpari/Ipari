using UnityEngine;

public class AIBoundaryState : AIState
{
    /// <summary>
    /// 경계모드
    /// 외각 범위 내에 Player가 탐색이 될 경우 움직임을 멈추고 플레이어를 주시하며 경계를 함.
    /// 내각 범위로 Player가 들어올 경우 RunState로 변함.
    /// Player가 외각 범위를 나갈 경우 parent State를 불러옴
    /// Player가 내각 범위로 들어갈 경우 child State를 불러옴.
    /// </summary>

    private float changeTime;
    private float currentTime;

    private float comeDistance;
    private float runDistance;

    public AIBoundaryState(AIStateMachine stateMachine, float comeDistance, float runDistance) : base(stateMachine)
    {
        this.AISM = stateMachine;
        this.comeDistance = comeDistance;
        this.runDistance = runDistance;
    }

    public override void SetChildren(AIState _state)
    {
        base.SetChildren(_state);
        SetParent(this, _state);
    }

    public override void Enter()
    {
        //stateMachine.NPCBase.stateName = "Boundary";

        changeTime = Random.Range(2, 5);
        currentTime = 0;
    }

    public override void Exit()
    {
        //stateMachine.NPCBase.stateName = "None";

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        Search();
        Boundary();
        // 경계의 태세
        //Debug.Log(!stateMachine.Agent.isPathStale);
        //Debug.Log("경계 중...");

    }

    private void Boundary()
    {
        //if (stateMachine.Target != null && !stateMachine.Agent.isOnOffMeshLink)
        //{
        //    stateMachine.Transform.LookAt(stateMachine.Target.transform);
        //}
        if(AISM.Target)
        {
            //stateMachine.Transform.LookAt(stateMachine.Target.transform);
        }


    }

    private void Search()
    {
        float distance = Mathf.Abs(
            Vector3.Distance(AISM.Transform.position,
            AISM.Target.transform.position));
        if (distance > comeDistance)
        {
            // Player가 범위를 벗어남.
            AISM.SetTarget(null);
            Parent.Current--;
            //stateMachine.pause = true;
            AISM.ChangeState(Parent);
        }
        else if(distance < runDistance)
        {
            AISM.ChangeState(Children[Current]);
        }
    }

    private void Change()
    {
        currentTime += Time.deltaTime;
        try
        {
            if (currentTime > changeTime)
            {
                if (Children.Count > 0)
                {
                    AISM.ChangeState(Children[Current]);
                }
                else
                    AISM.ChangeState(Parent);
            }
        }
        catch
        {
            Debug.Log("parent가 존재하지 않음.");
        }
    }
}
