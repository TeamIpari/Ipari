using UnityEngine;

public class AILineMove : AIState
{
    // ���� �������� �������� �����̴� ����� ������ ����. 
    // �ټ��� ����Ʈ�� ������ ������ �������� ����Ʈ�� ���� ��
    // ��ǥ ������ ���� �����̴� ����� ����.

    private Transform[] movingPoint;
    private float currentTimer = 0;
    private float changeTimer = 5;
    // Start Point�� ������ 0
    private int cur = 0;
    public AILineMove(AIStateMachine stateMachine, Transform[] points) : base(stateMachine)
    {
        this.AISM = stateMachine;
        movingPoint = points;
        if(movingPoint.Length < 1)
        {
            Debug.Log("�̵� �� ������ ����.");
        }
        //_stateMachine
    }

    public override void Enter()
    {

        Debug.Log("Start Ai Line Move State");
        int num;
        do
        { 
            num = Random.Range(0, movingPoint.Length);
        }
        while (num == cur);
        cur = num;
    }

    public override void Exit()
    {
        Debug.Log("End Ai Line Move State"); 
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        if (Move())
        {
            if (Children.Count > 0)
                AISM.ChangeState(Children[Current]);
            else if (Parent != null)
                AISM.ChangeState(Parent);
            else if (AISM.Pattern.Count > 0)
                AISM.NextPattern();
            else
                Debug.Log("����Ǿ����� ����.");

            //currentTimer = 0;
        }
    }

    // ��ǥ �������� �̵��ϴ� ��ũ��Ʈ.
    private bool Move()
    {
        if (Vector3.Distance(AISM.Transform.position, movingPoint[cur].position) <= 0.1)
        {
            AISM.Transform.position = movingPoint[cur].position;
            return true;
        }
        AISM.Transform.position
            = Vector3.MoveTowards(AISM.Transform.position, movingPoint[cur].position, 5f * Time.deltaTime);
        return false;
        //Debug.Log(movingPoint[cur].name);
        //Debug.Log(movingPoint[cur].position);
    }
}
