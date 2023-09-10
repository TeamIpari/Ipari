using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using static FModEventPlayBehavior;
using UnityEditor;
#endif

/****************************************************************
 * �÷��̾ �ش� ����� ���, ������ ȸ���ϴ� �������� ���ǵ� ������Ʈ.
 ****/
[AddComponentMenu("Platform/RotatePlatformBehavior")]
[RequireComponent(typeof(Collider))]
public sealed class RotatePlatformBehavior : PlatformBehaviorBase
{
    public enum RotatePlatformState
    {
        Stop,
        Rotation,
        Pause
    }

    //=======================================
    //////           Property            ////
    //=======================================
    public float PauseInterval
    {
        get { return _PauseInterval; }
        set
        {
            _PauseInterval = value;
            if (_PauseInterval < 0) _PauseInterval = 0;
            _pauseIntervalTimeDiv = (1f / _PauseInterval);
        }
    }
    public float PauseDuration
    {
        get { return _PauseDuration; }
        set
        {
            _PauseDuration = value;
            if (_PauseDuration < 0) _PauseDuration = 0;
            _PauseTimeDiv = (1f / _PauseDuration);
        }
    }
    public RotatePlatformState State
    {
        get { return _state; }
    }

    [Header("Default settings")] 
    [Space(5f)]
    [SerializeField] public bool    ApplyRotateAtObjectEnter  = false;
    [SerializeField] public bool    ApplyStandingObjectRotate = false;
    [SerializeField] public Vector3 RotateCenterOffset        = Vector3.zero;
    [SerializeField] public float   RotateAngleUntilStop      = 90f;

    [Space(15f)]

    [Header("Pause duration settings")]
    [Space(5f)]
    [SerializeField] private float _PauseInterval;
    [SerializeField] public float  _PauseDuration;



    //=======================================
    //////         Fields               /////
    ///======================================
    private float   _currTime = 0f;
    private float   _pauseIntervalTimeDiv = 0f;
    private float   _PauseTimeDiv = 0f;

    private RotatePlatformState _state           = RotatePlatformState.Stop;
    private float               _centerDistance  = 0f;
    private float               _lastRotateAngle = 0f;
    private Transform           _platformTr;
    private Quaternion          _lastUpdateQuat = Quaternion.identity;
#if UNITY_EDITOR
    private Collider            _editorCollider;
#endif



    //=======================================
    //////     Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        Vector3 startCenter   = (affectedPlatform.Collider.bounds.center + RotateCenterOffset);
        _centerDistance       = ( startCenter - affectedPlatform.transform.position).magnitude;
        _platformTr           = affectedPlatform.transform;
        _pauseIntervalTimeDiv = ( 1f / PauseInterval );
        _PauseTimeDiv         = ( 1f / PauseDuration );

