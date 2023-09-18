using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using IPariUtility;
using UnityEditor.Rendering;

public class NepenthesIdleState : AIIdleState
{
    private bool isSearch ;
    private float rotateAngle;
    private float SearchAngle;

    private float dot;
    private float angle;

    private Vector3 targetPos;
    private Vector3 dir;
    private Vector3 lookDir;
    private Vector3 startEulerAngle;
    private LayerMask targetMask;
    private LayerMask passMask;

    private List<Collider> hitedTargetContainer = new List<Collider>();
    public NepenthesIdleState(AIStateMachine stateMachine, float rotAngle, float searchAngle, LayerMask targetMask, LayerMask obstacleMask) : base(stateMachine)
    {
        rotateAngle = rotAngle;
        this.SearchAngle = searchAngle;
        this.targetMask = targetMask;
        passMask = obstacleMask;
        startEulerAngle = stateMachine.Transform.eulerAngles;
    }

    public override void Enter()
    {
        base.Enter();
        TargetReset();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void OntriggerEnter(Collider other)
    {
        base.OntriggerEnter(other);
    }

    private Collider[] FindViewTarget()
    {
        hitedTargetContainer.Clear();

        Vector3 originPos = AISM.Transform.position;
        Collider[] hitedTargets = Physics.OverlapSphere(originPos, AISM.character.AttackRange, targetMask);

        foreach(var hitedTarget in hitedTargets)
        {
            targetPos = hitedTarget.transform.position; // Ž���� ������Ʈ(Target)�� ��ġ
            dir = (targetPos - originPos).normalized;   // Ž���� �� ������Ʈ�� ��� ���⿡ Ÿ���� �����ϴ���?
            lookDir = IpariUtility.AngleToDirY(startEulerAngle, rotateAngle);

            // float RotateAngle = vector3.Angle(lookDir, dir)
            // �Ʒ� �� ���� ���� �ڵ�� �����ϰ� �۵���. ���� ������ ����.
            dot = Vector3.Dot(lookDir, dir);
            angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle <= SearchAngle)
            {
                RaycastHit rayHitedTarget;
                bool bHit = Physics.Raycast(originPos, dir, out rayHitedTarget, AISM.character.AttackRange, passMask);
                if (bHit)
                {
#if UNITY_EDITOR
                    Debug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);
#else
#endif
                }
                else
                {
                    hitedTargetContainer.Add(hitedTarget);
                    AISM.Target = hitedTarget.gameObject;
                    isSearch = true;
#if UNITY_EDITOR
                    Debug.DrawLine(originPos, targetPos, Color.red);
#else
#endif
                }
            }
        }
        if (hitedTargetContainer.Count > 0)
        {
            return hitedTargetContainer.ToArray();
        }
        else
            return null;
    }

    private bool Search()
    {

        // �� ���� ���� �÷��̾ ������ �ٶ󺸰� ���� �ֱⰡ ������ Attack
        Collider[] cols = Physics.OverlapSphere(AISM.Transform.position, AISM.character.AttackRange);


        // Layer�� Player�� ĳ���͸� ��ġ 
        if(cols.Length  > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                float targetRadian = Vector3.Dot(AISM.Transform.forward, (cols[i].transform.position - AISM.Transform.position).normalized);

                if ( cols[i].CompareTag("Player"))
                {
                    AISM.Target = cols[i].gameObject;
                    isSearch = true;
                    return true;
                }
            }
        }
        return false;
    }
    private void LookatTarget()
    {
        Vector3 dir = AISM.character.RotatePoint.position - AISM.Target.transform.position;
        dir.y = AISM.character.RotatePoint.position.y;

        Quaternion quat = Quaternion.LookRotation(dir.normalized);

        Vector3 temp = quat.eulerAngles;
        Vector3 temp2 = AISM.character.RotatePoint.rotation.eulerAngles;

        AISM.character.RotatePoint.rotation = Quaternion.Euler(temp.x, temp.y - 180f, temp2.z);

        AISM.ChangeState(AISM.character.isAttack() ? AISM.character.AiAttack : AISM.CurrentState);

    }

    public override void Update()
    {
        base.Update();
        //if (angle > SearchAngle)
        if (isSearch)
        {
            dot = Vector3.Dot(lookDir, this.dir);
            angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            float distance = Vector3.Distance(AISM.Transform.position, AISM.Target.transform.position);
            if (distance > AISM.character.AttackRange
                || angle > SearchAngle)
            {
                TargetReset();
                return;
            }
            LookatTarget();
        }
        //else
        FindViewTarget();
    }

    private void TargetReset()
    {
        isSearch = false;
        AISM.Target = null;
        //angle = 0;
    }
}
