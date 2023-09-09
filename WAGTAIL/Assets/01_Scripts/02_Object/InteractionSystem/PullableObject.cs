using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************
 *   ����� �� �ִ� ������ �����ϴ� ������Ʈ�Դϴ�...
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
