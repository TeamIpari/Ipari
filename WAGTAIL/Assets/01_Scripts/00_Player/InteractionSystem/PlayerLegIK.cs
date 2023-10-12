using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**********************************************
 *    플레이어의 Leg IK가 구현된 컴포넌트입니다...
 * *****/
public sealed class PlayerLegIK : MonoBehaviour
{
    //=======================================
    /////           Property            /////
    //=======================================
    [Header("Left Leg")]

    [SerializeField] public Transform LThigh;
    [SerializeField] public Transform LCalf;
    [SerializeField] public Transform LFoot;


    [Header("Right Leg")]

    [SerializeField] public Transform RThigh;
    [SerializeField] public Transform RCalf;
    [SerializeField] public Transform RFoot;



    //===========================================
    //////              Fields              /////
    //===========================================
    private bool _initComplete      = false;
    private float _tr2ThighLen      = 0f;
    private float _thigh2CalfLen    = 0f;
    private float _legLen           = 0f;
    private float _calf2FootLen     = 0f;
    private int _raycastLayer       = 0;

    RaycastHit LResult, RResult;



    //===========================================
    //////          Magic methods           /////
    //===========================================
    private void Start()
    {
        if(_initComplete = ( LThigh && LCalf && LFoot && RThigh && RCalf && RFoot )){

            _thigh2CalfLen = (LCalf.position - LThigh.position).magnitude;
            _calf2FootLen  = (LFoot.position - LCalf.position).magnitude;
            _legLen        = (_thigh2CalfLen + _calf2FootLen);
            _tr2ThighLen   = (LThigh.position - transform.position).y;
            _raycastLayer  = ~(1 << LayerMask.NameToLayer("Player"));
        }
    }

    private void LateUpdate()
    {
        #region Omit
        if (_initComplete == false) return;

        /*****************************************
         *    각 발들이 밟고있는 부분을 구한다....
         * ****/
        Vector3 position    = transform.position;
        Vector3 LCalfPoint  = transform.position + (transform.right * -.3f)+(transform.forward*.3f);
        Vector3 LCalf2Foot  = (LFoot.position - LCalf.position).normalized;
        LCalfPoint.y = LFoot.position.y;

        Vector3 RCalfPoint  = transform.position + (transform.right * .3f) + (transform.forward * .3f);
        Vector3 RCalf2Foot  = (RFoot.position - RCalf.position).normalized;
        RCalf2Foot.y = RFoot.position.y;

        //RaycastHit LResult, RResult;
        GetSteppedPosition(out LResult, LCalf.position, LCalf2Foot);
        GetSteppedPosition(out RResult, RCalf.position, RCalf2Foot);

        Debug.DrawLine(LCalfPoint, LCalfPoint+Vector3.down * _legLen, Color.yellow);    
        Debug.DrawLine(LResult.point, LResult.point + LResult.normal * 10f, Color.blue);
        Debug.DrawLine(RResult.point, RResult.point + RResult.normal * 10f, Color.blue);


        /*********************************************
         *    Lef IK를 적용한다....
         * ****/
        ApplyLegIK(ref LResult, LThigh, LCalf, LFoot);
        ApplyLegIK(ref RResult, RThigh, RCalf, RFoot);

        #endregion
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(LFoot.position, .1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(LResult.point, .1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(RFoot.position, .1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(RResult.point, .1f);
    }



    //================================================
    ///////          Utility methods            //////
    //================================================
    private bool GetSteppedPosition( out RaycastHit result, Vector3 startPosition, Vector3 dir )
    {
        #region Omit
        return Physics.SphereCast(

            startPosition,
            .1f,
            dir,
            out result,
            _calf2FootLen + .2f,
            _raycastLayer,
            QueryTriggerInteraction.Ignore

        );

        #endregion
    }

    private void ApplyLegIK( ref RaycastHit result, Transform thigh, Transform calf, Transform foot )
    {
        #region Omit
        if (result.collider == null || result.normal.y<=0f) return;

        /**계산에 필요한 요소들을 구한다....*/
        float   footRaiseOffset = (result.point - foot.position).magnitude;
        Vector3 right           = Vector3.Cross(transform.forward, result.normal);
        Vector3 forward         = -Vector3.Cross(right, result.normal);


        /**갱신할 발의 회전값, 위치값을 구한다....*/
        Vector3     footDir       = foot.right;
        Vector3     footGoalDir   = result.normal;
        Vector3     footPos       = result.point + (result.normal * .15f) + (forward*footRaiseOffset);
        Quaternion  footRot       = (IpariUtility.GetQuatBetweenVector(footDir, footGoalDir) * foot.rotation);

        /**갱신할 정강이의 회전값, 위치값을 구한다....*/
        Vector3    calfDir     = (foot.position - calf.position).normalized;
        Vector3    calfGoalDir = (footPos - calf.position).normalized;
        Vector3    calfPos     = footPos - (calfGoalDir * _calf2FootLen);
        Quaternion calfRot     = (IpariUtility.GetQuatBetweenVector(calfDir, calfGoalDir) * calf.rotation);

        /**갱신할 골반의 회전값을 구한다...*/
        Vector3     thighDir        = (calf.position - thigh.position).normalized;
        Vector3     thighGoalDir    = (calfPos - thigh.position).normalized;
        Vector3     thighPos        = calfPos - (thighDir* _thigh2CalfLen);
        Quaternion  thighRot        = (IpariUtility.GetQuatBetweenVector(thighDir, thighGoalDir) * thigh.rotation);


        /**최종 적용...*/
        thigh.position = thighPos;
        thigh.rotation = thighRot;

        calf.position = calfPos;
        calf.rotation = calfRot;

        foot.position = footPos;
        foot.rotation = footRot;

        #endregion
    }
}
