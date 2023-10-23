using System;
using UnityEngine;
using IPariUtility;
using UnityEngine.Serialization;

using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#region Define
[System.Serializable]
public struct CocosiCollection
{
    //===========================================
    //////            Property            ///////
    //===========================================
    public int  Data            { get { return _value; } }
    public int  CompleteData    { get { return GetCompleteCollection(_chapterCount, _chapterCocosiNum); } }
    public bool IsCompleted     { get { return (_value==GetCompleteCollection(_chapterCount, _chapterCocosiNum)); } }



    //===========================================
    ///////             Fields              /////
    //===========================================
    private int _value;
    private int _chapterCocosiNum;
    private int _chapterCount;

    

    //=============================================
    /////           Public methods            /////
    //=============================================
    public CocosiCollection(int chapterCount,int cocosiCount)
    {
        #region Omit
        _value            = 0;
        _chapterCount     = chapterCount;
        _chapterCocosiNum = cocosiCount;
        #endregion
    }

    private int GetCompleteCollection(int chapterCount, int cocosiCount)
    {
        #region Omit
        int maxValue      = (sizeof(int)*8);
        int completeCount = System.Math.Clamp((chapterCount * cocosiCount), 0, maxValue);

        return (1 << completeCount)-1;
        #endregion
    }

    public void ClearCollection()
    {
        _value = 0;
    }

    public void SetCocosiCollect(int chapter, int index, bool isCollect)
    {
        #region Omit
        if (chapter < 1 || chapter > _chapterCount || index < 1 || index > _chapterCocosiNum)
            return;

        int flagNum = (chapter-1)*_chapterCocosiNum + (index-1);

        if(isCollect) _value |= (1 << flagNum);
        else _value &= ~(1 << flagNum);

        #endregion
    }

    public bool GetCocosiCollect(int chapter, int index)
    {
        #region Omit
        if (chapter < 1 || chapter > _chapterCount || index < 1 || index > _chapterCocosiNum)
            return false;

        int flagNum = (chapter - 1) * _chapterCocosiNum + index;

        return (_value & (1 << flagNum))!=0;

        #endregion
    }

    public bool ChapterIsComplete(params int[] chapters)
    {
        #region Omit
        {
            int chapter = (1 << _chapterCocosiNum) - 1;
            int Count = chapters.Length;
            int left = Count;

            for (int i = 0; i < Count; i++){

                int checkFlags = (chapter << (_chapterCocosiNum * chapters[i]));
                if ((_value & checkFlags) != 0) left--;
            }

            return (left == 0);
        }
        #endregion
    }

};
#endregion

/*********************************************************************
 *    상호작용시, 특정 수집요소를 카운팅하는 기능이 구현된 컴포넌트입니다...
 * *****/
public sealed class ScoreObject : MonoBehaviour, IEnviroment
{
    #region Editor_Extension
#if UNITY_EDITOR
    [CustomEditor(typeof(ScoreObject))]
    private sealed class ScoreObjectEditor : Editor
    {
        //===========================================
        /////               Fields              /////
        //===========================================
        private SerializedProperty UseMagnetMovementProperty;
        private SerializedProperty ItemGetTypeProperty;
        private SerializedProperty ScoreTypeProperty;
        private SerializedProperty CocosiIndexProperty;
        private SerializedProperty CocosiChapterProperty;
        private SerializedProperty MagnetMoveDelayTimeProperty;
        private SerializedProperty GetRaiseUpTimeProperty;
        private SerializedProperty InteractionVFXProperty;



        //====================================================
        ///////           Override methods              //////
        //====================================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*******************************************
             *    모든 프로퍼티들을 표시한다....
             * ***/
            GUI_Initialized();

            GUI_ShowUseMagnetMovement();

            GUI_ShowScoreType();

            GUI_ShowGetKind();

            GUI_ShowVFX();

            /**값이 변화했다면 갱신한다...*/
            if(GUI.changed){ 

                serializedObject.ApplyModifiedProperties(); 
            }
        }



