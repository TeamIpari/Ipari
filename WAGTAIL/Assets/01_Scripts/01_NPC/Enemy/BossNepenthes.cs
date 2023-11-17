using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct BossNepenthesProfile
{
    public GameObject BulletPrefab;
    public Transform ShotPosition;
    public GameObject ShotMarker;

    public void SetProfile(GameObject Bullet, Transform point, GameObject ShotMarker)
    {
        this.BulletPrefab = Bullet;
        this.ShotPosition = point;
        this.ShotMarker = ShotMarker;
    }
}

/************************************************
 * �߰����� ���浥���� ������ ������Ʈ.
 * ***/

[RequireComponent(typeof(Animator))]
public sealed class BossNepenthes : Enemy
{
    #region Define
    public sealed class BossNepenthesAnim
    {
        public const string Idle = "Idle";
        public const string Hit = "Hit";
        public const string Die = "die";

        //public const string SpitSeed
    }

    #endregion

    #region Editor_Extension
#if UNITY_EDITOR

    [CustomEditor(typeof(BossNepenthes))]
    private sealed class BossNepenthesEditor : Editor
    {
        //===================================
        //////          Fields          /////
        //===================================

        /* ���� ä�� ��� */
        //========================================
        //////          GameObject          /////
        //========================================
        private SerializedProperty LeftVineProperty;
        private SerializedProperty RightVineProperty;
        private SerializedProperty FXVineAttackProperty;
        private SerializedProperty BombObjectProperty;


        /*���� ��ü ���� ����(�ѹ� ��� ��� ����)*/
        //========================================
        //////          GameObject          /////
        //========================================
        private SerializedProperty ShootPointProperty;       
        private SerializedProperty AcidBombPrefabProperty;
        private SerializedProperty AcidBombSmallMarkerProperty;
        private SerializedProperty AcidBombBigMarkerProperty;
        //========================================
        //////          data value          /////
        //========================================
        private SerializedProperty FlightTimeProperty;
        private SerializedProperty MaximumSizeProperty;
        private SerializedProperty MinimumSizeProperty;

        /* ������ ��� ��� */
        //========================================
        //////          data value          /////
        //========================================
        private SerializedProperty ShootCountProperty;
        private SerializedProperty ShootAreaProperty;

        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        /* ��Ÿ�� ���� ���� */
        private static bool _AcidBombSettingFoldOut = false;
        private static bool _VineAttackFoldOut = false;
        private static bool _OneShotAcidBombFoldOut = false;
        private static bool _ShotGunAcidBombFoldOut = false;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            /*****************************************
             * ��� ������Ƽ�� �ν����Ϳ� ǥ��
             * ***/
            serializedObject.Update();

            GUI_Initalized();
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Pattern settings", BoldLabelStyle);

            GUI_DefaultAcidBomb();
            GUI_ShowVineAttackState();
            GUI_OneShotSetting();
            GUI_ShotGunState();

            /**��������� �ִٸ� ���� �����Ѵ�...*/
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        //========================================
        //////         GUI methods            ////
        //========================================
        private void GUI_Initalized()
        {
            #region Omit
            /***************************************
             * ��� ������Ƽ�� �ʱ�ȭ �Ѵ�.
             * **/
            if(LeftVineProperty == null)
            {
                LeftVineProperty = serializedObject.FindProperty("_LeftVinePrefab");
            }

            if(RightVineProperty == null)
            {
                RightVineProperty = serializedObject.FindProperty("_RightVinePrefab");
            }

            if(FXVineAttackProperty == null)
            {
                FXVineAttackProperty = serializedObject.FindProperty("_FXVineAttackPrefab");
            }
            
            if(BombObjectProperty == null)
            {
                BombObjectProperty = serializedObject.FindProperty("_BombObjectPrefab");
            }

            if(ShootPointProperty == null)
            {
                ShootPointProperty = serializedObject.FindProperty("_ShootPoint");
            }

            if(AcidBombPrefabProperty == null)
            {
                AcidBombPrefabProperty = serializedObject.FindProperty("_AcidBombPrefab");
            }

            if(AcidBombSmallMarkerProperty == null)
            {
                AcidBombSmallMarkerProperty = serializedObject.FindProperty("_AcidBombSmallMarker");
            }

            if(AcidBombBigMarkerProperty == null)
            {
                AcidBombBigMarkerProperty = serializedObject.FindProperty("_AcidBombBigMarker");
            }

            if(FlightTimeProperty == null)
            {
                FlightTimeProperty = serializedObject.FindProperty("_FlightTime");
            }

            if(MaximumSizeProperty == null)
            {
                MaximumSizeProperty = serializedObject.FindProperty("_MaximumSize");
            }

            if(MinimumSizeProperty == null)
            {
                MinimumSizeProperty = serializedObject.FindProperty("_MinimumSize");
            }

            if(ShootCountProperty == null)
            {
                ShootCountProperty = serializedObject.FindProperty("_ShootCount");
            }

            if (ShootAreaProperty == null)
            {
                ShootAreaProperty = serializedObject.FindProperty("_ShootArea");
            }

            /******************************************
             * ��� ��Ÿ���� �ʱ�ȭ
             * ***/
            if(BoldLabelStyle == null)
            {
                BoldLabelStyle = new GUIStyle(GUI.skin.label);
                BoldLabelStyle.fontStyle = FontStyle.Bold;
                BoldLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                BoldLabelStyle.fontSize = 14;
            }

            if(BoldFoldStyle == null)
            {
                BoldFoldStyle = new GUIStyle(EditorStyles.foldout);
                BoldFoldStyle.fontStyle = FontStyle.Bold;
            }
            #endregion
        }

