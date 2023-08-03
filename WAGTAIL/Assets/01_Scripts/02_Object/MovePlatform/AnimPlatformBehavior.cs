using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/********************************************************
 * 애니메이션이 적용되어 있는 플레이어가 서있을 수 있는 땅입니다.
 ****/
[RequireComponent(typeof(Animator))]
public sealed class AnimPlatformBehavior : PlatformBehaviorBase
{
    //========================================
    //////     Property And Fields      /////
    //========================================
    [SerializeField] public string AnimClipName = string.Empty;

    private Animator       _animator;
    private PlatformObject _platformObject;


    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _platformObject = affectedPlatform;
        _animator = GetComponent<Animator>();
        affectedPlatform.CheckGroundOffset = 5f;
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget)
    {
        if(_platformObject== affectedPlatform) 
            _platformObject = null;
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        if (_animator == null) return;

        _animator.Play(AnimClipName);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        standingTarget.transform.position = standingPoint;
    }

    private void LateUpdate()
    {
         if(_platformObject==null || _platformObject.PlayerRideOn==false) return;

         RaycastHit hit;
        _platformObject.GetPlayerFloorinfo(out hit);
         Player.Instance.transform.position = hit.point;
    }

}
