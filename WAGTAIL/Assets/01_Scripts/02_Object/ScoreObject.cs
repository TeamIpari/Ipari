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
 *    ��ȣ�ۿ��, Ư�� ������Ҹ� ī�����ϴ� ����� ������ ������Ʈ�Դϴ�...
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
             *    ��� ������Ƽ���� ǥ���Ѵ�....
             * ***/
            GUI_Initialized();

            GUI_ShowUseMagnetMovement();

            GUI_ShowScoreType();

            GUI_ShowGetKind();

            GUI_ShowVFX();

            /**���� ��ȭ�ߴٸ� �����Ѵ�...*/
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
             *   ��� ������Ƽ���� �ʱ�ȭ�Ѵ�...
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
             *   �ڼ� �����Ӱ� ���õ� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            if (UseMagnetMovementProperty == null) return;

            SerializedProperty umm = UseMagnetMovementProperty;
            if(umm.boolValue = EditorGUILayout.ToggleLeft("Use Magnet Movement", umm.boolValue))
            {
                /**�ڼ� �������� ����� ���, ���� ������Ƽ���� ǥ���Ѵ�...*/
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
             *   �ڼ� �����Ӱ� ���õ� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            if (ScoreTypeProperty== null || CocosiIndexProperty==null) return;

            using(var scope = new EditorGUI.ChangeCheckScope())
            {
                ScoreType scoreValue = ScoreTypeProperty.GetEnumValue<ScoreType>();
                System.Enum value    = EditorGUILayout.EnumPopup("Score Type", scoreValue);

                /**���� ��ȭ�Ͽ��ٸ� �����Ѵ�...*/
                if(scope.changed){

                    ScoreTypeProperty.SetEnumValue(value);
                }
            }

            /*************************************************
             *  ���ڽð� ������ ���, ���� ������Ƽ�� ǥ���Ѵ�...
             * ***/
            if(ScoreTypeProperty.GetEnumValue<ScoreType>()==ScoreType.Cocosi)
            {
                int value = EditorGUILayout.IntField("Cocosi Index",CocosiIndexProperty.intValue);
                CocosiIndexProperty.intValue = System.Math.Clamp(value, 0, 4);

                EditorGUILayout.Space(7f);
            }

            #endregion
        }

        private void GUI_ShowGetKind()
        {
            #region Omit
            /****************************************************
             *   ȹ������ ���� ȿ���� ���õ� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            if (ItemGetTypeProperty == null || GetRaiseUpTimeProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                GetKind scoreValue = ItemGetTypeProperty.GetEnumValue<GetKind>();
                System.Enum value = EditorGUILayout.EnumPopup("Get Type", scoreValue);

                /**���� ��ȭ�Ͽ��ٸ� �����Ѵ�...*/
                if (scope.changed){

                    ItemGetTypeProperty.SetEnumValue(value);
                }
            }

            /*************************************************
             *  ���ڽð� ������ ���, ���� ������Ƽ�� ǥ���Ѵ�...
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

    [SerializeField] 
    public bool     UseMagnetMovement = false;

    [SerializeField] 
    public GetKind  ItemGetType  = GetKind.RaiseUp;

    [SerializeField, FormerlySerializedAs("scoreType")]
    public ScoreType ScoreType   = ScoreType.Coin;

    [SerializeField,MinMax(0,4)]
    private int     _CocosiIndex = 0;

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

    private float     _currTime        = 0f;    //��� �ð�....
    private bool      _isValid         = false;

    private Rigidbody _body;
    private Collider  _collider;
    private Transform _playerTr;



    //==================================================
    //////             Magic methods               /////
    //=================================================
    private void Awake()
    {
        #region Omit
        _body     = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        gameObject.tag = "Platform";

        /**�÷��̾��� Ʈ������ ĳ��...*/
        Player player = Player.Instance;
        if (player != null){
            _playerTr = player.transform;
        }

        _isValid = (_body != null || _collider != null || _playerTr != null);

        CocosiCollection collection = new CocosiCollection(4, 3);
        collection.SetCocosiCollect(1, 1, true);
        collection.SetCocosiCollect(1, 2, true);
        collection.SetCocosiCollect(1, 3, true);

        collection.SetCocosiCollect(2, 1, true);
        //collection.SetCocosiCollect(2, 2, true);
        collection.SetCocosiCollect(2, 3, true);


        Debug.Log($"é�� 1~2 Ŭ����: {collection.ChapterIsComplete(1,2)}/ Data: {Convert.ToString(collection.Data,2)}");
        #endregion
    }

    private void Update()
    {
        #region Omit

        /**********************************************
         *   �����ð� ��ŭ �����, �ڼ� ȿ���� �����Ѵ�...
         * ***/
        if(_currTime<MagnetMoveDelayTime){

            if ((_currTime+=Time.deltaTime) >= MagnetMoveDelayTime)
            {
                /**�����ð��� ������ ���...*/
                ApplyTimeout();
            }
            else return;
        }

        /**************************************
         *   �ڼ��� ������ ������ �����Ѵ�....
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

                /**������ ȹ���Ͽ��� ���...*/
                case (ScoreType.Coin):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Bead
                    );

                    gm.Coin++;
                    break;
                }

                /**���ڽø� ȹ���Ͽ��� ���...*/
                case (ScoreType.Cocosi):
                {
                    FModAudioManager.PlayOneShotSFX(
                          FModSFXEventType.Put_KoKoShi
                    );



                    break;
                }

                /**���� ȹ���Ͽ��� ���....*/
                case (ScoreType.Flower):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Flower
                    );

                    gm.Flower++;
                    break;
                }
        }

        /**������ ȹ��� �������� ȿ���� �����Ѵ�...*/
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

        /**�������� �ı��Ǹ鼭 ����Ʈ�� �����Ѵ�...*/
        SpawnVFX();
        Destroy(gameObject);

        #endregion
    }

    private void ApplyTimeout()
    {
        #region Omit
        /**********************************************
         *   Ư�� �����ð��� ������, ������ ȿ���� �����Ѵ�...
         * ***/

        /**ȹ��Ǿ��� ���...*/
        if(_collider!=null && _collider.enabled==false){

            SpawnVFX();
            Destroy(gameObject);
            return;
        }

        /**�ڼ�ȿ���� ������ ���...*/
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

    public void ExecutionFunction(float time)
    {
        /**NoUsed...*/
    }
}
