using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

/**************************************************
 *  이동하는 플랫폼에 대한 움직임이 정의된 컴포넌트입니다.
 * ***/
[AddComponentMenu("Platform/MovingPlatformBehavior")]
public sealed class MovingPlatformBehavior : PlatformBehaviorBase
{
    // 현재 위치에서 지정된 방향으로 이동하는 기능을 구현할 것입니다. 네.
    // 상태는 4가지 전조증상(Enter), 내려감(Down), 올라옴(Up), 대기(None),

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
         * 전조 현상 LandedType.Enter 일 때 Tile을 흔들어주는 기능을 함.
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
         * 전조현상이 끝나면 첫 위치에서 최종적으로 이동할 위치까지 연산함.
         * **/
        //UnityEngine.Debug.Log($"Move To Words");
        Vector3 direction = affectedPlatform.UpdatePosition - (_defaultPos + TargetPosition);       // 방향 벡터 결정
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
        Vector3 direction = _defaultPos - affectedPlatform.UpdatePosition;       // 방향 벡터 결정
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
         * 다음 State로 변경되는데 걸리는 시간.
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
                // 아무런 기능을 하지 않음.
                break;
            case MovingType.Enter:
                // 흔들림 현상
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
        // waitTimer;       다음 State로 변경하기 위해 기다리는 시간.

    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        /************************************************
         *  플랫폼을 밟으면 밟은 시점에 전조 증상이 발생하게 enumState를 변경. 이후 흔들리는 기능을 넣는다.
         *  **/
        if (!_isWait)
        {
            _isWait = true;
            _movingType = MovingType.Enter;         // 전조 증상 발생
        }
    }

}
