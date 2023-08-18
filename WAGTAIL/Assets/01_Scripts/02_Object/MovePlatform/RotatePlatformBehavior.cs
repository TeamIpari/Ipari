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
            _updateRadian   = (Mathf.Deg2Rad * 20f * RotateSpeedRatio);
            _updateVector.y = 20f * RotateSpeedRatio;
        }
    }
    [SerializeField] public bool     RotateAtObjectOnPlatform = true;
    [SerializeField] private float   _RotateSpeedRatio        = .1f;

    private Vector3 _updateVector     = new Vector3();
    private float   _updateRadian     = 0f;


    //=======================================
    //////     Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _updateRadian = (Mathf.Deg2Rad * -20f * RotateSpeedRatio);
        _updateVector.y = 20f * RotateSpeedRatio;
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        if ( (RotateAtObjectOnPlatform && affectedPlatform.ObjectOnPlatform) 
             || RotateAtObjectOnPlatform==false)
        {
            Vector3 currRot = affectedPlatform.transform.rotation.eulerAngles;
            Quaternion newRot = Quaternion.Euler(_updateVector + currRot);
            affectedPlatform.transform.rotation = newRot;
        }
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        //계산에 필요한 요소들을 모두 구한다.
        Vector3 platformCenter  = affectedPlatform.Collider.bounds.center;
        Vector3 platformCenter2 = new Vector3(platformCenter.x, standingPoint.y, platformCenter.z);
        Vector3 distance        = ( standingPoint - platformCenter2 );
        float   radius          = distance.magnitude;
        float   radian          = Mathf.Atan2(distance.z, distance.x) + _updateRadian;
        float   cos             = Mathf.Cos(radian);
        float   sin             = Mathf.Sin(radian);

        //현재 이 플랫폼을 밟고 있는 대상을 플랫폼과 같이 회전시킨다.
        Vector3 rotVector = new Vector3(cos, 0f, sin) * (radius);
        standingTarget.transform.position = (platformCenter2 + rotVector);
    }

}
