using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;


/********************************************************
 * �÷��̾ �ش� ����� ���, ������ �ⷷ�̴� ȿ���� �����մϴ�.
 ****/
[AddComponentMenu("Platform/WaterPlatformBehavior")]
public sealed class WaterPlatformBehavior : PlatformBehaviorBase
{
    private enum LandedType
    {
        None,
        Enter,
        Shake
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

    private Vector3         _startPosition = Vector3.zero;
    private Vector3         _startRotation = Vector3.zero;
    private Quaternion      _defaultRotation = Quaternion.identity;
    private LandedType      _landedType    = LandedType.None;



    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _startPosition = transform.position;
        _defaultRotation= transform.rotation;
        affectedPlatform.CheckGroundOffset = 2f;
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        #region Ommission
        if (_landedType == LandedType.None) return;

        Transform platformTr = affectedPlatform.transform;

        /***********************************
         * ���� ����� ���� �ⷷ�Ÿ� ȿ��
         ***/
        if (_landedType == LandedType.Enter)
        {
            Yspeed += _Buoyancy;

            //�ⷷ�Ÿ� ������.
            if ((platformTr.position.y + Yspeed) > _startPosition.y) {

                _landedType = LandedType.Shake;
                Yspeed *= .6f;
                return;
            }
        }

        /***********************************
         * ���� �� �ܿ� �ⷷ�� ȿ��
         ***/
        else
        {
            float y = platformTr.position.y - (_startPosition.y - Yspeed);
            float accel = -_WaterValue * y;

            Yspeed += accel;
        }

        //���� ����f
        Vector3 offset = (Vector3.up * Yspeed);
        platformTr.position += offset;

        Vector3 currEuler = platformTr.transform.eulerAngles;
        float radian  = Mathf.Atan2( _startRotation.z, _startRotation.x ) + Mathf.Deg2Rad* currEuler.y;
        float cos     = Mathf.Cos(radian);
        float sin     = Mathf.Sin(radian);

        Vector3 startRot = new Vector3(cos, 0f, sin) * _startRotation.magnitude;
        Vector3 rotEuler  = new Vector3(
            SpinPow * Yspeed * (-startRot.z),
            currEuler.y,
            SpinPow * Yspeed * (startRot.x)
        );

        platformTr.rotation = Quaternion.Euler( rotEuler );
        #endregion
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        _landedType = LandedType.Enter;
        Yspeed = -.1f;
        _startRotation = (standingTarget.transform.position - transform.position).normalized;
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        Vector3 pos = standingTarget.transform.position;
        pos.y = standingPoint.y;

        standingTarget.transform.position = pos;
    }
}