        /**�⺻ ȸ�� ����...*/
        if (ApplyRotateAtObjectEnter==false){

            _state = RotatePlatformState.Rotation;
            _currTime = 0f;
        }
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        /**ȸ�� ����...*/
        if (ApplyRotateAtObjectEnter){

            _state = RotatePlatformState.Rotation;
            _currTime = 0f;
        }
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody)
    {
        /**�÷��̾ ����� ��쿡�� ����� ���, ȸ�� ��..*/
        if (ApplyRotateAtObjectEnter){

            _state = RotatePlatformState.Stop;
            _currTime = 0f;
        }
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        #region Omit
        if (_state==RotatePlatformState.Stop) return;

        float deltaTime = Time.fixedDeltaTime;
        _currTime += deltaTime;

        /*********************************
         *  ȸ�� ���� ���...
         ***/
        if ( RotateAngleUntilStop!=0f && _state == RotatePlatformState.Rotation)
        {
            /**ȸ������ ȸ���ҷ��� �µ��� ����.*/
            bool isComplete = (_currTime >= PauseInterval);
            if (isComplete){

                deltaTime += ( PauseInterval - _currTime);
            }

            /**�̹� ������Ʈ�� ȸ������ ���Ѵ�.*/
            float deltaAngleRatio = ( deltaTime * _pauseIntervalTimeDiv );
            _lastRotateAngle      = ( RotateAngleUntilStop * deltaAngleRatio );

            /**������ �� ȸ������ŭ ȸ���ϵ��� �����Ѵ�...*/
            _lastUpdateQuat = Quaternion.AngleAxis( _lastRotateAngle, Vector3.up );
            affectedPlatform.UpdateQuat *= _lastUpdateQuat;
            

            /*****************************************************
             *   ������ ȸ�� �߽����� ȸ���ϱ� ���� ��ġ �������� ���ϰ�,
             *   ������ �� �̵�����ŭ �̵��ϵ��� �����Ѵ�...
             * ***/
            Vector3 center = (affectedPlatform.Collider.bounds.center + RotateCenterOffset);
            Vector3 dir    = (center - _platformTr.position);
            dir.y = 0f;

            float distance       = dir.magnitude;
            float offsetDistance = RotateCenterOffset.magnitude;

            float updateRadian = (Mathf.Deg2Rad * _lastRotateAngle);

            /**ȸ�� ���� ȸ�� �߽��� ��ġ�� ���Ѵ�..*/
            float radian = Mathf.Atan2(dir.z, dir.x) - updateRadian;
            float cos    = Mathf.Cos(radian);
            float sin    = Mathf.Sin(radian);

            /**ȸ�� �߽� �������� ��ġ�� ȸ���� �°� �����Ѵ�...*/
            if(offsetDistance>0f)
            {
                float offsetRadian = Mathf.Atan2(RotateCenterOffset.z, RotateCenterOffset.x) - updateRadian;
                float offsetCos = Mathf.Cos(offsetRadian);
                float offsetSin = Mathf.Sin(offsetRadian);
                RotateCenterOffset = new Vector3(offsetCos, 0f, offsetSin) * offsetDistance;
            }

            dir.Normalize();

            Vector3 rotDst      = new Vector3(cos, 0f, sin) * distance;
            Vector3 afterCenter = ( _platformTr.position + rotDst );
            Vector3 correctVec  = ( center - afterCenter );
            correctVec.y = 0f;

            affectedPlatform.UpdatePosition += correctVec;

            /**ȸ�� ������..*/
            if (isComplete){

                _currTime = 0f;
                _state = RotatePlatformState.Pause;
                return;
            }
        }

        /*********************************
         *  �Ͻ����� ���� ���...
         ***/
        else if (_state==RotatePlatformState.Pause)
        {
            /**�Ͻ����� ������.*/
            if (_currTime >= _PauseDuration){

                _currTime = 0f;
                _state = RotatePlatformState.Rotation;
            }
        }

        #endregion
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit
        if (_state != RotatePlatformState.Rotation) return;

        //��꿡 �ʿ��� ��ҵ��� ��� ���Ѵ�.
        Vector3 platformCenter  = ( affectedPlatform.Collider.bounds.center + RotateCenterOffset );
        Vector3 platformCenter2 = new Vector3(platformCenter.x, standingPoint.y, platformCenter.z);
        Vector3 distance        = (standingPoint - platformCenter2);
        float radius = distance.magnitude;
        float radian = Mathf.Atan2(distance.z, distance.x) - (Mathf.Deg2Rad *  _lastRotateAngle);
        float cos    = Mathf.Cos(radian);
        float sin    = Mathf.Sin(radian);

        //���� �� �÷����� ��� �ִ� ����� �÷����� ���� ȸ����Ų��.
        Vector3 rotVector = new Vector3(cos, 0f, sin) * radius;
        standingTarget.transform.position = (platformCenter2 + rotVector);

        if (ApplyStandingObjectRotate){

            standingTarget.transform.rotation *= _lastUpdateQuat;
        }
        #endregion
    }


    //===================================
    ///         Magic methods       ////  
    //===================================
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_editorCollider == null) _editorCollider = GetComponent<Collider>();

        if(_editorCollider != null){

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_editorCollider.bounds.center+RotateCenterOffset, .2f);
        }
    }
#endif

}
