using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/***************************************************
 *  플레이어가 무언가를 당기는 상태가 구현된 클래스입니다.
 * **/
public sealed class PullInOutState : State
{
    //======================================
    ////       Property and fields      ////
    ///=====================================
    public PullableObject PulledTarget { get; private set; }
    public float          PullPower    { get; set; }


    private Coroutine   _progressCoroutine;
    private const float _LookAtTime = 0.1f;


    //===================================
    ////       Override methods      ////
    //===================================
    public PullInOutState(Player player, StateMachine stateMachine)
    :base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.isPull = true;
    }

    public override void Exit()
    {
        player.isPull = false;

        if (_progressCoroutine != null){

            player.StopCoroutine(_progressCoroutine);
        }
    }


    //===================================
    ////       Core methods         /////
    ///==================================
    public void HoldTarget(GameObject target)
    {
        #region Omit
        Vector3 targetDistance = (target.transform.position - player.transform.position);

        bool isSameDir  = Vector3.Dot(player.transform.forward, targetDistance) >= 0f;
        bool isPullable = (PulledTarget = target.GetComponent<PullableObject>());

        /**당길 수 있는 객체라면 당기는 작업을 실행한다..*/
        if(isSameDir && isPullable){

            player.movementSM.ChangeState(this);

            if(_progressCoroutine!=null) player.StopCoroutine(_progressCoroutine);
            _progressCoroutine = player.StartCoroutine(PullingProgress());
        }
        #endregion
    }

    private IEnumerator PullingProgress()
    {
        player.animator.SetTrigger("pulling");
        float prevSpeed = player.animator.speed;

        /****************************************************
         *   당길 위치를 바라보는데 필요한 모든 요소들을 구한다...
         * **/
        Transform playerTr   = player.transform;
        Vector3 holdingPoint = PulledTarget.HoldingPoint.position;
        Vector3 playerPos    = playerTr.position;
        Vector3 forward      = playerTr.forward;

        Vector3    targetDistance = (holdingPoint - playerPos);
        Vector3    targetDir      = targetDistance.normalized;
        Quaternion startRot       = playerTr.rotation;

        float movDistance = targetDistance.magnitude;

        Vector3 lookTarget = -PulledTarget.PullingDir;
        Vector3 currForward = playerTr.forward;
        Vector3 cross = Vector3.Cross(lookTarget, currForward);
        float dirInput = Vector3.Dot(cross, Vector3.up);
        float rotDir = (dirInput >= 0 ? -1f : 1f);

        lookTarget.y = 0f;
        lookTarget.Normalize();

        float acosInput = Mathf.Clamp(Vector3.Dot(lookTarget, currForward), -1f, 1f);
        float radian = Mathf.Acos(acosInput);
        float degree = (Mathf.Rad2Deg * radian * rotDir);

        /*********************************
         *  지정한 위치를 바라본다...
         * **/
        float progressRatio = 0f;
        float currTime      = 0f;
        float goalTimeDiv   = ( 1f / _LookAtTime );

        do
        {
            float deltaTime = Time.deltaTime;
            currTime += deltaTime;
            progressRatio = Mathf.Clamp(( currTime * goalTimeDiv ), 0f, 1f);

            Quaternion rotQuat = Quaternion.AngleAxis(degree*progressRatio, Vector3.up);
            Vector3 movDelta   = targetDir * ( movDistance * progressRatio );
            movDelta.y = 0f;

            /**최종적용*/
            playerTr.rotation  = (startRot * rotQuat);
            playerTr.position  = (playerPos + movDelta);

            yield return null;
        }
        while (currTime<_LookAtTime);

        AnimatorStateInfo state = player.animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"normalizeTime: {state.normalizedTime}/ name: {state.length}");

        /***********************************
         *  애니메이션이 완료될 때까지 대기...
         * **/
        while (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime<1f) yield return null;

        Debug.Log("좋았어!!");
        yield break;
    }

    
}
