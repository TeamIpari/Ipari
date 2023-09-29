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
 *   ���ڳ� ũ�� ������ ������ ������Ʈ�Դϴ�.
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

        /**���� ��� ���� ����...*/
        private SerializedProperty SeedSpawnRangeProperty;
        private SerializedProperty SeedCountProperty;
        private SerializedProperty SeedExplodeTimeProperty;
        private SerializedProperty SeedExplodeRangeProperty;
        private SerializedProperty SeedPrefabProperty;
        private SerializedProperty SeedPositionProperty;
        private SerializedProperty SeedMarkerProperty;


        /**�н� ��� ���� ����...*/
        private SerializedProperty EgoAtkWaitTimeProperty;
        private SerializedProperty EgoAtkCompleteRateProperty;
        private SerializedProperty EgoAtkRangeProperty;
        private SerializedProperty CrabHandProperty;

        /**���ĵ� ���� ����...*/
        private SerializedProperty SandFXPrefabProperty;
        private SerializedProperty SandMaxWaveProperty;

        /**�������� ���� ����*/
        private SerializedProperty AntHellPrefabProperty;

        /**��Ÿ�� ����...*/
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
             *   ��� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            serializedObject.Update();

            GUI_Intialized();

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Pattern settings", BoldLabelStyle);

            GUI_ShowShotSeedState();

            GUI_ShowSandWaveState();

            GUI_ShowEgoStampState();

            GUI_ShowAntHellState();

            /**��������� �ִٸ� ���� �����Ѵ�...*/
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
             *    ��� ������Ƽ���� �ʱ�ȭ�Ѵ�...
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
             *    ��� ��Ÿ�ϵ��� �ʱ�ȭ�Ѵ�...
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

            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_EgoStampfoldout = EditorGUILayout.Foldout(_EgoStampfoldout, "EgoStamp Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���Թ� ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("CrabHandPrefab", CrabHandProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    CrabHandProperty.objectReferenceValue = value;
                }
            }

            /**���� ���� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Range", EgoAtkRangeProperty.floatValue);
                if (changeScope.changed){

                   EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**������ �ϴ� �ð� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Duration", EgoAtkCompleteRateProperty.floatValue);
                if (changeScope.changed){

                    EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**������ �ϱ��� ��ٸ��� �ð� ǥ��.*/
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


            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_SeedAttackfoldout = EditorGUILayout.Foldout(_SeedAttackfoldout, "Shot Seed MTE Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���� ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Seed Prefab", SeedPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SeedPrefabProperty.objectReferenceValue = value;
                }
            }

            /**���� ��Ŀ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("Marker Prefab", SeedMarkerProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SeedMarkerProperty.objectReferenceValue = value;
                }
            }

            /**���� �߻� ��ġ ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Transform value = (Transform)EditorGUILayout.ObjectField("Shot Position", SeedPositionProperty.objectReferenceValue, typeof(Transform), true);
                if (changeScope.changed){

                    SeedPositionProperty.objectReferenceValue = value;
                }
            }

            /**���� ���� ���� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Spawn Range", SeedSpawnRangeProperty.floatValue);
                if (changeScope.changed) {

                    SeedSpawnRangeProperty.floatValue = value;
                }
            }

            /**���� ���� ���� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Seed Spawn Count", SeedCountProperty.intValue);
                if (changeScope.changed){

                    SeedCountProperty.intValue = value;
                }
            }

            /**������ ��������� �ɸ��� �ð� ǥ��....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Duration", SeedExplodeTimeProperty.floatValue);
                if (changeScope.changed){

                    SeedExplodeTimeProperty.floatValue = value;
                }
            }

            /**������ ���� �ݰ� ǥ��...*/
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

            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_AntHellFoldout = EditorGUILayout.Foldout(_AntHellFoldout, "AntHell Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���� ������ �����ʵ� ǥ��..*/
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


            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_SandWavefoldout = EditorGUILayout.Foldout(_SandWavefoldout, "SandWave Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���ĵ�����Ʈ ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("SandWaveFX Prefab", SandFXPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    SandFXPrefabProperty.objectReferenceValue = value;
                }
            }

            /**���ĵ� ����Ű�� Ƚ�� ǥ��...*/
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
     *   ���� ��� ���� ���� ������Ƽ...
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
     *   �н� ���� ���� ���� ������Ƽ...
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
     *   �� �ĵ� ���� ������Ƽ...
     * ***/
    [SerializeField,HideInInspector] 
    public GameObject     SandFXPrefab;

    [SerializeField,HideInInspector] 
    public int           SandWaveCount;


    /************************************
     *   ������ ���� ������Ƽ...
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

        /**���¸ӽ��� �ʱ�ȭ�Ѵ�...*/
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
