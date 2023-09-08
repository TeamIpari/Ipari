using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPlatformBehavior : PlatformBehaviorBase
{
    // affectedPlatform = 영향을 받고 있는 Platform object 
    // standing target = 밟은 GameObject를 참조함.
    // standingBody = rigidbody를 가지고 있으면 standingBody 파라미터가 null이 아님. < - 물리 쪽을 신경쓰고 작업한게 아님.
    // standingPoint = 밟은 객체가 현재 어디 좌표를 밟았는지를 체크하는 포인터
    // staindNormal = 밟은 지점의 Normal 값.

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {

    }

}
