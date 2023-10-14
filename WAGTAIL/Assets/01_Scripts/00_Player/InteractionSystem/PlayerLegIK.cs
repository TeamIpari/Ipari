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

        /*********************************************
         *    Lef IK를 적용한다....
         * ****/
        ApplyLegIK( LThigh, LCalf, LFoot);
        ApplyLegIK( RThigh, RCalf, RFoot);

        #endregion
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position + (Vector3.up * .655f);
        Vector3 LPos     = position + (transform.right * -.2f);
        Vector3 RPos     = position + (transform.right * .2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(position, .2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LPos, .2f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(RPos, .2f);
    }



    //================================================
    ///////          Utility methods            //////
    //================================================
    private bool GetSteppedPosition( out RaycastHit result, Vector3 startPosition, Vector3 dir )
    {
        #region Omit
        return Physics.SphereCast(

            startPosition,
            .05f,
            dir,
            out result,
            .2f,
            _raycastLayer,
            QueryTriggerInteraction.Ignore

        );

        #endregion
    }

    private void ApplyLegIK( Transform thigh, Transform calf, Transform foot )
    {
        #region Omit
        Vector3 startPosition = foot.position + (Vector3.up);

        RaycastHit result;
        Physics.SphereCast(

            startPosition,
            .05f,
            Vector3.down,
            out result,
            1.4f,
            _raycastLayer,
            QueryTriggerInteraction.Ignore

        );

        if (result.collider == null) return;

        /**계산에 필요한 요소들을 구한다....*/
        Debug.DrawLine(result.point, result.point + result.normal * 10f, Color.red);
        float   footRaiseOffset = (result.point.y - foot.position.y)+.18f;
        Vector3 right           = Vector3.Cross(transform.forward, result.normal);
        Vector3 forward         = -Vector3.Cross(right, result.normal);


        /**갱신할 발의 회전값, 위치값을 구한다....*/
        Vector3     footDir       = foot.right;
        Vector3     footGoalDir   = result.normal;
        Vector3     footPos       = foot.position + ((result.normal+forward).normalized * footRaiseOffset);
        Quaternion  footRot       = (IpariUtility.GetQuatBetweenVector(footDir, footGoalDir) * foot.rotation);

        /**갱신할 정강이의 회전값, 위치값을 구한다....*/
        Vector3    calfDir     = (foot.position - calf.position).normalized;
        Vector3    calfGoalDir = (footPos - calf.position).normalized;
        Vector3    calfPos     = footPos - (calfGoalDir * _calf2FootLen);
        Quaternion calfRot     = (IpariUtility.GetQuatBetweenVector(calfDir, calfGoalDir) * calf.rotation);

        /**갱신할 골반의 회전값을 구한다...*/
        Vector3     thighDir        = (calf.position - thigh.position).normalized;
        Vector3     thighGoalDir    = (calfPos - thigh.position).normalized;
        Vector3     thighPos        = calfPos - (thighGoalDir* _thigh2CalfLen);
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
