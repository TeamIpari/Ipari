using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;

public abstract class AIAttackState : AIState
{
    public AIAttackState(AIStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {

    }

    protected virtual void ChangeState()
    {
        if (children.Count > 0)
            stateMachine.ChangeState(children[current]);
        else if (parent != null)
            stateMachine.ChangeState(parent);
        else if (stateMachine.Pattern.Count > 0)
            stateMachine.NextPattern();
        else
            Debug.Log("연결된 State가 없음.");
    }

    //protected Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    //{
    //    // define the distance x and y first;
    //    Vector3 distance = target - origin;
    //    Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
    //    distanceXZ.y = 0f; // y는 0으로 설정.

    //    // Create a float the represent our distance
    //    float Sy = distance.y;      // 세로 높이의 거리를 지정.
    //    float Sxz = distanceXZ.magnitude;

    //    // 속도 추가
    //    float Vxz = Sxz / time;
    //    float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

    //    // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
    //    Vector3 result = distanceXZ.normalized;
    //    result *= Vxz;
    //    result.y = Vy;
    //    return result;
    //}
}
