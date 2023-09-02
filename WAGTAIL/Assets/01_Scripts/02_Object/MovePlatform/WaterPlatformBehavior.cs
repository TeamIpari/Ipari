using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/********************************************************
 * �÷��̾ �ش� ����� ���, ������ �ⷷ�̴� ȿ���� �����մϴ�.
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
         *  ȸ�����⺤�͸� ���Ѵ�...
         * ***/
        Vector3 currEuler = _platformTr.eulerAngles;

        /**Y�� ȸ���� ��ȭ�� ���ٸ� ȸ�����⺤�͸� ������ �ʿ䰡 ����..*/
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
         *   �ⷷ�Ÿ��� ������ �����Ѵ�...
         * **/
        float y     = _platformTr.position.y - (affectedPlatform.StartPosition.y - Yspeed);
        float accel = -_WaterValue * y;

        Yspeed += accel;


        /*************************************
         *  ���� �̵� �� ȸ���� ���� ���� ����...
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
         *  ������ ���, ���� ���� �� ���� ���·� ��ȯ�Ѵ�...
         * **/
        _landedType = LandedType.Enter;
        Yspeed      = -.1f;

        Vector3 standingDir = (standingTarget.transform.position - transform.position).normalized;
        _landedRadian = Mathf.Atan2(standingDir.z, standingDir.x);

        /**ȸ�����⺤�͸� ���Ѵ�...*/
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
