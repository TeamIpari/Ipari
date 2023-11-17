using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;
using static BossCrabSowingSeedsState;
using IPariUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   ���ڳ� ũ�� ������ ������ ������Ʈ�Դϴ�.
 * ***/
[RequireComponent(typeof(Animator))]
public sealed class BossCrab : Enemy
{
    #region Define
    public sealed class BossCrabAnimation
    {
        public const string Idle = "Idle";
        public const string Hit  = "Hit";
        public const string Die  = "Death_Copy";

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

        /**���� ��� ���� ����...*/
        private SerializedProperty SeedSpawnRangeProperty;
        private SerializedProperty SeedCountProperty;
        private SerializedProperty SeedExplodeTimeProperty;
        private SerializedProperty SeedExplodeRangeProperty;
        private SerializedProperty SeedPrefabProperty;
        private SerializedProperty SeedPositionProperty;
        private SerializedProperty SeedMarkerProperty;
        private SerializedProperty SeedFlightTimeProperty;


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
        private SerializedProperty AntHellDurationProperty;

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
            if (!(_EgoStampfoldout = EditorGUILayout.Foldout(_EgoStampfoldout, "MagicCrabHand Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���Թ� ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("MagicCrabHand Prefab", CrabHandProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    CrabHandProperty.objectReferenceValue = value;
                }
            }

            /**���� ���� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Range", EgoAtkRangeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
                    EgoAtkRangeProperty.floatValue = value;
                }
            }

            /**������ �ϴ� �ð� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Attack Duration", EgoAtkCompleteRateProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = .01f;
                    EgoAtkCompleteRateProperty.floatValue = value;
                }
            }

            /**������ �ϱ��� ��ٸ��� �ð� ǥ��.*/
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

                    if (value < 0) value = 0f;
                    SeedSpawnRangeProperty.floatValue = value;
                }
            }

