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
            Debug.Log("����� State�� ����.");
    }

    //protected Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    //{
    //    // define the distance x and y first;
    //    Vector3 distance = target - origin;
    //    Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
    //    distanceXZ.y = 0f; // y�� 0���� ����.

    //    // Create a float the represent our distance
    //    float Sy = distance.y;      // ���� ������ �Ÿ��� ����.
    //    float Sxz = distanceXZ.magnitude;

    //    // �ӵ� �߰�
    //    float Vxz = Sxz / time;
    //    float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

    //    // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
    //    Vector3 result = distanceXZ.normalized;
    //    result *= Vxz;
    //    result.y = Vy;
    //    return result;
    //}
}
