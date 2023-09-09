using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 *   당겨질 수 있는 로직을 제공하는 컴포넌트입니다...
 * **/
public sealed class PullableObject : MonoBehaviour
{
    //=================================
    ////          Property         ////
    //=================================
    [SerializeField] public Transform HoldingPoint;
    [SerializeField] public Vector3   PullingDir;


    //====================================
    /////        Magic methods        ////
    ///===================================
    private void OnDrawGizmos()
    {
        if(HoldingPoint!=null){

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(HoldingPoint.position, .1f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(HoldingPoint.position, HoldingPoint.position + ( PullingDir * .5f ));
        }
    }


    //====================================
    /////       Public methods        ////
    ///===================================
    public void Pull( float power, Vector3 dir )
    {
        //TODO:
    }



}
