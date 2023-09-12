using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/**************************************************
 *  �̵��ϴ� �÷����� ���� �������� ���ǵ� ������Ʈ�Դϴ�.
 * ***/
[AddComponentMenu("Platform/MovingPlatformBehavior")]
public sealed class MovingPlatformBehavior : PlatformBehaviorBase
{
    // ���� ��ġ���� ������ �������� �̵��ϴ� ����� ������ ���Դϴ�. ��.
    // ���´� 4���� ��������(Enter), ������(Down), �ö��(Up), ���(None),

    private enum MovingType
    {
        None = 1,
        Enter = 2,
        Down = 3,
        Up = 4,
        End = 5,
    }

    //========================================
    //////           Property            /////
    //========================================
    [SerializeField] public float       Speed           = 0;
    [SerializeField] public float       ShakeDistance   = 0;
    [SerializeField] public float       shakeDelay      = 0;
    [SerializeField] public float       QuakeRate       = 0f;
    [SerializeField] public Vector3     TargetPosition  = Vector3.zero;


    //=======================================
    //////      Private Fields          /////
    //=======================================
    private float                       _curTime        = 0f;
    private float                       _curWaitTime    = 0f;
    private bool                        _isWait         = false;

    private Vector3                     _TargetPos      = Vector3.zero;
    private Vector3                     _defaultPos     = Vector3.zero;
    private MovingType                  _movingType     = MovingType.None;
    private Quaternion                  _defaultQuat    = Quaternion.identity;
    private Transform                   _platformTr;

    //=======================================
    /////       Core Method             /////
    //=======================================
    
    private void EarthQuake(PlatformObject affectedPlatform)
    {
        /************************************************
         * ���� ���� LandedType.Enter �� �� Tile�� �����ִ� ����� ��.
         * **/
        _curTime += Time.deltaTime;
        if (_curTime > shakeDelay)
        {
            Vector3 offset = _platformTr.position + (UnityEngine.Random.insideUnitSphere * ShakeDistance);
            _TargetPos = new Vector3(offset.x, _platformTr.position.y, offset.z);
            affectedPlatform.OffsetPosition +=  _TargetPos.normalized * ShakeDistance;
            _curTime = 0;
        }
    }
    private void MoveToWordsPlatform(PlatformObject affectedPlatform)
    {
        /************************************************
         * ���������� ������ ù ��ġ���� ���������� �̵��� ��ġ���� ������.
         * **/
        //UnityEngine.Debug.Log($"Move To Words");
        Vector3 direction = affectedPlatform.UpdatePosition - (_defaultPos + TargetPosition);       // ���� ���� ����
        float distance = Vector3.Distance(affectedPlatform.UpdatePosition, (_defaultPos + TargetPosition));
        if(distance > 0.1f)
        {
            affectedPlatform.UpdatePosition -= direction.normalized * (Time.deltaTime * Speed);
        }
        else
        {
            StartPlatformStateChange(1f);
        }
    }

    private void MoveToOriginPlatform(PlatformObject affectedPlatform)
    {
        //UnityEngine.Debug.Log($"Move To Origin");
        Vector3 direction = _defaultPos - affectedPlatform.UpdatePosition;       // ���� ���� ����
        float distance = Vector3.Distance(affectedPlatform.UpdatePosition, _defaultPos );
        if (distance > 0.1f)
        {
            affectedPlatform.UpdatePosition += direction.normalized * (Time.deltaTime * Speed);
        }
        else
        {
            affectedPlatform.UpdatePosition = _defaultPos;
            _isWait = false;
            StartPlatformStateChange(0f);
        }
    }

    private void StartPlatformStateChange(float rate)
    {
        /***************************************
         * ���� State�� ����Ǵµ� �ɸ��� �ð�.
         * **/
        _curWaitTime += Time.deltaTime;
        if(_curWaitTime > rate)
        {
            _movingType = MovingType.Up != _movingType ? _movingType  + 1 : MovingType.None;
            _curWaitTime = 0;
        }
        
    }


    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _platformTr = affectedPlatform.transform;
        _defaultPos = affectedPlatform.transform.position;
        _defaultQuat = _platformTr.rotation;
        //affectedPlatform.CheckGroundOffset = 2f;
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        switch (_movingType)
        {
            case MovingType.None:
                // �ƹ��� ����� ���� ����.
                break;
            case MovingType.Enter:
                // ��鸲 ����
                EarthQuake(affectedPlatform);
                StartPlatformStateChange(QuakeRate);
                break;
            case MovingType.Down:
                MoveToWordsPlatform(affectedPlatform);
                break;
            case MovingType.Up:
                MoveToOriginPlatform(affectedPlatform);

                break;
            default:
                break;
        }
        // waitTimer;       ���� State�� �����ϱ� ���� ��ٸ��� �ð�.

    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        /************************************************
         *  �÷����� ������ ���� ������ ���� ������ �߻��ϰ� enumState�� ����. ���� ��鸮�� ����� �ִ´�.
         *  **/
        if (!_isWait)
        {
            _isWait = true;
            _movingType = MovingType.Enter;         // ���� ���� �߻�
        }
    }

}
