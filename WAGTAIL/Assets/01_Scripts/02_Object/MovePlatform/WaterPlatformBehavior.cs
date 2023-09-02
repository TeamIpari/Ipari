using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/********************************************************
 * 플레이어가 해당 밟았을 경우, 발판이 출렁이는 효과를 적용합니다.
 ****/
[AddComponentMenu("Platform/WaterPlatformBehavior")]
public sealed class WaterPlatformBehavior : PlatformBehaviorBase
{
    private enum LandedType
    {
        None,
        Enter
    };

    //========================================
    //////           Property            /////
    //========================================
    [SerializeField] public float Yspeed        = 0f;
    [SerializeField] public float SpinPow       = 80f;



    //=======================================
    //////      Private Fields          /////
    //=======================================
    private const float     _WaterValue = .025f;
    private const float     _Buoyancy   = .008f;

    private Vector3         _SpinRotDir      = Vector3.zero;
    private float           _lastYEuler      = 0f;
    private float           _landedRadian    = 0f;
    private LandedType      _landedType      = LandedType.None;
    private Quaternion      _defaultQuat     = Quaternion.identity;
    private Transform       _platformTr;



    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _platformTr = affectedPlatform.transform;
        _defaultQuat= _platformTr.rotation;
        affectedPlatform.CheckGroundOffset = 2f;
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        #region Ommission
        if (_landedType == LandedType.None) return;

        /**********************************************
         *  회전방향벡터를 구한다...
         * ***/
        Vector3 currEuler = _platformTr.eulerAngles;

        /**Y축 회전이 변화가 없다면 회전방향벡터를 갱신할 필요가 없음..*/
        if( _lastYEuler!=currEuler.y ){

            _lastYEuler= currEuler.y;
            float radian = _landedRadian + (Mathf.Deg2Rad * _lastYEuler);
            float cos = Mathf.Cos(radian);
            float sin = Mathf.Sin(radian);

            _SpinRotDir = new Vector3(
                -sin,
                0f,
                cos
            );
        }

        /**************************************
         *   출렁거리는 연산을 적용한다...
         * **/
        float y     = _platformTr.position.y - (affectedPlatform.StartPosition.y - Yspeed);
        float accel = -_WaterValue * y;

        Yspeed += accel;


        /*************************************
         *  수직 이동 및 회전에 대한 최종 적용...
         ***/
        Vector3 offset = (Vector3.up * Yspeed);
        _platformTr.position += offset;

        Quaternion spinRot    = Quaternion.AngleAxis( (Yspeed * SpinPow), _SpinRotDir);
        affectedPlatform.UpdateQuat *= spinRot;
        #endregion
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit
        /***********************************************
         *  밟혔을 경우, 밟힌 지점 및 밟힌 상태로 전환한다...
         * **/
        _landedType = LandedType.Enter;
        Yspeed      = -.1f;

        Vector3 standingDir = (standingTarget.transform.position - transform.position).normalized;
        _landedRadian = Mathf.Atan2(standingDir.z, standingDir.x);

        /**회전방향벡터를 구한다...*/
        _lastYEuler = _platformTr.transform.eulerAngles.y;
        float radian = _landedRadian + (Mathf.Deg2Rad * _lastYEuler);
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        _SpinRotDir = new Vector3(
            -sin,
            0f,
            cos
        );

        #endregion
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        Vector3 pos = standingTarget.transform.position;
        pos.y = standingPoint.y;

        standingTarget.transform.position = pos;
    }
}
