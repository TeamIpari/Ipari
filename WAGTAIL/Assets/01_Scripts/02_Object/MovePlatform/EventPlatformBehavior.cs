using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPlatformBehavior : PlatformBehaviorBase
{
    // affectedPlatform = ������ �ް� �ִ� Platform object 
    // standing target = ���� GameObject�� ������.
    // standingBody = rigidbody�� ������ ������ standingBody �Ķ���Ͱ� null�� �ƴ�. < - ���� ���� �Ű澲�� �۾��Ѱ� �ƴ�.
    // standingPoint = ���� ��ü�� ���� ��� ��ǥ�� ��Ҵ����� üũ�ϴ� ������
    // staindNormal = ���� ������ Normal ��.

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {

    }

}
