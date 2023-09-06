using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**************************************************
 *  �̵��ϴ� �÷����� ���� �������� ���ǵ� ������Ʈ�Դϴ�.
 * ***/
[AddComponentMenu("Platform/MovingPlatformBehavior")]
public sealed class MovingPlatformBehavior : PlatformBehaviorBase
{
    // ���� ��ġ���� ������ �������� �̵��ϴ� ����� ������ ���Դϴ�. ��.
    // ���´� 4���� ��������(Enter), ������(Down), �ö��(Up), ���(None),

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
         * ���� ���� LandedType.Enter �� �� Tile�� �����ִ� ����� ��.
         * �� ������ �����ߴ°�? game object �ȿ� ����־��� ������ 0, 0, 0 ��ġ���� ������ �Ͼ���Ƿ� ��������. ������ ��ǥ���� ������ �ٸ� �� �ڵ忡���� �ٸ� ������ �����.
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
                // �ƹ��� ����� ���� ����.
                break;
            case LandedType.Enter:
                // ��鸲 ����
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
         *  �÷����� ������ ���� ������ ���� ������ �߻��ϰ� enumState�� ����. ���� ��鸮�� ����� �ִ´�.
         *  **/

        _landedType = LandedType.Enter;         // ���� ���� �߻�

        Debug.Log("AA");
    }

}
