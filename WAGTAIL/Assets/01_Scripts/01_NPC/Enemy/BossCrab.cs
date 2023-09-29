using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using FMODUnity;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   코코넛 크랩 보스가 구현된 컴포넌트입니다.
 * ***/
[RequireComponent(typeof(Animator))]
public sealed class BossCrab : Enemy
{
    #region Editor_Extension
#if UNITY_EDITOR
    [CustomEditor(typeof(BossCrab))]
    private sealed class BossCrabEditor : Editor
    {
        //===================================
        //////          Fields          /////
        //===================================

        /**씨앗 뱉기 패턴 관련...*/
        private SerializedProperty SeedSpawnRangeProperty;
        private SerializedProperty SeedCountProperty;
        private SerializedProperty SeedExplodeTimeProperty;
        private SerializedProperty SeedExplodeRangeProperty;
        private SerializedProperty SeedPrefabProperty;
        private SerializedProperty SeedPositionProperty;
        private SerializedProperty SeedMarkerProperty;


        /**분신 찍기 패턴 관련...*/
        private SerializedProperty EgoAtkWaitTimeProperty;
        private SerializedProperty EgoAtkCompleteRateProperty;
        private SerializedProperty EgoAtkRangeProperty;
        private SerializedProperty CrabHandProperty;

        /**모래파도 패턴 관련...*/
        private SerializedProperty SandFXPrefabProperty;
        private SerializedProperty SandMaxWaveProperty;

        /**개미지옥 패턴 관련*/
        private SerializedProperty AntHellPrefabProperty;

        /**스타일 관련...*/
        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        private static bool _EgoStampfoldout   = false;
        private static bool _SandWavefoldout   = false;
        private static bool _AntHellFoldout    = false;
        private static bool _SeedAttackfoldout = false;


        //==============================================
        //////         Override methods           //////
        //==============================================
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            /***********************************
             *   모든 프로퍼티들을 표시한다...
             * ***/
            serializedObject.Update();

            GUI_Intialized();

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Pattern settings", BoldLabelStyle);

            GUI_ShowShotSeedState();

            GUI_ShowSandWaveState();

            GUI_ShowEgoStampState();

            GUI_ShowAntHellState();

            /**변경사항이 있다면 값을 갱신한다...*/
            if (GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
        }



        //========================================
        //////         GUI methods            ////
        //========================================
        private void GUI_Intialized()
        {
            #region Omit
            /**************************************
             *    모든 프로퍼티들을 초기화한다...
             * ***/
            if(SeedSpawnRangeProperty==null){

                SeedSpawnRangeProperty = serializedObject.FindProperty("SeedSpawnRange");
            }

            if(SeedCountProperty==null){

                SeedCountProperty = serializedObject.FindProperty("SeedCount");
            }

            if(SeedExplodeTimeProperty==null){

                SeedExplodeTimeProperty = serializedObject.FindProperty("SeedExplodeTime");
            }

            if(SeedExplodeRangeProperty==null) {

                SeedExplodeRangeProperty = serializedObject.FindProperty("SeedExplodeRange");
            }

            if(SeedPrefabProperty==null){

                SeedPrefabProperty = serializedObject.FindProperty("SeedPrefab");
            }

            if(SeedMarkerProperty==null){

                SeedMarkerProperty = serializedObject.FindProperty("MarkerPrefab");
            }

            if(SeedPositionProperty==null)
            {
                SeedPositionProperty = serializedObject.FindProperty("SeedShotPosition");
            }

            if(EgoAtkWaitTimeProperty==null){

                EgoAtkWaitTimeProperty = serializedObject.FindProperty("EgoAtkWaitTime");
            }

            if(EgoAtkCompleteRateProperty==null){

                EgoAtkCompleteRateProperty = serializedObject.FindProperty("EgoAtkCompleteRate");
            }

            if(EgoAtkRangeProperty==null){

                EgoAtkRangeProperty = serializedObject.FindProperty("EgoAtkRange");
            }

            if(CrabHandProperty==null){

                CrabHandProperty = serializedObject.FindProperty("CrabHandPrefab");
            }

            if(SandFXPrefabProperty == null){

                SandFXPrefabProperty = serializedObject.FindProperty("SandFXPrefab");
            }

            if(SandMaxWaveProperty==null){

                SandMaxWaveProperty = serializedObject.FindProperty("SandWaveCount");
            }

            if(AntHellPrefabProperty==null){

                AntHellPrefabProperty = serializedObject.FindProperty("AntHellPrefab");
            }


            /*****************************************
             *    모든 스타일들을 초기화한다...
             * ***/
            if(BoldLabelStyle==null)
            {
                BoldLabelStyle = new GUIStyle(GUI.skin.label);
                BoldLabelStyle.fontStyle= FontStyle.Bold;
                BoldLabelStyle.normal.textColor = EditorGUIUtility.isProSkin? Color.white:Color.black;
                BoldLabelStyle.fontSize = 14;
            }

            if(BoldFoldStyle==null)
            {
                BoldFoldStyle = new GUIStyle(EditorStyles.foldout);
                BoldFoldStyle.fontStyle= FontStyle.Bold;    
            }

            #endregion
        }

