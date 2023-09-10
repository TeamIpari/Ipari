using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IPariUtility
{
    /// <summary>
    /// Utilites
    /// </summary>
    public struct IpariUtility
    {
        /// <summary>
        /// �̰� �������� ����� �����̿�
        /// </summary>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
        {
            // define the distance x and y first;
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
            distanceXZ.y = 0f; // y�� 0���� ����.
                               //Forward = origin;
                               // Create a float the represent our distance
            float Sy = distance.y;      // ���� ������ �Ÿ��� ����.
            float Sxz = distanceXZ.magnitude;

            // �ӵ� �߰�
            float Vxz = Sxz / time;
            float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

            // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
            Vector3 result = distanceXZ.normalized;
            result *= Vxz;
            result.y = Vy;
            return result;
        }

        /// <summary>
        /// ���� ���͸� ���ϰ� 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirZ(Transform transform,float angleInDegree)
        {
            float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirX(Transform transform, float angleInDegree)
        {
            float radian = (angleInDegree - transform.eulerAngles.x) * Mathf.Deg2Rad;
            return new Vector3(0f, Mathf.Sin(radian), Mathf.Cos(radian));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirY(Transform transform, float angleInDegree)
        {
            float radian = (angleInDegree + transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        }
        internal static Vector3 AngleToDirY(Vector3 position, float angleInDegree)
        {
            float radian = (angleInDegree + position.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        }

        //internal static Vector3 AngleToDirZ(Transform transform, float v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
