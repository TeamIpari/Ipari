using System;
using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        private struct SFXColorSample
        {
            public Vector3 Color;
            public float   ParamValue;
        }

        public enum FadeOutType
        {
            DARK_TO_WHITE = 0b100,
            WHITE_TO_DARK = 0b011,
            DARK_TO_WHITE_TO_DARK = 0b101,
            WHITE_TO_DARK_TO_WHITE = 0b010
        }

        public delegate void OnFadeCompleteNotify(bool isDark, int id);
        #endregion

        //==============================================
        //////               Fields                 ////
        //==============================================
        public static OnFadeCompleteNotify OnFadeChange;

        private static Coroutine          _padCoroutne;
        private static int                _vibeNum       = 0;
        private static VibrationDesc[]    _vibeDescs     = new VibrationDesc[10];

        /**바닥 환경 관련....*/
        private static Terrain            _lastTerrain   = null;
        private static float[]            _layerSFXTypes = new float[10];
        private static SFXColorSample[]   _texColors = new SFXColorSample[]
        {
            new SFXColorSample{ Color=new Vector3(117f/255f, 106f/255f, 73f/255f), ParamValue=FModParamLabel.EnvironmentType.Ground}, //흙(1)
            new SFXColorSample{ Color=new Vector3(194f/255f, 143f/255f, 93f/255f), ParamValue=FModParamLabel.EnvironmentType.Ground}, //흙(2)
            new SFXColorSample{ Color=new Vector3(123f/255f, 95/255f, 62/255f), ParamValue=FModParamLabel.EnvironmentType.Ground}, //흙(3)

            new SFXColorSample{ Color=new Vector3(58f/255f, 95f/255f, 40f/255f),   ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(1)
            new SFXColorSample{ Color=new Vector3(97f/255f, 126f/255f, 67f/255f),   ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(2)
            new SFXColorSample{ Color=new Vector3(63f/255f, 98f/255f, 29f/255f),   ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(3)
            new SFXColorSample{ Color=new Vector3(56f/255f, 94f/255f, 38f/255f),   ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(4)
            new SFXColorSample{ Color=new Vector3(225f/255f, 171f/255f, 110f/255f), ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(5)
            new SFXColorSample{ Color=new Vector3(231f/255f, 184/255f, 120/255f), ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(5)
            new SFXColorSample{ Color=new Vector3(142/255f, 171/255f, 66f/255f), ParamValue=FModParamLabel.EnvironmentType.Grass}, //풀숲(6)

            new SFXColorSample{ Color=new Vector3(208f/255f, 168f/255f, 101f/255f), ParamValue=FModParamLabel.EnvironmentType.Sand }, //모래(1)

            new SFXColorSample{ Color=new Vector3(121f/255f, 118f/255f, 104f/255f), ParamValue=FModParamLabel.EnvironmentType.Stone}, //돌(1)
            new SFXColorSample{ Color=new Vector3(140f/255f, 129f/255f, 111f/255f), ParamValue=FModParamLabel.EnvironmentType.Stone}, //돌(2)
            new SFXColorSample{ Color=new Vector3(230f/255f, 223/255f, 206/255f), ParamValue=FModParamLabel.EnvironmentType.Stone}, //돌(3)

            new SFXColorSample{ Color=new Vector3(213f/255f, 173f/255f, 110f/255f), ParamValue=FModParamLabel.EnvironmentType.Wood }, //나무(1)
            new SFXColorSample{ Color=new Vector3(205f/255f, 156f/255f, 93f/255f), ParamValue=FModParamLabel.EnvironmentType.Wood }, //나무(2)
            new SFXColorSample{ Color=new Vector3(123f/255f, 81/255f, 57/255f), ParamValue=FModParamLabel.EnvironmentType.Wood }, //나무(3)
        };



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
        internal static Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time, float height = 1.5f)
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
            float Vy = Sy / time + height * Mathf.Abs(Physics.gravity.y) * time;
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
            /*********************************************************
             *   게임패드의 우측/좌측 모터에 진동을 일정시간만큼 가합니다.
             * ****/

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
            OnFadeChange = null;
            #endregion
        }

        internal static FModParameterReference GetFloorSFXType(Vector3 worldPosition, int layer=1)
        {
            #region Omit

            /*********************************************************
             *   주어진 worldPosition이 위치한 바닥의 종류를 조사한다...
             * ****/
            RaycastHit ret;
            float      paramValue = 0f;
            if (Physics.Raycast(   worldPosition+(Vector3.up),
                                   Vector3.down,
                                   out ret,
                                   1.1f,
                                   layer,
                                   QueryTriggerInteraction.Ignore ))
            {
                /**터레인인지 확인한다....*/
                bool TerrainIsChanged = (_lastTerrain != null && !_lastTerrain.gameObject.Equals(ret.collider.gameObject) && (_lastTerrain = ret.collider.GetComponent<Terrain>()) != null)
                                        || (_lastTerrain == null && (_lastTerrain = ret.collider.GetComponent<Terrain>()) != null);

                bool isTerrain = (_lastTerrain!=null);


                /*********************************************
                 *   터레인을 밟고 있는지 확인하고, 맞다면 해당
                 *   터레인을 캐싱한다....
                 * ******/
                Renderer renderer = null;
                if (isTerrain){

                    /**터레인이 변경되었을 경우, 터레인 레이어 룩업테이블을 갱신한다.....*/
                    if(TerrainIsChanged)
                    {
                        TerrainLayer[] layers = _lastTerrain.terrainData.terrainLayers;
                        UpdateTerrainLayerSFXLists(layers);
                    }

                    int index = GetTerrainLayer(ConvertTerrainPosition(worldPosition, _lastTerrain), _lastTerrain);
                    paramValue = _layerSFXTypes[index];
                }

                /**********************************************
                 *    터레인이 아닌 GameObject들에 대한 처리.....
                 * *******/
                else if((renderer=ret.collider.GetComponent<Renderer>()))
                {
                    Vector2    hitCoord = ret.textureCoord;
                    Texture2D  tex      = renderer.sharedMaterial.mainTexture as Texture2D;

                    if(tex==null)
                    {
#if UNITY_EDITOR
                        UnityEngine.Debug.Log($"밟은 땅(Name: {ret.collider.name})/ (텍스쳐를 읽어올 수 없음!!)");
#endif
                        FModParameterReference paramRefFail = new FModParameterReference();
                        paramRefFail.SetParameter(FModLocalParamType.EnvironmentType, FModParamLabel.EnvironmentType.Wood);
                        return paramRefFail;
                    }
                    hitCoord.x *= tex.width;
                    hitCoord.y *= tex.height;

                    int   index  = 0;
                    Color sample = tex.GetPixel(Mathf.FloorToInt(hitCoord.x), Mathf.FloorToInt(hitCoord.y));
                    paramValue   = GetSFXTypeFromColorSamples(sample, out index);

#if UNITY_EDITOR
                    Vector3 sample2 = new Vector3(sample.r * 255f, sample.g * 255f, sample.b * 255f);
                    UnityEngine.Debug.Log($"밟은 땅 (Name: {ret.collider.name})/ (index: {index})/ (Color: {sample2})");
#endif
                }


            }

            FModParameterReference paramRef = new FModParameterReference();
            paramRef.SetParameter(FModLocalParamType.EnvironmentType, paramValue);   

            return paramRef;
            #endregion
        }

        internal static void ApplyImageFade(FadeOutType type, Image target, float changeTime, float secondChangeDelay = 0f, int id = 0, Color startColor = default, float whiteAlpha = 0f, float darkAlpha = 1f, float startDelay = 0f, Color? goalColor = null)
        {
            GameManager.GetInstance().StartCoroutine(FadeOutProcess(type, target, changeTime, secondChangeDelay, id, startColor, whiteAlpha, darkAlpha, startDelay, goalColor));
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

        private static Vector2 ConvertTerrainPosition(Vector3 pos, Terrain targetTerrain)
        {
            #region Omit
            if (targetTerrain == null) return Vector2.zero;

            /*************************************************
             *   주어진 좌표를 터레인 좌표계로 변환한다.....
             * *******/

            /**터레인의 원점을 기준으로*/
            TerrainData data        = targetTerrain.terrainData;
            Vector3     terrainPos  = (pos-targetTerrain.transform.position);
            Vector3     mapPos      = new Vector3(

                (terrainPos.x / data.size.x),
                0f,
                (terrainPos.z / data.size.z)
            );

            float xCoord = Mathf.Clamp(mapPos.x * data.alphamapWidth, 0f, data.alphamapWidth-1);
            float zCoord = Mathf.Clamp(mapPos.z * data.alphamapHeight, 0f, data.alphamapHeight-1);

            return new Vector3((int)xCoord, (int)zCoord);
            #endregion
        }

        private static int GetTerrainLayer(Vector2 position, Terrain terrainObject)
        {
            #region Omit
            float[,,] aMap = terrainObject.terrainData.GetAlphamaps((int)position.x, (int)position.y, 1, 1);
            int tLayer = 0;
            float lastHighest = 0;

            for (int x = 0; x < aMap.GetLength(0); x++)
            {
                for (int y = 0; y < aMap.GetLength(1); y++)
                {
                    for (int z = 0; z < aMap.GetLength(2); z++)
                    {
                        if (aMap[x, y, z] > lastHighest)
                        {
                            lastHighest = aMap[x, y, z];
                            tLayer = z;
                        }
                    }
                }
            }

            return tLayer;
            #endregion
        }

        private static void UpdateTerrainLayerSFXLists(TerrainLayer[] layers)
        {
            #region Omit
            if (layers == null) return;

            /**************************************************
             *   터레인 레이어 수에 알맞게 SFXLists를 확장한다...
             * ***/
            int tableLength = (_layerSFXTypes==null? 0:_layerSFXTypes.Length);
            int layerCount  = layers.Length;
            if(tableLength < layerCount){

                _layerSFXTypes = new float[layerCount];
            }


            /***************************************************
             *   레이어의 이름에 따른 적절한 SFX Type을 채워넣는다...
             * ******/
            for(int i=0; i<layerCount; i++){

                string str;
                _layerSFXTypes[i] = GetSFXTypeFromTerrainLayer(layers[i], out str);
            }

            #endregion
        }

        private static float GetSFXTypeFromTerrainLayer(TerrainLayer layer, out string str)
        {
            #region Omit
            str = "??";
            if (layer == null) return -1;

            string layerName = layer.name;

            /**퓰을 밟았을 경우...*/
            if (layerName.Contains("Grass") || layerName.Contains("Moss") || layerName.Contains("cliff")) {

                str = "Grass";
                return FModParamLabel.EnvironmentType.Grass;
            }

            /**모래을 밟았을 경우...*/
            if (layerName.Contains("Sand") || layerName.Contains("New")){

                str = "Sand";
                return FModParamLabel.EnvironmentType.Sand;
            }

            if(layerName.Contains("Wood"))
            {
                str = "Wood";
                return FModParamLabel.EnvironmentType.Wood;
            }

            return -1;
            #endregion
        }

        private static float GetSFXTypeFromColorSamples(Color inputColor, out int index)
        {
            #region Omit
            Vector3 color = new Vector3(inputColor.r, inputColor.g, inputColor.b);

            int   Count  = _texColors.Length;
            float minDst = float.MaxValue;
            int   minIdx = 0;

            /**************************************
             *   가장 가까운 색상을 선택한다.....
             * ******/
            for( int i=0; i<Count; i++ ){

                float distance = (_texColors[i].Color - color).sqrMagnitude;

                /**완전히 일치할 경우...*/
                if(distance==0)
                {
                    index = i;
                    return _texColors[i].ParamValue;
                }
                else if (distance<minDst)
                {
                    minDst = distance;
                    minIdx = i;
                }
            }

            index = minIdx;
            return _texColors[minIdx].ParamValue;
            #endregion
        }

        private static IEnumerator FadeOutProcess(FadeOutType type, Image handler, float taktTime, float delayTime = 0f, int id = 0, Color color = default, float whiteAlpha = 0f, float darkAlpha = 1f, float startDelay = 0f, Color? goalColor = null)
        {
            #region Omit
            float start = ((int)type & 0b100) == 0 ? whiteAlpha : darkAlpha;
            float goal1 = ((int)type & 0b010) == 0 ? whiteAlpha : darkAlpha;
            float goal2 = ((int)type & 0b001) == 0 ? whiteAlpha : darkAlpha;

            if (taktTime < 0) yield break;

            //알파 초기화
            Color imgColor = handler.color;
            imgColor       = color;
            imgColor.a     = start;
            handler.color  = imgColor;

            //시작 딜레이
            while ((startDelay -= Time.unscaledDeltaTime) > 0) yield return null;

            float goalDiv = 1.0f / taktTime;
            float time = 0f;
            Color RGB = imgColor;

            //1->2
            while (time <= taktTime)
            {
                float process = time * goalDiv;
                imgColor.a = start + (goal1 - start) * process;
                if (goalColor != null)
                {
                    Color goalRGB = (Color)goalColor;
                    RGB.r = color.r + (goalRGB.r - color.r) * process;
                    RGB.g = color.g + (goalRGB.g - color.g) * process;
                    RGB.b = color.b + (goalRGB.b - color.b) * process;
                    imgColor = RGB;
                }

                handler.color = imgColor;
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            imgColor.a = goal1;
            handler.color = imgColor;

            OnFadeChange?.Invoke(imgColor.a == 1.0f, id);

            //중간 마무리 여부
            if (goal1 == goal2) yield break;

            //중간 딜레이
            while (delayTime > 0)
            {
                delayTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            //2->3
            time = 0f;

            while (time <= taktTime)
            {
                imgColor.a = goal1 + (goal2 - goal1) * (time * goalDiv);
                handler.color = imgColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            imgColor.a = goal2;
            handler.color = imgColor;

            OnFadeChange?.Invoke(imgColor.a == 1.0f, id);
            #endregion
        }
    }
}