        private void GUI_ShowEgoStampState()
        {
            #region Omit
            if (EgoAtkRangeProperty == null || EgoAtkCompleteRateProperty == null || EgoAtkWaitTimeProperty == null || CrabHandProperty==null) 
                return;

            /**접힌 상태라면 내용을 표시하지 않는다...*/
            if (!(_EgoStampfoldout = EditorGUILayout.Foldout(_EgoStampfoldout, "EgoStamp Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**집게발 프래팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("CrabHandPrefab", CrabHandProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    CrabHandProperty.objectReferenceValue = value;
                }
            }

            /**공격 범위 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Range", EgoAtkRangeProperty.floatValue);
                if (changeScope.changed){

                   EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**공격을 하는 시간 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Duration", EgoAtkCompleteRateProperty.floatValue);
                if (changeScope.changed){

                    EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**공격을 하기전 기다리는 시간 표시.*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Start Duration", EgoAtkWaitTimeProperty.floatValue);
                if (changeScope.changed){

                    EgoAtkWaitTimeProperty.floatValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
            #endregion
        }

        private void GUI_ShowShotSeedState()
        {
            #region Omit
            if (SeedSpawnRangeProperty == null  || SeedCountProperty == null || SeedPositionProperty==null ||
                SeedExplodeTimeProperty == null || SeedExplodeRangeProperty == null || SeedPrefabProperty==null || SeedMarkerProperty==null)
                return;


            /**접힌 상태라면 내용을 표시하지 않는다...*/
            if (!(_SeedAttackfoldout = EditorGUILayout.Foldout(_SeedAttackfoldout, "Shot Seed MTE Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**씨앗 프래팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Seed Prefab", SeedPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SeedPrefabProperty.objectReferenceValue = value;
                }
            }

            /**씨앗 마커 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Marker Prefab", SeedMarkerProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SeedMarkerProperty.objectReferenceValue = value;
                }
            }

            /**씨앗 발사 위치 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Transform value = (Transform)EditorGUILayout.ObjectField("Shot Position", SeedPositionProperty.objectReferenceValue, typeof(Transform), true);
                if (changeScope.changed){

                    SeedPositionProperty.objectReferenceValue = value;
                }
            }

            /**씨앗 생성 범위 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Spawn Range", SeedSpawnRangeProperty.floatValue);
                if (changeScope.changed) {

                    SeedSpawnRangeProperty.floatValue = value;
                }
            }

            /**씨앗 생성 개수 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Seed Spawn Count", SeedCountProperty.intValue);
                if (changeScope.changed){

                    SeedCountProperty.intValue = value;
                }
            }

            /**씨앗이 터지기까지 걸리는 시간 표시....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Duration", SeedExplodeTimeProperty.floatValue);
                if (changeScope.changed){

                    SeedExplodeTimeProperty.floatValue = value;
                }
            }

            /**씨앗의 폭발 반경 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Range", SeedExplodeRangeProperty.floatValue);
                if (changeScope.changed){

                    SeedExplodeRangeProperty.floatValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
            #endregion
        }

        private void GUI_ShowAntHellState()
        {
            #region Omit
            if (AntHellPrefabProperty == null) return;

            /**접힌 상태라면 내용을 표시하지 않는다...*/
            if (!(_AntHellFoldout = EditorGUILayout.Foldout(_AntHellFoldout, "AntHell Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**씨앗 프래팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AntHell Prefab", AntHellPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    AntHellPrefabProperty.objectReferenceValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
            #endregion
        }

        private void GUI_ShowSandWaveState()
        {
            #region Omit
            if (SandFXPrefabProperty == null || SandMaxWaveProperty == null) return;


            /**접힌 상태라면 내용을 표시하지 않는다...*/
            if (!(_SandWavefoldout = EditorGUILayout.Foldout(_SandWavefoldout, "SandWave Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**모래파도이펙트 프래팹 참조필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("SandWaveFX Prefab", SandFXPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SandFXPrefabProperty.objectReferenceValue = value;
                }
            }

            /**모래파도 일으키는 횟수 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("SandWave Count", SandMaxWaveProperty.intValue);
                if (changeScope.changed){

                    SandMaxWaveProperty.intValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
            #endregion
        }

        private void GUI_DrawLine(float space = 5f, float subOffset = 0f)
        {
            #region Omit
            EditorGUILayout.Space(15f);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15 + subOffset, rect.y), new Vector2(rect.width + 15 - subOffset * 2, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f);
            #endregion
        }



    }
#endif
    #endregion

    //===========================================
    //////          Property                /////
    //===========================================
    
    /************************************
     *   씨앗 뱉는 패턴 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector]
    public float        SeedSpawnRange;

    [SerializeField,HideInInspector] 
    public int          SeedCount;

    [SerializeField,HideInInspector] 
    public float        SeedExplodeTime;

    [SerializeField,HideInInspector] 
    public float        SeedExplodeRange;

    [SerializeField,HideInInspector] 
    public GameObject   SeedPrefab;

    [SerializeField,HideInInspector]
    public GameObject   MarkerPrefab;

    [SerializeField,HideInInspector]
    public Transform    SeedShotPosition;


    /************************************
     *   분신 집게 공격 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector] 
    public float        EgoAtkWaitTime;

    [SerializeField,HideInInspector] 
    public float        EgoAtkCompleteRate;

    [SerializeField,HideInInspector] 
    public float        EgoAtkRange;

    [SerializeField,HideInInspector] 
    public GameObject   CrabHandPrefab;


    /************************************
     *   모래 파도 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector] 
    public GameObject     SandFXPrefab;

    [SerializeField,HideInInspector] 
    public int           SandWaveCount;


    /************************************
     *   모래지옥 관련 프로퍼티...
     * ***/
    [SerializeField, HideInInspector] 
    private GameObject    AntHellPrefab;


    //==========================================
    //////              Fields              ////
    //==========================================




    //===============================================
    //////          Magic methods               /////
    //===============================================
    private void Awake()
    {
        #region Omit
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        AiSM.CurrentState = AiSM.Pattern[0];
        #endregion
    }

    private void Update()
    {
       AiSM?.CurrentState.Update();
    }



    //============================================
    ////////         Core methods           //////
    //============================================
    private void StateSetting()
    {
        #region Omit
        BossNepenthesProfile profile = new BossNepenthesProfile
        {
            BulletPrefab = SeedPrefab,
            ShotMarker   = MarkerPrefab,
            ShotPosition = SeedShotPosition
        };

        /**상태머신을 초기화한다...*/
        AiSM      = AIStateMachine.CreateFormGameObject(gameObject);
        AiIdle    = new BossCrabIdleState(AiSM, IdleRate);
        AiDie     = new BossCrabDieState(AiSM);
        AiAttack  = new BossNepenthesSmallShotGun(AiSM, profile, 2f, 5, 3f);

        if(CrabHandPrefab!=null){

            EgoCrabHand newHand = GameObject.Instantiate(CrabHandPrefab).GetComponent<EgoCrabHand>();
            newHand.gameObject.SetActive(false);

            AiAttack2 = new BossCrabEgoStampState(AiSM, newHand);
        }

        AiAttack3 = new BossCrabSandWaveState(AiSM);
        AiAttack4 = new BossCrabAntHellState(AiSM, AntHellPrefab);


        #endregion
    }



}
