using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;
using static BossCrabSowingSeedsState;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   코코넛 크랩 보스가 구현된 컴포넌트입니다.
 * ***/
[RequireComponent(typeof(Animator))]
public sealed class BossCrab : Enemy
{
    #region Define
    public sealed class BossCrabAnimation
    {
        public const string Idle = "Idle";
        public const string Hit  = "Hit";
        public const string Die  = "die";

        public const string SpitSeedsReady   = "Attack1Ready";
        public const string SpittingSeeds    = "Attack1Spit";

        public const string EgoTongAttack_TongRise   = "Attack2TongsRise";
        public const string EgoTongAttack_RiseIdle   = "Attack2TongsRiseIdle";
        public const string EgoTongAttack_Down       = "Attack2TongsDown";

        public const string MakeSandWave_TongsRise = "Attack3TongsRise";
        public const string MakeSandWave_Smash     = "Attack3TongsSmash";

        public const string MakeAntHell_Ready     = "Attack4Ready";
    }
    #endregion

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
        private SerializedProperty SeedFlightTimeProperty;


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
        private SerializedProperty AntHellDurationProperty;

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

                SeedSpawnRangeProperty = serializedObject.FindProperty("_SeedSpawnRange");
            }

            if(SeedFlightTimeProperty==null){

                SeedFlightTimeProperty = serializedObject.FindProperty("_SeedFlightTime");
            }

            if(SeedCountProperty==null){

                SeedCountProperty = serializedObject.FindProperty("_SeedCount");
            }

            if(SeedExplodeTimeProperty==null){

                SeedExplodeTimeProperty = serializedObject.FindProperty("_SeedExplodeTime");
            }

            if(SeedExplodeRangeProperty==null) {

                SeedExplodeRangeProperty = serializedObject.FindProperty("_SeedExplodeRange");
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

                EgoAtkWaitTimeProperty = serializedObject.FindProperty("_EgoAtkWaitTime");
            }

            if(EgoAtkCompleteRateProperty==null){

                EgoAtkCompleteRateProperty = serializedObject.FindProperty("_EgoAtkCompleteRate");
            }

            if(EgoAtkRangeProperty==null){

                EgoAtkRangeProperty = serializedObject.FindProperty("_EgoAtkRange");
            }

            if(CrabHandProperty==null){

                CrabHandProperty = serializedObject.FindProperty("CrabHandPrefab");
            }

            if(SandFXPrefabProperty == null){

                SandFXPrefabProperty = serializedObject.FindProperty("SandWavePrefab");
            }

            if(SandMaxWaveProperty==null){

                SandMaxWaveProperty = serializedObject.FindProperty("_SandWaveCount");
            }

            if(AntHellPrefabProperty==null){

                AntHellPrefabProperty = serializedObject.FindProperty("AntHellPrefab");
            }

            if(AntHellDurationProperty==null){

                AntHellDurationProperty = serializedObject.FindProperty("_AntHellDuration");
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

                    if (value < 0) value = 0f;
                    EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**공격을 하는 시간 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Duration", EgoAtkCompleteRateProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = .01f;
                    EgoAtkCompleteRateProperty.floatValue = value;
                }
            }

            /**공격을 하기전 기다리는 시간 표시.*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Start Duration", EgoAtkWaitTimeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = .01f;
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

                    if (value < 0) value = 0f;
                    SeedSpawnRangeProperty.floatValue = value;
                }
            }

            /**씨앗 생성 개수 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Seed Spawn Count", SeedCountProperty.intValue);
                if (changeScope.changed){

                    if (value < 0) value = 1;
                    SeedCountProperty.intValue = value;
                }
            }

            /**씨앗이 날아가는데 걸리는 시간 표시.....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Flight Duration", SeedFlightTimeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
                    SeedFlightTimeProperty.floatValue = value;
                }
            }

            /**씨앗이 터지기까지 걸리는 시간 표시....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Duration", SeedExplodeTimeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
                    SeedExplodeTimeProperty.floatValue = value;
                }
            }

            /**씨앗의 폭발 반경 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Range", SeedExplodeRangeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
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
            if (AntHellPrefabProperty == null || AntHellDurationProperty==null) return;

            /**접힌 상태라면 내용을 표시하지 않는다...*/
            if (!(_AntHellFoldout = EditorGUILayout.Foldout(_AntHellFoldout, "AntHell Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**모래지옥 참조 필드 표시..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AntHell Prefab", AntHellPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    AntHellPrefabProperty.objectReferenceValue = value;
                }
            }

            /**모래지옥 지속시간 표시...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("AntHell Duration", AntHellDurationProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = .01f;
                    AntHellDurationProperty.floatValue = value;
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
            EditorGUILayout.PropertyField(SandFXPrefabProperty);

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
    public float SeedSpawnRange     { get { return _SeedSpawnRange; } set { _SeedSpawnRange = (value < 0f ? 0f : value); } }
    public float SeedExplodeTime    { get { return _SeedExplodeTime; } set { _SeedExplodeTime = (value < 0f ? 0f : value); } }
    public float SeedExplodeRange   { get { return _SeedExplodeTime; } set { _SeedExplodeTime = (value < 0f ? 0f : value); } }
    public float EgoAtkWaitTime     { get { return _EgoAtkWaitTime; } set { _EgoAtkWaitTime = (value < 0f ? 0f : value); } }
    public float EgoAtkCompleteRate { get { return _EgoAtkCompleteRate; } set { _EgoAtkCompleteRate = (value < 0f ? 0f : value); } }
    public float AntHellDuration    { get { return _AntHellDuration; } set { _AntHellDuration = (value < 0f ? 0f : value); } }
    public float EgoAtkRange        { get { return _EgoAtkRange; } set { _EgoAtkRange = (value < 0f ? 0f : value); } }
    public int   SandWaveCount      { get { return _SandWaveCount; } set { _SandWaveCount = (value < 0 ? 0 : value); } }
    public int   SeedCount          { get { return _SeedCount; } set { _SeedCount = (value < 0 ? 0 : value); } }

    /************************************
     *   씨앗 뱉는 패턴 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector, Min(0f)]
    private float        _SeedSpawnRange    = 3f;

    [SerializeField,HideInInspector, Min(0)] 
    private int          _SeedCount         = 5;

    [SerializeField,HideInInspector, Min(0f)] 
    private float       _SeedExplodeTime    = 5f;

    [SerializeField,HideInInspector, Min(0f)] 
    private float       _SeedExplodeRange   = 1.5f;

    [SerializeField, HideInInspector, Min(0f)]
    private float       _SeedFlightTime     = 3f;

    [SerializeField,HideInInspector] 
    public GameObject   SeedPrefab;

    [SerializeField,HideInInspector]
    public GameObject   MarkerPrefab;

    [SerializeField,HideInInspector]
    public Transform    SeedShotPosition;


    /************************************
     *   분신 집게 공격 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector, Min(0f)]
    private float        _EgoAtkWaitTime    = 1f;

    [SerializeField,HideInInspector, Min(0f)]
    private float        _EgoAtkCompleteRate = 1f;

    [SerializeField,HideInInspector, Min(0f)]
    private float        _EgoAtkRange        = 1f;

    [SerializeField,HideInInspector] 
    public GameObject   CrabHandPrefab;


    /************************************
     *   모래 파도 관련 프로퍼티...
     * ***/
    [SerializeField,HideInInspector] 
    public GameObject[]   SandWavePrefab;

    [SerializeField,HideInInspector, Min(0)]
    private int           _SandWaveCount;


    /************************************
     *   모래지옥 관련 프로퍼티...
     * ***/
    [SerializeField, HideInInspector] 
    public GameObject    AntHellPrefab;

    [SerializeField, HideInInspector]
    private float        _AntHellDuration;


    [SerializeField]
    public bool StartPatternOnAwake = false;

    [SerializeField]
    public bool StateTrigger = false;


    //==================================================
    ///////              Fields                   //////
    //==================================================
    private Animator         _HPAnim;
    private Stack<Transform> _HPstack;

    private float _stateTriggerDelay = 0f;



    //===============================================
    //////          Magic methods               /////
    //===============================================
    private void Awake()
    {
        #region Omit
        gameObject.tag = "Boss";

        InitHPUI();
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        if(StartPatternOnAwake) AiSM.CurrentState = AiSM.Pattern[0];
        #endregion
    }

    private void Update()
    {
        #region Omit
        if (AiSM.CurrentState!=null){

            if(_stateTriggerDelay>0f)
            {
                /**딜레이가 끝나면 StateTrigger를 발생시킨다...*/
                if((_stateTriggerDelay-=Time.deltaTime)<=0f){

                    StateTrigger = true;
                }
            }

            AiSM?.CurrentState.Update();
        }
        #endregion
    }



    //============================================
    ////////         Core methods           //////
    //============================================
    private void InitHPUI()
    {
        #region Omit
        /****************************************
         *   보스 UI가 있을 경우 초기화한다....
         * ***/
        GameObject BossCanvas = GameObject.Find("Boss_Canvas");
        if (BossCanvas != null)
        {
            Transform hp            = BossCanvas.transform.Find("HP");
            Transform area          = hp.Find("HPArea");
            Stack<Transform> HPbars = _HPstack = new Stack<Transform>();

            int Count = area.childCount;
            for (int i = 0; i < Count; i++){

                HPbars.Push(area.GetChild(i));
            }

            hp.gameObject.SetActive(false);
            _HPAnim = hp.GetComponent<Animator>();
        }
        #endregion
    }

    private void StateSetting()
    {
        #region Omit
        /**********************************************
         *   상태 초기화에 필요한 요소들을 초기화한다...
         * ***/
        EgoCrabHand newHand = null;
        if (CrabHandPrefab != null)
        {
            newHand = GameObject.Instantiate(CrabHandPrefab).GetComponent<EgoCrabHand>();
            newHand.AttackDuration = EgoAtkCompleteRate;
            newHand.AttackRange = EgoAtkRange;
            newHand.AttackReadyDuration = EgoAtkWaitTime;
        }

        BossCrabSowingSeedsDesc sowingSeedsDesc = new BossCrabSowingSeedsDesc()
        {
            SeedPrefab = SeedPrefab,
            MarkerPrefab = MarkerPrefab,
            changeTime = 7f,
            delayTime = 2f,
            count = SeedCount,
            flightTime = _SeedFlightTime,
            rad = SeedSpawnRange,
            shootPoint = SeedShotPosition
        };



        /*****************************************
         *   상태머신 및 패턴들을 초기화한다....
         * ***/
        AiSM = AIStateMachine.CreateFormGameObject(gameObject);
        AiIdle = new BossCrabIdleState(AiSM, IdleRate);
        AiWait = new BossNepenthesWaitState(AiSM, WaitRate);
        AiHit = new BossCrabHitState(AiSM, this);
        AiDie = new BossCrabDieState(AiSM);
        AiAttack = new BossCrabSowingSeedsState(AiSM, ref sowingSeedsDesc, this);
        AiAttack2 = new BossCrabEgoStampState(AiSM, newHand, this);
        AiAttack3 = new BossCrabSandWaveState(AiSM, this, SandWavePrefab);
        AiAttack4 = new BossCrabAntHellState(AiSM, AntHellPrefab, _AntHellDuration, this);

        #endregion
    }

    public override void Hit()
    {
        AiSM.ChangeState(AiHit);
    }



    //=====================================================
    ////////           Public methods                //////
    //=====================================================
    public void PopHPStack()
    {
        #region Omit
        if (_HPstack == null || _HPstack.Count==0) return;

        Animator hpAnim = _HPstack.Pop().GetComponent<Animator>();  
        if(hpAnim){

            hpAnim.SetTrigger("isDamaged");
        }
        #endregion
    }

    public void SetAnimTrigger(string triggerName)
    {
        #region Omit
        if (AiSM.Animator == null) return;

        AiSM.Animator.SetTrigger(triggerName);
        #endregion
    }

    public void SetStateTrigger(float delay=0f)
    {
        #region Omit
        if (delay>0f)
        {
            _stateTriggerDelay = delay;
            StateTrigger = false;
            return;
        }

        StateTrigger = true;
        #endregion
    }

    public void ExitCurrentState()
    {
        #region Omit
        if (AiSM != null && AiSM.CurrentState!=null)
        {
            AiSM.CurrentState.Exit();   
        }
        #endregion
    }

    public void CrabBossAweaking()
    {
        #region Omit
        if (AiSM.CurrentState == null){

            AiSM.CurrentState = AiSM.Pattern[0];
        }


        if(_HPAnim!=null)
        {
            _HPAnim.gameObject.SetActive(true);
            _HPAnim.speed = .8f;
            _HPAnim.Play("HP_FadeIn", 0, 0f);
        }
        #endregion
    }
}