        //=================================================
        ////////            GUI methods             ///////
        //=================================================
        private void GUI_Initialized()
        {
            #region Omit
            /***********************************
             *   모든 프로퍼티들을 초기화한다...
             * ***/
            if(UseMagnetMovementProperty==null){

                UseMagnetMovementProperty = serializedObject.FindProperty("UseMagnetMovement");
            }

            if(ItemGetTypeProperty==null){

                ItemGetTypeProperty = serializedObject.FindProperty("ItemGetType");
            }

            if(ScoreTypeProperty==null){

                ScoreTypeProperty = serializedObject.FindProperty("ScoreType");
            }

            if ( CocosiIndexProperty== null){

                CocosiIndexProperty = serializedObject.FindProperty("_CocosiIndex");
            }

            if (CocosiChapterProperty == null)
            {
                CocosiChapterProperty = serializedObject.FindProperty("_CocosiChapter");
            }

            if (MagnetMoveDelayTimeProperty == null){

                MagnetMoveDelayTimeProperty = serializedObject.FindProperty("_MagnetMoveDelayTime");
            }

            if ( GetRaiseUpTimeProperty == null){

               GetRaiseUpTimeProperty = serializedObject.FindProperty("_GetRaiseUpTime");
            }

            if ( InteractionVFXProperty== null){

                InteractionVFXProperty = serializedObject.FindProperty("InteractionVFX");
            }

            #endregion
        }

        private void GUI_ShowUseMagnetMovement()
        {
            #region Omit
            /**********************************************
             *   자석 움직임과 관련된 프로퍼티들을 표시한다...
             * ***/
            if (UseMagnetMovementProperty == null) return;

            SerializedProperty umm = UseMagnetMovementProperty;
            if(umm.boolValue = EditorGUILayout.ToggleLeft("Use Magnet Movement", umm.boolValue))
            {
                /**자석 움직임을 사용할 경우, 관련 프로퍼티들을 표시한다...*/
                float value = EditorGUILayout.FloatField("Magnet Move DelayTime", MagnetMoveDelayTimeProperty.floatValue);
                MagnetMoveDelayTimeProperty.floatValue = Mathf.Clamp(value, 0f, float.MaxValue);
                EditorGUILayout.Space(6f);
            }

            #endregion
        }

        private void GUI_ShowScoreType()
        {
            #region Omit
            /**********************************************
             *   자석 움직임과 관련된 프로퍼티들을 표시한다...
             * ***/
            if (ScoreTypeProperty== null || CocosiIndexProperty==null) return;

            using(var scope = new EditorGUI.ChangeCheckScope())
            {
                ScoreType scoreValue = ScoreTypeProperty.GetEnumValue<ScoreType>();
                System.Enum value    = EditorGUILayout.EnumPopup("Score Type", scoreValue);

                /**값이 변화하였다면 갱신한다...*/
                if(scope.changed){

                    ScoreTypeProperty.SetEnumValue(value);
                }
            }

            /*************************************************
             *  코코시가 지정된 경우, 관련 프로퍼티를 표시한다...
             * ***/
            if(ScoreTypeProperty.GetEnumValue<ScoreType>()==ScoreType.Cocosi)
            {
                int value = EditorGUILayout.IntField("Cocosi Index",CocosiIndexProperty.intValue);
                CocosiIndexProperty.intValue = System.Math.Clamp(value, 0, 4);
                int chaptervalue = EditorGUILayout.IntField("Cocosi Chapter", CocosiChapterProperty.intValue);
                CocosiChapterProperty.intValue = System.Math.Clamp(chaptervalue, 0, 2);

                EditorGUILayout.Space(7f);
            }

            #endregion
        }

