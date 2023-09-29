using UnityEngine;

public class AILineMove : AIState
{
    // 일정 방향으로 직선으로 움직이는 기능을 가지고 있음. 
    // 다수의 포인트를 가지고 있으며 랜덤으로 포인트를 지정 후
    // 목표 지점을 향해 움직이는 기능을 가짐.

    private Transform[] movingPoint;
    private float currentTimer = 0;
    private float changeTimer = 5;
    // Start Point는 무조건 0
    private int cur = 0;
    public AILineMove(AIStateMachine stateMachine, Transform[] points) : base(stateMachine)
    {
        this.AISM = stateMachine;
        movingPoint = points;
        if(movingPoint.Length < 1)
        {
            Debug.Log("이동 할 지점이 없음.");
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
                Debug.Log("연결되어있지 않음.");

            //currentTimer = 0;
        }
    }

    // 목표 지점으로 이동하는 스크립트.
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
