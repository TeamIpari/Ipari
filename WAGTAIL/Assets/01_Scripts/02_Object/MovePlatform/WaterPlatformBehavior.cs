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
    [HideInInspector] public float Yspeed        = 0f;
    [HideInInspector] public float Rotspeed      = 0f;
    [SerializeField]  public float sinkDepth     = .1f;
    [SerializeField]  public float SpinPow       = 80f;



    //=======================================
    //////      Private Fields          /////
    //=======================================
    private const float     _WaterValue = .025f;
    private const float     _Buoyancy   = .008f;

    private Vector3         _SpinRotDir      = Vector3.zero;
    private Vector3         _defaultPos      = Vector3.zero;
    private float           _lastYEuler      = 0f;
    private float           _landedRadian    = 0f;
    private LandedType      _landedType      = LandedType.None;
    private Quaternion      _defaultQuat     = Quaternion.identity;
    private Transform       _platformTr;

    private Vector3 _playerLastEuler = Vector3.zero;

    float _currY   = 0f;
    float _currRot = 0f;



    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _platformTr = affectedPlatform.transform;
        _defaultPos = affectedPlatform.transform.position;
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

        /**Y�� �ⷷ�Ÿ�...*/
        float y       = _currY - ( -Yspeed );
        float yAccel  = ( -_WaterValue * y );
        Yspeed += yAccel;
        _currY += Yspeed;

        /**ȸ�� �ⷷ�Ÿ�...*/
        float rot       = _currRot - ( -Rotspeed );
        float rotAccel  = ( -_WaterValue * rot );
        Rotspeed += rotAccel;
        _currRot += Rotspeed;


        /*************************************
         *  ���� �̵� �� ȸ���� ���� ���� ����...
         ***/
        Vector3 offset = (Vector3.up * Yspeed);
        affectedPlatform.UpdatePosition += offset;

        Quaternion spinRot = Quaternion.AngleAxis( (Rotspeed * SpinPow), _SpinRotDir);
        affectedPlatform.OffsetQuat *= spinRot;

        /**�÷��̾��� ȸ�� ����...*/
        if(affectedPlatform.PlayerOnPlatform){

            Vector3 euler = _playerLastEuler;
            euler.y = Player.Instance.transform.eulerAngles.y;
            Player.Instance.transform.rotation = Quaternion.Euler( euler );
        }
        #endregion
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit
        /***********************************************
         *  ������ ���, ���� ���� �� ���� ���·� ��ȯ�Ѵ�...
         * **/
        _landedType = LandedType.Enter;
        Yspeed      = -sinkDepth;
        Rotspeed    = -.1f;

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

        /************************************
         *   �÷��̾��� ȸ�������� ���� ó��...
         * ***/
        if(standingTarget.gameObject==Player.Instance.gameObject) {

            _playerLastEuler = Player.Instance.transform.eulerAngles;
        }
        #endregion
    }
}
