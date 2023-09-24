using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

/********************************************************
 * 애니메이션이 적용되어 있는 플레이어가 서있을 수 있는 땅입니다.
 ****/
[AddComponentMenu("Platform/AnimPlatformBehavior")]
public sealed class AnimPlatformBehavior : PlatformBehaviorBase
{
    //========================================
    //////     Property And Fields      /////
    //========================================
    [SerializeField] public string AnimClipName = string.Empty;
    [SerializeField] public float StartSpeed = 0f;
    [SerializeField] public float Speed = 1f;
    [SerializeField] private Animator       _animator;

    private PlatformObject _platformObject;


    //=======================================
    //////      Override Methods          ////
    //=======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        _platformObject = affectedPlatform;
        if(_animator==null) _animator = GetComponent<Animator>();
        
        if(_animator!=null)
        {
            _animator.speed = StartSpeed;
        }
        //affectedPlatform.CheckGroundOffset = 10f;
        //affectedPlatform.CheckGroundDownOffset = 10f;
        affectedPlatform.PreventAddChild = true;
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody)
    {
        if(_platformObject== affectedPlatform) 
            _platformObject = null;
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        if (_animator == null) return;
        _animator.Play(AnimClipName, 0,0f);
        _animator.speed = Speed;
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        //standingTarget.transform.position = standingPoint;
    }

    private void LateUpdate()
    {
         if(_platformObject==null || _platformObject.PlayerOnPlatform==false) return;

        RaycastHit hit;
        _platformObject.GetPlayerFloorinfo(out hit);
        Player.Instance.transform.position = hit.point;
    }

    private void OnDrawGizmos()
    {
        
    }

}
