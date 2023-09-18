using UnityEngine;

public class AIBoundaryState : AIState
{
    /// <summary>
    /// �����
    /// �ܰ� ���� ���� Player�� Ž���� �� ��� �������� ���߰� �÷��̾ �ֽ��ϸ� ��踦 ��.
    /// ���� ������ Player�� ���� ��� RunState�� ����.
    /// Player�� �ܰ� ������ ���� ��� parent State�� �ҷ���
    /// Player�� ���� ������ �� ��� child State�� �ҷ���.
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
        // ����� �¼�
        //Debug.Log(!stateMachine.Agent.isPathStale);
        //Debug.Log("��� ��...");

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
            // Player�� ������ ���.
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
            Debug.Log("parent�� �������� ����.");
        }
    }
}