        private void GUI_ShowGetKind()
        {
            #region Omit
            /****************************************************
             *   획득했을 때의 효과와 관련된 프로퍼티들을 표시한다...
             * ***/
            if (ItemGetTypeProperty == null || GetRaiseUpTimeProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                GetKind scoreValue = ItemGetTypeProperty.GetEnumValue<GetKind>();
                System.Enum value = EditorGUILayout.EnumPopup("Get Type", scoreValue);

                /**값이 변화하였다면 갱신한다...*/
                if (scope.changed){

                    ItemGetTypeProperty.SetEnumValue(value);
                }
            }

            /*************************************************
             *  코코시가 지정된 경우, 관련 프로퍼티를 표시한다...
             * ***/
            if (ItemGetTypeProperty.GetEnumValue<GetKind>() == GetKind.RaiseUp)
            {
                float value = EditorGUILayout.FloatField("Raise Up Duration",GetRaiseUpTimeProperty.floatValue);
                GetRaiseUpTimeProperty.floatValue = Mathf.Clamp(value, 0f, float.MaxValue);

                EditorGUILayout.Space(7f);
            }

            #endregion
        }

        private void GUI_ShowVFX()
        {
            #region Omit
            if (InteractionVFXProperty == null) return;

            InteractionVFXProperty.objectReferenceValue = EditorGUILayout.ObjectField(
                   "Interaction VFX",
                   InteractionVFXProperty.objectReferenceValue,
                   typeof(GameObject),
                   true

            );

            #endregion
        }

    }