            /**���� ���� ���� ǥ��...*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Seed Spawn Count", SeedCountProperty.intValue);
                if (changeScope.changed){

                    if (value < 0) value = 1;
                    SeedCountProperty.intValue = value;
                }
            }

            /**������ ���ư��µ� �ɸ��� �ð� ǥ��.....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Flight Duration", SeedFlightTimeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
                    SeedFlightTimeProperty.floatValue = value;
                }
            }

            /**������ ��������� �ɸ��� �ð� ǥ��....*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Seed Explode Duration", SeedExplodeTimeProperty.floatValue);
                if (changeScope.changed){

                    if (value < 0) value = 0f;
                    SeedExplodeTimeProperty.floatValue = value;
                }
            }

            /**������ ���� �ݰ� ǥ��...*/
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

            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_AntHellFoldout = EditorGUILayout.Foldout(_AntHellFoldout, "AntHell Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**������ ���� �ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AntHell Prefab", AntHellPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed){

                    AntHellPrefabProperty.objectReferenceValue = value;
                }
            }

            /**������ ���ӽð� ǥ��...*/
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


            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_SandWavefoldout = EditorGUILayout.Foldout(_SandWavefoldout, "SandWave Pattern", BoldFoldStyle))){

                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            /**���ĵ�����Ʈ ������ �����ʵ� ǥ��..*/
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
    public float    SeedSpawnRange              { get { return _SeedSpawnRange; } set { _SeedSpawnRange = (value < 0f ? 0f : value); } }
    public float    SeedExplodeTime             { get { return _SeedExplodeTime; } set { _SeedExplodeTime = (value < 0f ? 0f : value); } }
    public float    SeedExplodeRange            { get { return _SeedExplodeTime; } set { _SeedExplodeTime = (value < 0f ? 0f : value); } }
    public float    EgoAtkWaitTime              { get { return _EgoAtkWaitTime; } set { _EgoAtkWaitTime = (value < 0f ? 0f : value); } }
    public float    EgoAtkCompleteRate          { get { return _EgoAtkCompleteRate; } set { _EgoAtkCompleteRate = (value < 0f ? 0f : value); } }
    public float    AntHellDuration             { get { return _AntHellDuration; } set { _AntHellDuration = (value < 0f ? 0f : value); } }
    public float    EgoAtkRange                 { get { return _EgoAtkRange; } set { _EgoAtkRange = (value < 0f ? 0f : value); } }
    public int      SandWaveCount               { get { return _SandWaveCount; } set { _SandWaveCount = (value < 0 ? 0 : value); } }
    public int      SeedCount                   { get { return _SeedCount; } set { _SeedCount = (value < 0 ? 0 : value); } }
    public int      CurrentSuperArmorCount      { get { return _CurrentSuperArmorCount; } set { _CurrentSuperArmorCount = (value < 0 ? 0 : value); } }
    public int      GetSuperArmorGainCount      { get { return _GetSuperArmorCountOnDamage; } set { _GetSuperArmorCountOnDamage = (value < 0 ? 0 : value); } }
    public Collider Collider                    { get { return _collider; } }

    /************************************
     *   ���� ��� ���� ���� ������Ƽ...
     * ***/
    [SerializeField, Min(0f)]
    private int          _GetSuperArmorCountOnDamage = 3;

    [SerializeField, Min(0f)]
    private int          _CurrentSuperArmorCount = 0;

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
     *   �н� ���� ���� ���� ������Ƽ...
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
     *   �� �ĵ� ���� ������Ƽ...
     * ***/
    [SerializeField,HideInInspector] 
    public GameObject[]   SandWavePrefab;

    [SerializeField,HideInInspector, Min(0)]
    private int           _SandWaveCount;


    /************************************
     *   ������ ���� ������Ƽ...
     * ***/
    [SerializeField, HideInInspector] 
    public GameObject    AntHellPrefab;

    [SerializeField, HideInInspector]
    private float        _AntHellDuration;


    /*************************************
     *    ���� SFX ���� ������Ƽ...
     * ****/
    [SerializeField]
    public GameObject DeathSFXPrefab;

    [SerializeField]
    public GameObject AttackSFXPrefab;

    [SerializeField]
    public Material   BodyDissolveMat;

    [SerializeField]
    public Material   HandDissolveMat;

    [SerializeField]
    public WhaleHorn  DropableHorn;


    /***************************************
     *    ���� ����.....
     * ******/
    [SerializeField]
    public bool StartPatternOnAwake = false;

    [SerializeField]
    public bool StateTrigger = false;


    //==================================================
    ///////              Fields                   //////
    //==================================================
    private Collider         _collider;
    private Animator         _HPAnim;
    private Stack<Transform> _HPstack;

    /**���� ����....*/
    private float _stateTriggerDelay = 0f;

    /**��Ʈ��ž ����....*/
    private float _hitStopDuration   = 0f;
    private float _lastAnimSpeed     = 0f;


    //===============================================
    //////          Magic methods               /////
    //===============================================
    private void Awake()
    {
        #region Omit
        gameObject.tag = "Boss";
        _collider = GetComponent<Collider>();

        InitHPUI();
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        if(StartPatternOnAwake) AiSM.CurrentState = AiSM.Pattern[0];
        #endregion
    }

    private void Start()
    {
        FModAudioManager.PlayBGM(FModBGMEventType.Chapter4BGM);
    }

    private void Update()
    {
        #region Omit
        float deltaTime = Time.deltaTime;

        /****************************************
         *    ��Ʈ��ž�� ó���Ѵ�....
         * ******/
        if(_hitStopDuration>0f){

            /**��Ʈ��ž�� �������Ǿ��� ���....*/
            if((_hitStopDuration -= deltaTime)<=0f)
            {
                AiSM.Animator.speed = _lastAnimSpeed;
            }
        }


        /*******************************************
         *   ���� Ʈ���Ÿ� ó���Ѵ�....
         * *****/
        else if (AiSM.CurrentState!=null){

            AIState lastState = AiSM.CurrentState;
            if(_stateTriggerDelay>0f)
            {
                /**�����̰� ������ StateTrigger�� �߻���Ų��...*/
                if((_stateTriggerDelay-= deltaTime) <=0f){

                    StateTrigger = true;
                }

            }

            AiSM?.CurrentState.Update();

            /**������ ��ȭ�� ���� ���, ���۾Ƹ� ī��Ʈ�� ���δ�...*/
            if(lastState!=AiSM.CurrentState)
            {
                CurrentSuperArmorCount--;
            }

        }
        #endregion
    }

    private void OnDestroy()
    {
        FModAudioManager.StopBGM();
    }



    //============================================
    ////////         Core methods           //////
    //============================================
    private void InitHPUI()
    {
        #region Omit
        /****************************************
         *   ���� UI�� ���� ��� �ʱ�ȭ�Ѵ�....
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
         *   ���� �ʱ�ȭ�� �ʿ��� ��ҵ��� �ʱ�ȭ�Ѵ�...
         * ***/
        MagicCrabHand newHand = null;
        if (CrabHandPrefab != null)
        {
            newHand = GameObject.Instantiate(CrabHandPrefab).GetComponent<MagicCrabHand>();
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
         *   ���¸ӽ� �� ���ϵ��� �ʱ�ȭ�Ѵ�....
         * ***/
        AiSM = AIStateMachine.CreateFormGameObject(gameObject);
        AiIdle = new BossCrabIdleState(AiSM, IdleRate);
        AiWait = new BossNepenthesWaitState(AiSM, WaitRate);
        AiHit = new BossCrabHitState(AiSM, this);
        AiDie = new BossCrabDieState(AiSM);
        AiAttack = new BossCrabSowingSeedsState(AiSM, ref sowingSeedsDesc, this);
        AiAttack2 = new BossCrabMagicCrabHandState(AiSM, newHand, this);
        AiAttack3 = new BossCrabSandWaveState(AiSM, this, SandWavePrefab);
        AiAttack4 = new BossCrabAntHellState(AiSM, AntHellPrefab, _AntHellDuration, this);

        #endregion
    }

    public override void Hit()
    {
        #region Omit
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_BoomBurst);
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_Hit, transform.position, 3f);
        IpariUtility.PlayGamePadVibration(1f, 1f, .08f);

        PopHPUIStack();
        HP -= 10;

        /**������ �װų�, ���۾ƸӰ� �ƴ� ���� ������ �Դ´�...*/
        if(HP>0 && _CurrentSuperArmorCount > 0)
        {
            _hitStopDuration    = .27f;
            _lastAnimSpeed      = AiSM.Animator.speed;
            AiSM.Animator.speed = 0f;
            CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .6f, .022f);
        }
        else AiSM.ChangeState(AiHit);

        #endregion
    }



    //=====================================================
    ////////           Public methods                //////
    //=====================================================
    public void PopHPUIStack()
    {
        #region Omit
        if (_HPstack == null || _HPstack.Count==0) return;

        Animator hpAnim = _HPstack.Pop().GetComponent<Animator>();  
        if(hpAnim){

            hpAnim.SetTrigger("isDamaged");
        }
        #endregion
    }

    public void ShowHPUI(bool isShow, float speed=1f)
    {
        #region
        if (_HPAnim == null) return;

        _HPAnim.speed = speed;
        if (isShow)
        {
            _HPAnim.Play("HP_FadeIn");
            return;
        }

        _HPAnim.Play("HP_FadeOut");
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
            StateTrigger       = false;
            return;
        }

        StateTrigger = true;
        #endregion
    }

    public void ClearStateTriggerDelay()
    {
        _stateTriggerDelay = 0f;
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

            FModAudioManager.UsedBGMAutoFade     = true;
            FModAudioManager.BGMAutoFadeDuration = 3f;

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
