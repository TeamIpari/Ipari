using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************
 * 플레이어가 해당 밟았을 경우, 발판이 회전하는 움직임이 정의된 컴포넌트.
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
             *  중심축의 오프셋이 중심에 없다면 중심점으로 이동시킨다...
             * **/
            Vector3 dir = _platformTr.forward * _centerDistance;
            Vector3 pos = (_startCenter - dir);
            pos.y = _platformTr.position.y;
            _platformTr.position = pos;

        }
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        //계산에 필요한 요소들을 모두 구한다.
        Vector3 platformCenter = affectedPlatform.Collider.bounds.center;
        Vector3 platformCenter2 = new Vector3(platformCenter.x, standingPoint.y, platformCenter.z);
        Vector3 distance = (standingPoint - platformCenter2);
        float radius = distance.magnitude;
        float radian = Mathf.Atan2(distance.z, distance.x) - (Mathf.Deg2Rad *  20f * RotateSpeedRatio);
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        //현재 이 플랫폼을 밟고 있는 대상을 플랫폼과 같이 회전시킨다.
        Vector3 rotVector = new Vector3(cos, 0f, sin) * radius;
        standingTarget.transform.position = _test = (platformCenter2 + rotVector);
    }

}
