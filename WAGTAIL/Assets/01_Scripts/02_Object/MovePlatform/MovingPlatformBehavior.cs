using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************************
 *  이동하는 플랫폼에 대한 움직임이 정의된 컴포넌트입니다.
 * ***/
public sealed class MovingPlatformBehavior : PlatformBehaviorBase
{
    // 현재 위치에서 지정된 방향으로 이동하는 기능을 구현할 것입니다. 네.
    // 상태는 4가지 전조증상(Already), 내려감(Down), 올라옴(Up), 대기(None),
    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {


    }

}
