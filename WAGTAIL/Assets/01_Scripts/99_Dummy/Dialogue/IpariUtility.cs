using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IPariUtility
{
    /// <summary>
    /// Utilites
    /// </summary>
    public struct IpariUtility
    {
        #region Define
        private struct VibrationDesc
        {
            public Vector2 VibeVec;
            public float   TimeLeft;
            public int     Id;
        }
        #endregion

        //==============================================
        //////               Fields                 ////
        //==============================================
        private static Coroutine        _padCoroutne;
        private static int              _vibeNum   = 0;
        private static VibrationDesc[]  _vibeDescs = new VibrationDesc[10];



        //===============================================
        //////              Core methods            /////
        //===============================================

        /// <summary>
        /// 이건 포물선을 사용한 공식이여
        /// </summary>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        internal static Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
        {
            #region Omit
            // define the distance x and y first;
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
            distanceXZ.y = 0f; // y는 0으로 설정.
                               //Forward = origin;
                               // Create a float the represent our distance
            float Sy = distance.y;    // 세로 높이의 거리를 지정.
            float Sxz = distanceXZ.magnitude;

            // 속도 추가
            float Vxz = Sxz / time;
            float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
            // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
            Vector3 result = distanceXZ.normalized;
            result *= Vxz;
            result.y = Vy;
            return result;
            #endregion
        }

        /// <summary>
        /// 8방향을 랜덤으로 출력, Vector3.normalized;
        /// </summary>
        /// <returns></returns>
        internal static Vector3 RandomDirection()
        {
            #region Omit
            Vector3 Direction = Vector3.zero;
            // 8방향으로 이동이 가능하게 할 예정.
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
            #endregion
        }

        /// <summary>
        /// 방향 벡터를 구하고 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirZ(Transform transform,float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirX(Transform transform, float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree - transform.eulerAngles.x) * Mathf.Deg2Rad;
            return new Vector3(0f, Mathf.Sin(radian), Mathf.Cos(radian));
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="angleInDegree"></param>
        /// <returns></returns>
        internal static Vector3 AngleToDirY(Transform transform, float angleInDegree)
        {
            #region Omit
            float radian = (angleInDegree + transform.eulerAngles.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion
        }
        internal static Vector3 AngleToDirY(Vector3 position, float angleInDegree)
        {
            #region Omit   
            float radian = (angleInDegree + position.y) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
            #endregion 
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
             *   주어진 두 벡터사이의 쿼터니언 값을 계산한다...
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

        internal static void PlayGamePadVibration( float leftPow, float rightPow, float time, int id=0 )
        {
            #region Omit
            /**공간이 부족하면 확장한다...*/
            if(_vibeNum>=_vibeDescs.Length)
            {
                VibrationDesc[] descs = new VibrationDesc[ _vibeNum*2 ];
                _vibeDescs.CopyTo( descs, 0 );
                _vibeDescs = descs;
            }

            ref VibrationDesc desc = ref _vibeDescs[ _vibeNum++ ];
            desc.VibeVec  = new Vector2(leftPow, rightPow);
            desc.TimeLeft = time;
            desc.Id       = id;

            /**패드 진동을 적용한다...*/
            Gamepad currPad = Gamepad.current;
            if(currPad!=null){

                float xx = (GamePadUIController.LastInputGamePadKind == GamePadUIController.GamePadKind.XBox? 3f:1f);
                currPad.SetMotorSpeeds(leftPow*xx, rightPow*xx);
            }


            /**진동 코루틴을 실행한다...*/
            if(_padCoroutne==null){

                _padCoroutne = GameManager.GetInstance().StartCoroutine(PadVibrationProgress());
            }
            #endregion
        }

        internal static void StopGamePadVibration(int stopID)
        {
            #region Omit
            for(int i=0; i<_vibeNum; i++)
            {
                ref VibrationDesc desc = ref _vibeDescs[i];

                /**ID가 동일하면 종료시킨다...*/
                if(stopID==desc.Id)
                {
                    _vibeDescs[i] = _vibeDescs[_vibeNum - 1];
                    _vibeNum--;
                    i--;
                }
            }
            #endregion
        }

        internal static void ClearUtilityState()
        {
            #region Omit
            _padCoroutne = null;
            _vibeNum     = 0;
            Gamepad.current?.SetMotorSpeeds(0f, 0f);
            #endregion
        }



        //===================================================
        ///////             Core methods               //////
        //===================================================

        private static IEnumerator PadVibrationProgress()
        {
            #region Omit
            /************************************************
             *   게임 패드 진동을 강도순으로 차례대로 적용된다...
             * ****/
            Gamepad current    = Gamepad.current;
            int     applyIndex = _vibeNum;
            bool    isChange   = false;

            while(current!=null && _vibeNum>0)
            {
                float deltaTime   = Time.unscaledDeltaTime;

                /**모든 로직을 적용한다....*/
                for (int i = 0; i < _vibeNum; i++){

                    /**해당 진동이 마무리 되었다면 다음 진동을 적용한다....*/
                    if(ApplyPadVibration(i, deltaTime, ref isChange, current))
                    {
                        i--;
                        isChange = true;
                    }
                }

                yield return null;
            }

            /**게임패드의 진동을 종료한다....*/
            if (current != null) current.SetMotorSpeeds(0f, 0f);
            _padCoroutne = null;

            #endregion
        }

        private static bool ApplyPadVibration(int index, float deltaTime, ref bool isChange, Gamepad current)
        {
            #region Omit
            ref VibrationDesc desc = ref _vibeDescs[index];

            /**진동이 마무리되었을 경우....*/
            if((desc.TimeLeft -= deltaTime)<=0f){

                _vibeDescs[index] = _vibeDescs[_vibeNum-1];
                _vibeNum--;
                return true;
            }

            if (isChange && current != null)
            {
                isChange = false;

                float xx = (GamePadUIController.LastInputGamePadKind == GamePadUIController.GamePadKind.XBox ? 3f : 1f);
                current.SetMotorSpeeds(desc.VibeVec.x*xx, desc.VibeVec.y*xx);
            }

            return false;
            #endregion
        }
    }
}