        private void GUI_ShowVineAttackState()
        {
            #region Omit
            if (LeftVineProperty == null || RightVineProperty == null)
                return;
            if(!(_VineAttackFoldOut = EditorGUILayout.Foldout(_VineAttackFoldOut, "VIneAttack Pattern", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("FXVineAttack", FXVineAttackProperty.objectReferenceValue, typeof(GameObject), true);
                if(changeScope.changed)
                {
                    FXVineAttackProperty.objectReferenceValue = value;
                }
            }

            /* ������ ���� ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("RightVine", RightVineProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    RightVineProperty.objectReferenceValue = value;
                }
            }

            /* ���� ���� ������ �����ʵ� ǥ��..*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("LeftVine", LeftVineProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    LeftVineProperty.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("BombObject", BombObjectProperty.objectReferenceValue, typeof(GameObject), true);
                if(changeScope.changed)
                {
                    BombObjectProperty.objectReferenceValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);
            #endregion
        }

        private void GUI_DefaultAcidBomb()
        {
            #region Omit
            if (AcidBombPrefabProperty == null || AcidBombSmallMarkerProperty == null || ShootPointProperty == null)
                return;

            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_AcidBombSettingFoldOut = EditorGUILayout.Foldout(_AcidBombSettingFoldOut, "AcidBomb Setting", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }
            EditorGUI.indentLevel++;

            /* ���� ������Ʈ ���� */
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("AcidBombPrefab", AcidBombPrefabProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AcidBombPrefabProperty.objectReferenceValue = value;
                }
            }

            /* ���� �� ��Ŀ ���� */
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("SmallMarkerPrefab", AcidBombSmallMarkerProperty.objectReferenceValue, typeof(GameObject), true);
                if(changeScope .changed)
                {
                    AcidBombSmallMarkerProperty.objectReferenceValue = value;
                }
            }

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                GameObject value = (GameObject)EditorGUILayout.ObjectField("BigMarkerPrefab", AcidBombBigMarkerProperty.objectReferenceValue, typeof(GameObject), true);
                if (changeScope.changed)
                {
                    AcidBombBigMarkerProperty.objectReferenceValue = value;
                }
            }

            /* �߻� ���� ǥ�� */
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                Transform value = (Transform)EditorGUILayout.ObjectField("ShootPoint", ShootPointProperty.objectReferenceValue, typeof(Transform), true);
                if(changeScope.changed)
                {
                    ShootPointProperty.objectReferenceValue = value;
                }
            }

