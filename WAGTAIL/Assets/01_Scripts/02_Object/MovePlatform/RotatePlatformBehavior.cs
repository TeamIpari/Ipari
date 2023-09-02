using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************
 * �÷��̾ �ش� ����� ���, ������ ȸ���ϴ� �������� ���ǵ� ������Ʈ.
 ****/
[AddComponentMenu("Platform/RotatePlatformBehavior")]
public sealed class RotatePlatformBehavior : PlatformBehaviorBase
{
    //=======================================
    //////           Property            ////
    //=======================================
    public float RotateSpeedRatio { 
        get { return _RotateSpeedRatio; } 
        set 
        { 
            _RotateSpeedRatio = value; 
            _updateQuat     = Quaternion.AngleAxis((20f * RotateSpeedRatio), Vector3.up);
        }
    }
    [SerializeField] public bool     RotateAtObjectOnPlatform = true;
    [SerializeField] private float   _RotateSpeedRatio        = .1f;

    private Quaternion _updateQuat    = Quaternion.identity;
    private Vector3    _startCenter   = Vector3.zero;
    private Vector3 _test;
    private float      _centerDistance = 0f;
    private Transform  _platformTr;


    //=======================================
    //////     Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _updateQuat  = Quaternion.AngleAxis((20f * RotateSpeedRatio), Vector3.up);
        _startCenter = affectedPlatform.Collider.bounds.center;
        _centerDistance = (_startCenter - affectedPlatform.transform.position).magnitude;
        _platformTr = affectedPlatform.transform;
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        if ( (RotateAtObjectOnPlatform && affectedPlatform.ObjectOnPlatform) 
             || RotateAtObjectOnPlatform==false)
        {
            affectedPlatform.StartQuat *= _updateQuat;

            /*****************************************************
             *  �߽����� �������� �߽ɿ� ���ٸ� �߽������� �̵���Ų��...
             * **/
            Vector3 dir = _platformTr.forward * _centerDistance;
            Vector3 pos = (_startCenter - dir);
            pos.y = _platformTr.position.y;
            _platformTr.position = pos;

        }
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        //��꿡 �ʿ��� ��ҵ��� ��� ���Ѵ�.
        Vector3 platformCenter = affectedPlatform.Collider.bounds.center;
        Vector3 platformCenter2 = new Vector3(platformCenter.x, standingPoint.y, platformCenter.z);
        Vector3 distance = (standingPoint - platformCenter2);
        float radius = distance.magnitude;
        float radian = Mathf.Atan2(distance.z, distance.x) - (Mathf.Deg2Rad *  20f * RotateSpeedRatio);
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        //���� �� �÷����� ��� �ִ� ����� �÷����� ���� ȸ����Ų��.
        Vector3 rotVector = new Vector3(cos, 0f, sin) * radius;
        standingTarget.transform.position = _test = (platformCenter2 + rotVector);
    }

}
