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
            float Sy = distance.y;    // ���� ������ �Ÿ��� ����.
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
        /// 8������ �������� ���, Vector3.normalized;
        /// </summary>
        /// <returns></returns>
        internal static Vector3 RandomDirection()
        {
            Vector3 Direction = Vector3.zero;
            // 8�������� �̵��� �����ϰ� �� ����.
            switch (UnityEngine.Random.Range(0, 8))
            {
                case 0:
                    Direction = new Vector3(0, 0, 1); break;
                case 1:
                    Direction = new Vector3(1, 0, 1); break;
                case 2:
                    Direction = new Vector3(0, 0, 1); break;
                case 3:
                    Direction = new Vector3(1, 0, -1); break;
                case 4:
                    Direction = new Vector3(0, 0, -1); break;
                case 5:
                    Direction = new Vector3(-1, 0, -1); break;
                case 6:
                    Direction = new Vector3(-1, 0, 0); break;
                case 7:
                    Direction = new Vector3(-1, 0, 1); break;
            }
            //Debug.Log(Direction);
            return Direction.normalized;
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
        
        internal static Vector3 GetBezier(ref Vector3 s, ref Vector3 c, ref Vector3 d, float w = 0f)
        {
            #region Omit
            Vector3 sc = (c - s);
            Vector3 cd = (d - c);

            Vector3 a2 = s + (sc * w);
            Vector3 b2 = c + (cd * w);
            Vector3 c2 = (b2 - a2);

            return a2 + (c2 * w);
            #endregion
        }

        internal static Quaternion GetQuatBetweenVector(Vector3 from, Vector3 to, float ratio = 1f)
        {
            #region Omit

            /********************************************
             *   �־��� �� ���ͻ����� ���ʹϾ� ���� ����Ѵ�...
             * ***/
            float angle         = Vector3.Angle(from, to) * ratio;
            Vector3 cross       = Vector3.Cross(from, to);
            return Quaternion.AngleAxis(angle, cross);
            #endregion
        }

        internal static  bool GetPlayerFloorinfo(out RaycastHit result, int layerMask, Vector3 startOffset=default, float extraDistance=0f)
        {
            #region Omit
            CharacterController con = Player.Instance.controller;

            float heightHalf        = con.height;
            float radius            = con.radius;
            float heightHalfOffset  = (heightHalf * .5f) - radius;
            Vector3 playerPos       = con.transform.position;
            Vector3 center          = (playerPos + con.center + startOffset);

            return Physics.SphereCast(
                center,
                radius,
                Vector3.down,
                out result,
                (heightHalf + .1f + extraDistance),
                layerMask
            );
            #endregion
        }

        //internal static Vector3 AngleToDirZ(Transform transform, float v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
