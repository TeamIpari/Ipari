using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**************************************************
 *  이동하는 플랫폼에 대한 움직임이 정의된 컴포넌트입니다.
 * ***/
[AddComponentMenu("Platform/MovingPlatformBehavior")]
public sealed class MovingPlatformBehavior : PlatformBehaviorBase
{
    // 현재 위치에서 지정된 방향으로 이동하는 기능을 구현할 것입니다. 네.
    // 상태는 4가지 전조증상(Enter), 내려감(Down), 올라옴(Up), 대기(None),

    private enum LandedType
    {
        None,
        Enter,
        Down,
        Up,
    }

    //========================================
    //////           Property            /////
    //========================================
    [SerializeField] public float       Speed           = 0;
    [SerializeField] public float       ShakeDistance    = 0;
    [SerializeField] public float       shakeDelay      = 0;
    [SerializeField] public Vector3     TargetPosition  = Vector3.zero;


    //=======================================
    //////      Private Fields          /////
    //=======================================
    private float                       curTime         = 0f;

    private Vector3                     _TargetPos      = Vector3.zero;
    private Vector3                     _defaultPos     = Vector3.zero;
    private LandedType                  _landedType     = LandedType.None;
    private Quaternion                  _defaultQuat    = Quaternion.identity;
    private Transform                   _platformTr;


    private void EarthQuake(PlatformObject affectedPlatform)
    {
        /************************************************
         * 전조 현상 LandedType.Enter 일 때 Tile을 흔들어주는 기능을 함.
         * 왜 이전엔 가능했는가? game object 안에 들어있었기 때문에 0, 0, 0 위치에서 변동이 일어났으므로 가능했음. 때문에 좌표값이 완전히 다른 현 코드에서는 다른 로직을 써야함.
         * **/
        curTime += Time.deltaTime;
        if (curTime > shakeDelay)
        {
            Vector3 offset = _platformTr.position + (UnityEngine.Random.insideUnitSphere * ShakeDistance);
            _TargetPos = new Vector3(offset.x, _platformTr.position.y, offset.z);
            affectedPlatform.UpdatePosition +=  _TargetPos.normalized;
            curTime = 0;
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
        switch (_landedType)
        {
            case LandedType.None:
                // 아무런 기능을 하지 않음.
                break;
            case LandedType.Enter:
                // 흔들림 현상
                EarthQuake(affectedPlatform);
                break;
            case LandedType.Down:

                break;
            case LandedType.Up:

                break;
            default:
                break;
        }
        Debug.Log("BB");
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        /************************************************
         *  플랫폼을 밟으면 밟은 시점에 전조 증상이 발생하게 enumState를 변경. 이후 흔들리는 기능을 넣는다.
         *  **/

        _landedType = LandedType.Enter;         // 전조 증상 발생

        Debug.Log("AA");
    }

}