#endif
    #endregion

    public enum GetKind
    {
        None,
        RaiseUp
    }

    //=================================================
    /////                Property                 /////
    //=================================================
    public Rigidbody Body                 { get { return _body; } }
    public Collider  Collider             { get { return _collider;  }}
    public float     MagnetMoveDelayTime  { get { return _MagnetMoveDelayTime; } set { _MagnetMoveDelayTime = (value < 0f ? 0.01f : value); } }
    public float     GetRaiseUpTime       { get { return _GetRaiseUpTime; } set { _GetRaiseUpTime = (value < 0f ? 0f : value); } }
    public string    EnviromentPrompt     { get; }=String.Empty;
    public bool      IsHit                { get; set; } = false;
    public int       CocosiIndex          { get { return _CocosiIndex; } set { _CocosiIndex = System.Math.Clamp(value, 0, 4); } }
    public int       CocosiChapter        { get { return _CocosiChapter; } set { _CocosiChapter = System.Math.Clamp(value, 0, 2); } }

    [SerializeField] 
    public bool     UseMagnetMovement = false;

    [SerializeField] 
    public GetKind  ItemGetType  = GetKind.RaiseUp;

    [SerializeField, FormerlySerializedAs("scoreType")]
    public ScoreType ScoreType   = ScoreType.Coin;

    [SerializeField,MinMax(0,4)]
    private int     _CocosiIndex = 0;

    [SerializeField, MinMax(0, 2)]
    private int     _CocosiChapter = 0;

    [SerializeField,MinMax(0f, float.MaxValue)]
    private float       _MagnetMoveDelayTime  = 0f;

    [SerializeField,MinMax(0f, float.MaxValue)]
    private float       _GetRaiseUpTime       = .25f;

    [SerializeField,FormerlySerializedAs("_interactionVFX")] 
    public GameObject   InteractionVFX;

    

    //=================================================
    /////                 Fields                  /////
    //=================================================
    private const float Height    = 1.7f;
    private const float FlightDiv = 4.0f;

    private float     _currTime        = 0f;    //경과 시간....
    private bool      _isValid         = false;

    private Rigidbody _body;
    private Collider  _collider;
    private Transform _playerTr;
    private Animator  _animator;



    //==================================================
    //////             Magic methods               /////
    //=================================================
    private void Awake()
    {
        #region Omit
        _body     = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();

        gameObject.tag = "Platform";

        /**플레이어의 트랜스폼 캐싱...*/
        Player player = Player.Instance;
        if (player != null){
            _playerTr = player.transform;
        }

        _isValid = (_body != null || _collider != null || _playerTr != null);

        if (ScoreType == ScoreType.Cocosi)
        {
            Debug.Log(CocosiChapter);
            Debug.Log(CocosiIndex);
        }
        #endregion
    }

    private void Update()
    {
        #region Omit

        /**********************************************
         *   지연시간 만큼 대기후, 자석 효과를 적용한다...
         * ***/
        if(_currTime<MagnetMoveDelayTime){

            if ((_currTime+=Time.deltaTime) >= MagnetMoveDelayTime)
            {
                /**지연시간이 끝났을 경우...*/
                ApplyTimeout();
            }
            else return;
        }

        /**************************************
         *   자석의 움직임 로직을 적용한다....
         * ****/
        if (UseMagnetMovement == false || _body==null) return;

        Vector3 center2Player = (_playerTr.position - _collider.bounds.center);
        float   distance      = center2Player.magnitude;
        float  magentDistance = (10 / distance) * 1.25f;
        _body.velocity        = center2Player * magentDistance;

        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GetItem();
    }


    //================================================
    ///////           Core methods              //////
    //================================================
    private void GetItem()
    {
        #region Omit
        GameManager gm = GameManager.GetInstance();
        if (gm == null || _collider==null) return;

        _collider.enabled = false;
        UseMagnetMovement = false;

        switch (ScoreType){

                /**코인을 획득하였을 경우...*/
                case (ScoreType.Coin):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Bead
                    );

                    gm.Coin++;
                    break;
                }

                /**코코시를 획득하였을 경우...*/
                case (ScoreType.Cocosi):
                {
                    FModAudioManager.PlayOneShotSFX(
                          FModSFXEventType.Put_KoKoShi
                    );

                    gm.cocosi[CocosiChapter][CocosiIndex] = true;
                    UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>()
                        .SetCocosiUI(CocosiChapter, CocosiIndex, true);
                    transform.GetChild(2).gameObject.SetActive(false);
                    _animator.SetTrigger("escape");

                    break;
                }

                /**꽃을 획득하였을 경우....*/
                case (ScoreType.Flower):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Flower
                    );

                    gm.Flower++;
                    break;
                }
        }

        /**아이템 획득시 떠오르는 효과를 구현한다...*/
        if (ItemGetType == GetKind.RaiseUp)
        {
            UseRigidbody();
            _body.useGravity = false;
            _body.velocity   = IpariUtility.CaculateVelocity(
                transform.position + (Vector3.up*Height),
                transform.position,
                _GetRaiseUpTime
            );

            SetTime(_GetRaiseUpTime);
            return;
        }

        /**아이템이 파괴되면서 이펙트를 생성한다...*/
        SpawnVFX();
        if (_animator == null) Destroy(gameObject);

        #endregion
    }

    private void ApplyTimeout()
    {
        #region Omit
        /**********************************************
         *   특정 지연시간이 지난후, 지정한 효과를 적용한다...
         * ***/

        /**획득되었을 경우...*/
        if(_collider!=null && _collider.enabled==false){

            SpawnVFX();
            Destroy(gameObject);
            return;
        }

        /**자석효과를 적용할 경우...*/
        if(UseMagnetMovement && _body!=null && _collider!=null)
        {
            _body.velocity      = Vector3.zero;
            _body.useGravity    = false;
            _collider.isTrigger = true;
            return;
        }
        #endregion
    }

    private void SpawnVFX()
    {
        #region Omit
        if (InteractionVFX != null)
        {
            GameObject exploVFX = Instantiate(
                InteractionVFX,
                gameObject.transform.position,
                gameObject.transform.rotation
            );

            Destroy(exploVFX, 2);
            return;
        }

        Debug.LogWarning("InteractionVFX was missing!");
        #endregion
    }

    public void UseRigidbody(bool isTrigger=true, bool useGravity=false)
    {
        #region Omit
        if (_body != null || _collider==null) return;

        _body = gameObject.AddComponent<Rigidbody>();
        _body.useGravity    = useGravity;
        _collider.isTrigger = isTrigger;
        #endregion
    }

    public void SetTime(float time, float div=1f)
    {
        _MagnetMoveDelayTime = (time / div);
        _currTime = 0f;
    }

    public bool Interact()
    {
        GetItem();
        return true;
    }

    public void AnimationEvent()
    {
        SpawnVFX();
        Destroy(gameObject);
    }

    public void ExecutionFunction(float time)
    {
        /**NoUsed...*/
    }
}