            /* ü�� �ð� ǥ��*/
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = (float)EditorGUILayout.FloatField("FlightTime", FlightTimeProperty.floatValue);
                if (changeScope.changed)
                {
                    FlightTimeProperty.floatValue = value;
                }
            }

            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);

            #endregion
        }

        private void GUI_ShotGunState()
        {
            #region Omit
            if (!(_ShotGunAcidBombFoldOut = EditorGUILayout.Foldout(_ShotGunAcidBombFoldOut, "ShotGunAcidBombState", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }

            EditorGUI.indentLevel++;
            /* ������ ���� */
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Shoot Count", ShootCountProperty.intValue);

                if (changeScope.changed)
                {
                    if (value < 0) value = 0;
                    ShootCountProperty.intValue = value;
                }
            }

            /* ���� ���� ǥ��  */
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                int value = EditorGUILayout.IntField("Attack Area", ShootAreaProperty.intValue);
                if (changeScope.changed)
                {
                    if (value < 0) value = 0;
                    ShootAreaProperty.intValue = value;
                }
            }
            EditorGUI.indentLevel--;
            GUI_DrawLine(5f, 20f);

            #endregion
        }

        private void GUI_OneShotSetting()
        {
            #region Omit
            /**���� ���¶�� ������ ǥ������ �ʴ´�...*/
            if (!(_OneShotAcidBombFoldOut = EditorGUILayout.Foldout(_OneShotAcidBombFoldOut, "OneShotAcidBombState", BoldFoldStyle)))
            {
                GUI_DrawLine(5f, 20f);
                return;
            }
            EditorGUI.indentLevel++;

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = (float)EditorGUILayout.FloatField("MaximumSize", MaximumSizeProperty.floatValue);
                if(changeScope.changed)
                {
                    if (value < 0) value = 0;
                    MaximumSizeProperty.floatValue = value;
                }
            }
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                float value = (float)EditorGUILayout.FloatField("MinimumSize", MinimumSizeProperty.floatValue);
                if (changeScope.changed)
                {
                    if (value < 0) value = 0;
                    MinimumSizeProperty.floatValue = value;
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
    public float FlightTime { get { return _FlightTime; } set { _FlightTime = (value < 0f ? 0f : value); } }
    public float MaxSize { get { return _MaximumSize; } set { _MaximumSize = (value < 0f ? 0f : value); } }
    public float MinSize { get { return _MinimumSize; } set { _MinimumSize = (value < 0f ? 0f : value); } }
    public int ShootCount { get { return _ShootCount; } set { _ShootCount = (value < 0 ? 0 : value); } }
    public int ShootArea { get { return _ShootArea; } set{_ShootArea  = (value < 0 ? 0 : value); }}

    /*********************************************
     * ���� ä�� ���� ������Ƽ...
     * ***/
    [SerializeField, HideInInspector, Min(0f)]
    private float _FlightTime = 2f;

    [SerializeField, HideInInspector, Min(0f)]
    private float _MaximumSize = 3f;

    [SerializeField, HideInInspector, Min(0f)]
    private float _MinimumSize = 1f;

    [SerializeField, HideInInspector]
    public GameObject _LeftVinePrefab;

    [SerializeField, HideInInspector]
    public GameObject _RightVinePrefab;

    [SerializeField, HideInInspector]
    public GameObject _FXVineAttackPrefab;

    [SerializeField, HideInInspector]
    public GameObject _BombObjectPrefab;

    /*********************************************
     * ���� ��ü �Ϲ� ����
     * ***/
    [SerializeField, HideInInspector, Min(0f)]
    public int _ShootCount;

    [SerializeField, HideInInspector, Min(0f)]
    public int _ShootArea;

    [SerializeField, HideInInspector]
    public Transform _ShootPoint;

    [SerializeField, HideInInspector]
    public GameObject _AcidBombPrefab;

    [SerializeField, HideInInspector]
    public GameObject _AcidBombBigMarker;

    [SerializeField, HideInInspector]
    public GameObject _AcidBombSmallMarker;    


    //==============================================
    /////               Fields                  ////
    //==============================================
    [Header("Bullet Prefab")]
    private BossNepenthesProfile BossProfile;
    public GameObject FX_Hitprefab;
    public ChildPlatformsShaker PlatformShaker;
    public Transform HitTrasnform;

    [Header("Next Chapter")]
    [Tooltip("������ �׾��� �� �� ���� �� �̸�")]
    public string nextChapterName;
    public GameObject deathZone;
    public GameObject potal;
    private Stack<GameObject> HpCanvas;
    
    private float curTImer;
    private bool isOne = false;

    //==========================================
    /////           Magic Method            ////
    //==========================================
    void Awake()
    {
        #region Omit
        HpCanvas = new Stack<GameObject>();
        SetProfile(_AcidBombBigMarker);
        StateSetting();
        SettingPattern(CharacterMovementPattern[GetCurPhaseHpArray].EPatterns);
        AiSM.CurrentState = AiSM.Pattern[0];
        #endregion
    }

    void Start()
    {
        initializeUI();
    }

    void Update()
    {
        bool Ending = AiSM.CurrentState == AiDie;
        if(!Ending)
        {
            if (AiSM != null)
                AiSM.CurrentState.Update();
        }
        else if(Ending && !isOne)
        {
            curTImer += Time.deltaTime;
            if (curTImer > 1.5f)
            {
                GameObject obj = GameObject.Find("Floor2");
                //Debug.Log($"{obj.transform.childCount}");
                //for (int i = 0; i < obj.transform.childCount; i++)
                //{
                //    Transform child = obj.transform.GetChild(i);
                //    child.GetComponent<IEnviroment>().ExecutionFunction(0.5f);
                //}
                StartCoroutine(DestroyPlatforms());
                deathZone.SetActive(false);
                potal.SetActive(true);
                isOne = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Omit
        // ������ ����.
        if ( AiSM.CurrentState != AiDie && other.CompareTag("Bullet"))
        {
            // �������� �󸶳�?
            HP -= other.GetComponent<Bullet>().Damage;
            if (HP < 0)
            {    // State �ٲ��ֱ�.
                AiSM.ChangeState(AiHit);
            }
            else
            {
                AiSM.ChangeState(AiDie);
            }
        }
        #endregion
    }


    //============================================
    ////////         Core methods           //////
    //============================================

    private IEnumerator DestroyPlatforms()
    {
        GameObject obj = GameObject.Find("Floor2");
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            if (child.GetComponent<BrokenPlatformBehavior>().isBroken) continue;
            child.GetComponent<IEnviroment>().ExecutionFunction(0f);  // �̰� �ڷ�ƾ����?
            yield return new WaitForSeconds(0.1f);
            //if (obj.transform.childCount % 6 == 0)
            //{
            //    yield return new WaitForSeconds(0.25f);
            //}
        }
    }

    public override void initializeUI()
    {
        #region Omit
        GameObject obj = GameObject.Find("HPArea");
        //HpCanvas = new GameObject[obj.transform.childCount];
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            var piece = obj.transform.GetChild(i);
            if (piece != null)
            {
                HpCanvas.Push(piece.gameObject);
            }
        }
        #endregion
    }

    public override void Hit()
    {
        #region Omit
        base.Hit();
        GameObject hpGage = HpCanvas.Pop();
        hpGage.GetComponent<Animator>().SetTrigger("isDamaged");
        GameObject FX_Hit = GameObject.Instantiate(FX_Hitprefab, HitTrasnform.position, FX_Hitprefab.transform.rotation, this.transform.parent);
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_Hit);
        Destroy(FX_Hit, 1.0f);
        #endregion
    }

    private void StateSetting()
    {
        #region Omit
        AiSM = AIStateMachine.CreateFormGameObject(this.gameObject);

        AiIdle = new BossNepenthesIdleState(AiSM, IdleRate);
        AiWait = new BossNepenthesWaitState(AiSM, WaitRate);
        AiAttack = new BossNepenthesVineAttack(AiSM, this, _LeftVinePrefab, _RightVinePrefab, _FXVineAttackPrefab, _BombObjectPrefab , PlatformShaker);
        AiAttack2 = new BossNepenthesOneShot(AiSM, BossProfile, _MaximumSize, _FlightTime);
        SetProfile(_AcidBombSmallMarker);
        AiAttack3 = new BossNepenthesSmallShotGun(AiSM, BossProfile, _FlightTime, _ShootCount, _ShootArea);
        AiAttack4 = new BossNepenthesOneShot(AiSM, BossProfile, _MinimumSize, _FlightTime);
        // �״� ���.
        AiHit = new BossNepenthesHitState(AiSM, _LeftVinePrefab, _RightVinePrefab);
        AiDie = new BossNepenthesDieState(AiSM, _LeftVinePrefab, _RightVinePrefab, nextChapterName);
        #endregion
    }

    //=====================================================
    ////////           Public methods                //////
    //=====================================================

    public void SetProfile(GameObject ShotMarker)
    {
        BossProfile.SetProfile(_AcidBombPrefab, _ShootPoint, ShotMarker);
    }


    /******************************************************
     * coroutine
     * */
    public void CoroutineFunc(Func<float, IEnumerator> coFunc, float time)
    {
        StartCoroutine(coFunc(time));
    }
}
