using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Drawing;
using UnityEngine.UIElements;
using IPariUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

/***********************************************
 *   당겨질 수 있는 효과를 제공하는 컴포넌트입니다...
 * **/
public sealed class PullableObject : MonoBehaviour
{
    #region Editor_Extension
    /****************************************
     *   에디터 확장을 위한 private class...
     * ***/
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(BoneData))]
    private sealed class BoneDataDrawer : PropertyDrawer
    {
        //====================================
        /////           Fields            ////
        //====================================
        private SerializedProperty TargetProperty;



        //=====================================================
        /////         Override and Magic methods          /////
        //=====================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(GUI_Initialized(property)==false) return;

            /**프로퍼티를 표시한다....*/
            GUI_ShowObjField(position);
        }



        //======================================================
        /////           GUI and Utility methods             ////
        //======================================================
        private float GetBaseHeight()
        {
            return GUI.skin.textField.CalcSize(GUIContent.none).y;
        }

        private bool GUI_Initialized(SerializedProperty property)
        {
            return (TargetProperty = property.FindPropertyRelative("Tr")) != null;
        }

        private void GUI_ShowObjField(Rect header)
        {
            #region Omit
            if (TargetProperty == null) return;

            using (var changedScope = new EditorGUI.ChangeCheckScope()){

                Transform curr = (Transform)EditorGUI.ObjectField(header, "bone Transform", TargetProperty.objectReferenceValue, typeof(Transform), true);

                /**값이 변경되었다면 갱신한다...*/
                if(changedScope.changed)
                {
                    TargetProperty.objectReferenceValue = curr;
                }

            }


            #endregion
        }
    }

    [CustomEditor(typeof(PullableObject))]
    private sealed class PullableObjectEditor : Editor
    {
        //====================================
        //////         Fields             ////
        //====================================
        private const int                         BONE_QUAD_SIZE  = 12;
        private static readonly UnityEngine.Color BONE_QUAD_COLOR = new UnityEngine.Color(.8f, .8f, .8f);
        private static readonly UnityEngine.Color GRAB_QUAD_COLOR = new UnityEngine.Color(0f, .8f, 0f);

        private static GUIStyle    NodeButtonStyle, GrabNodeButtonStyle;

        /**select target...*/
        private PullableObject     targetObj;
        private Transform          selectionData;
        private Tool               toolType = Tool.Move;

        /**Serialized Properties...*/
        private SerializedProperty dataListProperty;
        private SerializedProperty BeginSnappedProperty;
        private SerializedProperty GrabTargetProperty;
        private SerializedProperty FullyExtendedProperty;
        private SerializedProperty BreakLengthRatioProperty;
        private SerializedProperty UsedStrongVibrationProperty;


        //=====================================================
        /////          Magic and Override methods         /////
        //=====================================================
        private void OnSceneGUI()
        {
            #region Omit
            if (targetObj == null) return;
            if (targetObj._datas==null) return;
            if (targetObj._datas.Length==0) return;


            /**********************************************
             *   모든 본들을 표시한다.
             * ***/
            BoneData[]  datas = targetObj._datas;
            int         Count = datas.Length;

            Handles.BeginGUI();
            {
                Vector3 prevPos = (datas[0].Tr!=null? HandleUtility.WorldToGUIPoint(datas[0].Tr.position):Vector3.zero);

                /**잡힌 오브젝트가 있다면 트랜스폼을 편집할 수 있도록 한다...*/
                if (targetObj.HoldingPoint != null)
                {
                    Vector3 grabPos = targetObj.HoldingPoint.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.HoldingPoint.transform;
                    }
                }

                /**GUI를 표시한다....*/
                for (int i = 0; i < Count; i++){

                    if (datas[i].Tr == null) continue;
                    Vector3 pos    = datas[i].Tr.position;
                    Vector3 guiPos = HandleUtility.WorldToGUIPoint(pos);

                    Handles.color = BONE_QUAD_COLOR;
                    Handles.DrawLine( prevPos, guiPos, 1f );

                    prevPos = guiPos;

                    /**해당 GUI버튼을 누르면 트랜스폼을 편집할 수 있도록 한다..*/
                    if (GUI_ShowBoneButton(guiPos, NodeButtonStyle))
                    {
                        selectionData = datas[i].Tr;
                    }
                }

                /**잡힌 오브젝트가 있다면 트랜스폼을 편집할 수 있도록 한다...*/
                if (targetObj.HoldingPoint != null){

                    Vector3 grabPos = targetObj.HoldingPoint.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.HoldingPoint.transform;
                    }
                }
            }
            Handles.EndGUI();


            /*****************************************************
             *   선택한 본이 있다면 트랜스폼을 편집할 수 있도록 한다...
             * ***/
            if(selectionData!=null){

                /*********************************************
                 *   해당 오브젝트가 비활성화 되었다면 스킵한다...
                 * ***/
                Tools.current = Tool.None;
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    Vector3     newPos  = selectionData.position;
                    Vector3     guiPos = HandleUtility.WorldToGUIPoint(newPos);
                    Quaternion  newQuat = selectionData.rotation;

                   /*********************************************
                    *   도구를 변경한다...
                    * ***/
                   Event curr = Event.current;
                    if(curr.type==EventType.KeyDown)
                    {
                        switch(curr.keyCode){

                                case (KeyCode.W):
                                {
                                    toolType = Tool.Move;
                                    break;
                                }

                                case (KeyCode.E):
                                {
                                    toolType = Tool.Rotate;
                                    break;
                                }
                        }
                    }

                    /**********************************************
                     *   현재 선택된 도구에 따라 트랜스폼 변경을 한다..
                     * ***/
                    switch (toolType){

                            /**이동 도구일 경우...*/
                            case (Tool.Move):
                            {
                                newPos = Handles.PositionHandle(selectionData.position, newQuat);
                                break;
                            }

                            /**회전 도구일 경우...*/
                            case (Tool.Rotate):
                            {
                                newQuat = Handles.RotationHandle(selectionData.rotation, newPos);
                                break;
                            }
                    }

                    /**값이 바뀌었다면 갱신한다...*/
                    if (scope.changed){

                        Undo.RecordObject(selectionData, $"Changed transform of {selectionData.name}.");
                        selectionData.position = newPos;
                        selectionData.rotation = newQuat;
                    }

                    Handles.BeginGUI();
                    {
                        GUI_ShowBoneTransform(guiPos, selectionData);
                    }
                    Handles.EndGUI();
                }
            }

            #endregion
        }

        private void OnEnable()
        {
            GUI_Initialized();
            selectionData = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*****************************************
             *   모든 프로퍼티들을 표시한다...
             * ***/
            GUI_Initialized();

            GUI_ShowGrabTarget();

            GUI_ShowExtendedRatio();

            GUI_ShowBreakLengthRatio();

            EditorGUILayout.Space(10f);


            GUI_ShowResetRotateButton();

            GUI_ShowDataList();

            EditorGUILayout.Space(10f);


            GUI_ShowEvents();

            if(GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
        }



        //==========================================
        //////          GUI methods             ////
        //==========================================
        private void GUI_Initialized()
        {
            #region Omit
            /****************************************
             *   모든 프로퍼티와 스타일들을 초기화한다..
             * ***/

            /**스타일 초기화....*/
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, BONE_QUAD_COLOR);
            t.Apply();

            Texture2D t2 = new Texture2D(1, 1);
            t2.SetPixel(0, 0, GRAB_QUAD_COLOR);
            t2.Apply();

            NodeButtonStyle = new GUIStyle();
            NodeButtonStyle.normal.background = t;

            GrabNodeButtonStyle= new GUIStyle();
            GrabNodeButtonStyle.normal.background = t2;

            /**직렬화 프로퍼티 초기화...*/
            if(targetObj==null){

                targetObj                       = (target as PullableObject);
                dataListProperty                = serializedObject.FindProperty("_datas");
                GrabTargetProperty              = serializedObject.FindProperty("_GrabTarget");
                BeginSnappedProperty            = serializedObject.FindProperty("OnPullRelease");
                FullyExtendedProperty           = serializedObject.FindProperty("OnFullyExtended");
                BreakLengthRatioProperty        = serializedObject.FindProperty("MaxScale");
                UsedStrongVibrationProperty     = serializedObject.FindProperty("UseStrongShake");
            }
            #endregion
        }

        private bool GUI_ShowBoneButton(Vector2 pos, GUIStyle style)
        {
            #region Omit
            float halfQuadSize  = (BONE_QUAD_SIZE * .5f);
            Rect btnRect        = new Rect(
                pos - new Vector2( halfQuadSize, halfQuadSize ),
                new Vector3( BONE_QUAD_SIZE, BONE_QUAD_SIZE )
            );

            return GUI.Button(btnRect, GUIContent.none, style);
            #endregion
        }

        private void GUI_ShowExtendedRatio()
        {
            #region Omit
            if (targetObj == null) return;

            EditorGUILayout.TextField("Current Length", $"{targetObj.Length} ({targetObj.NormalizedLength}%)");

            #endregion
        }

        private void GUI_ShowBreakLengthRatio()
        {
            #region Omit
            if (BreakLengthRatioProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                float value = EditorGUILayout.FloatField("Max Scale", BreakLengthRatioProperty.floatValue);

                /**값이 바뀌었다면 갱신한다..*/
                if(scope.changed){

                    BreakLengthRatioProperty.floatValue = value;
                }
            }

            #endregion
        }

        private void GUI_ShowBoneTransform(Vector2 pos, Transform selectionData)
        {
            #region Omit
            Rect fieldRect = new Rect(
                pos - new Vector2(100f, -80f),
                new Vector3(300f, 40f)
            );

            /**이동 도구일 경우...*/
            if (toolType == Tool.Move){

                EditorGUI.Vector3Field(fieldRect, "", selectionData.position);
            }

            /**회전 도구일 경우...*/
            else
            {
                Quaternion.Euler(EditorGUI.Vector3Field(fieldRect, "", selectionData.localEulerAngles));
            }
            #endregion
        }

        private void GUI_ShowResetRotateButton()
        {
            #region Omit
            if (targetObj == null) return;

            /*************************************
             *   모든 본의 회전량을 초기화한다.
             * ***/
            if(GUILayout.Button("Reset all bones rotation"))
            {
                int Count        = targetObj._datas.Length;
                BoneData[] datas = targetObj._datas;

                Undo.RegisterChildrenOrderUndo(targetObj._datas[0].Tr, "Changed All bones transform");
                for (int i=1; i<Count; i++){

                    if (datas[i].Tr == null) continue;
                    datas[i].Tr.localRotation = Quaternion.identity;
                }
            }

            #endregion
        }

        private void GUI_ShowDataList()
        {
            #region Omit
            if (dataListProperty == null) return;

            EditorGUILayout.PropertyField(dataListProperty);

            #endregion
        }

        private void GUI_ShowGrabTarget()
        {
            #region Omit
            if (GrabTargetProperty == null) return;

            EditorGUILayout.BeginHorizontal();
            {
                /**GrabTarget 참조필드...*/
                using (var scope = new EditorGUI.ChangeCheckScope()){

                    GameObject value = (GameObject)EditorGUILayout.ObjectField("Holding Point", GrabTargetProperty.objectReferenceValue, typeof(GameObject), true);
                    if (scope.changed) GrabTargetProperty.objectReferenceValue = value;
                }

                /**루트모션 사용여부...*/
                using ( var scope = new EditorGUI.ChangeCheckScope()){

                    bool value = EditorGUILayout.ToggleLeft("Use Strong Shake", UsedStrongVibrationProperty.boolValue, GUILayout.Width(130f));
                    if(scope.changed) UsedStrongVibrationProperty.boolValue = value;
                }

            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        private void GUI_ShowEvents()
        {
            #region Omit
            if (BeginSnappedProperty == null || FullyExtendedProperty == null) return;

            EditorGUILayout.PropertyField(BeginSnappedProperty);
            EditorGUILayout.PropertyField(FullyExtendedProperty);

            #endregion
        }

    }
#endif
    #endregion

    #region Define
    [System.Serializable]
    public sealed class PullableObjEvent : UnityEvent
    {
    }

    [System.Serializable]
    private struct BoneData
    {
        public Transform  Tr;

        [System.NonSerialized] public Vector3    OriginPos;
        [System.NonSerialized] public Quaternion OriginQuat;
        [System.NonSerialized] public Quaternion PrevQuat;
        [System.NonSerialized] public Vector3    PrevPos;
        [System.NonSerialized] public Vector3    originDir;
        [System.NonSerialized] public Vector3    LastPos;
        [System.NonSerialized] public Vector3    LastDir;
        [System.NonSerialized] public float     originLength;
        [System.NonSerialized] public float     originLengthDiv;
        [System.NonSerialized] public float     lengthRatio;
    }
    #endregion

    //=========================================
    /////            Property             /////
    //=========================================
    public float        MaxLength
    {
        get
        {
            /*************************************
             *   각 본들의 길이의 총합을 구한다.
             * ***/
            float len = 0f;
            int Count = _datas.Length;

            for (int i = 1; i < Count; i++){

                len += (_datas[i].Tr.position - _datas[i - 1].Tr.position).magnitude;
            }

            return len;
        }
    }
    public float        NormalizedLength 
    {
        get
        {
            if (HoldingPoint == null || _datas == null || _datas.Length == 0 || _datas[0].Tr == null){

                return 0f;
            }

            float Root2Target = (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;

            /**최대길이 연산자가 초기화되지않았다면 초기화.*/
            if(_fullyExtendedDiv<0) {

                _fullyExtendedDiv = (1f / MaxLength);
            }

            return ( Root2Target * _fullyExtendedDiv );
        }
    }
    public float        Length
    {
        get
        {
            if (HoldingPoint == null || _datas == null || _datas.Length == 0 || _datas[0].Tr == null){

                return 0f;
            }

            return (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;
        }
    }
    public bool         IsBroken { get; private set; } = false;
    public bool         IsDestroy { get; private set; } = false;
    public int          BoneCount { get { return _dataCount; } }
    public Vector3      StartPosition
    {
        get
        {
            if(_datas == null || _datas.Length == 0 || _datas[0].Tr == null) 
                return Vector3.zero;

            return _datas[0].Tr.position;
        }
    }
    public Vector3      EndPosition
    {
        get
        {
            bool dataIsNull = (_datas == null);
            bool dataIsEmpty = (_datas.Length == 0);
            bool IndexOneIsNull = (_datas[_dataCount - 1].Tr == null);

            if (_datas == null || _datas.Length == 0 || _datas[_dataCount - 2].Tr == null) return Vector3.zero;

            return _datas[_dataCount-2].Tr.position;
        }
    }
    public GameObject   HoldingPoint 
    { 
        get { return _GrabTarget; } 
        set
        {
            _GrabTarget = value;
        }
    }

    [SerializeField] public float            MaxScale = 1.5f;
    [SerializeField] public bool             UseStrongShake = false;
    [SerializeField] private GameObject       _GrabTarget; 
    [SerializeField] public  PullableObjEvent OnPullRelease;
    [SerializeField] public  PullableObjEvent OnFullyExtended;



    //=======================================
    //////            Fields            /////
    //=======================================
    [SerializeField, HideInInspector] 
    private BoneData[] _datas;

    [SerializeField, HideInInspector]
    private int        _dataCount = -1;

    private const int   _fabrikLimit       = 100;
    private float       _fullyExtendedLen = -1f;
    private float       _fullyExtendedDiv  = -1f;
    private float       _boneCountDiv      = 1f;

    /**줄의 반동에 관련된 필드...*/
    private float  _lastExtendedLen = 0f;
    private float  _Yspeed = 2f;
    private float  _boundTime  = 0f;


    /**줄의 끊어짐과 관련된 필드...*/
    private float       _brokenTime = .1f;
    private float       _brokenDiv  = 0f;



    //===========================================
    /////          Magic methods            /////
    //===========================================
    private void Awake()
    {
        #region Omit
        _fullyExtendedLen      = MaxLength;
        _fullyExtendedDiv      = (1f/ _fullyExtendedLen);
        _boneCountDiv          = (1f/(_datas.Length-1));
        _brokenDiv             = (1f/_brokenTime);

        gameObject.layer = LayerMask.NameToLayer("Pullable");

        /**************************************
         *   이벤트 대리자 초기화....
         * ***/
        if (OnPullRelease == null){

            OnPullRelease = new PullableObjEvent();
        }

        if (OnFullyExtended == null){

            OnFullyExtended = new PullableObjEvent();
        }


        /**************************************
         *  본 정보 초기화....
         * ***/
        if (_datas==null){

            _datas = new BoneData[10];
        }

        _dataCount = _datas.Length;
        for(int i=0; i<_dataCount-1; i++){

            ref BoneData data = ref _datas[i];
            ref BoneData next = ref _datas[i+1];

            /**연결이 끊겨있다면 마무리...*/
            if(data.Tr==null)
            {
                _dataCount = (i+1);
                return;
            }

            data.Tr.parent       = transform;
            data.OriginPos       = data.Tr.position;
            data.OriginQuat      = data.Tr.rotation;
            data.PrevQuat        = data.Tr.rotation;
            data.originDir       = (next.Tr.position-data.Tr.position).normalized;
            data.originLength    = (next.Tr.position - data.Tr.position).magnitude;
            data.originLengthDiv = (1f / data.originLength);
            data.lengthRatio     = (data.originLength * _fullyExtendedDiv);
        }

        /**마지막 본의 자식부모 관계를 해지한다...*/
        #endregion
    }

    private void Update()
    {
        if (_dataCount < 2) return;

        /*************************************
         *   외부로부터 당겨질 경우의 처리를 한다..
         * ***/
        if(HoldingPoint!=null){

            /**팽팽하게 당겨졌을 때의 처리...*/
            if (!UpdateFullExtendedVibration())
            {
                /**완전히 당겨지지 않았을 경우의 처리...*/
                UpdateLookAtTarget(HoldingPoint.transform.position);
            }
            return;
        }


        /****************************************
         *   당겨졌다가 놓아졌을 때의 처리를 한다...
         * ***/

        /**끊어졌을 때의 처리...*/
        if(!UpdateBreakRestore()){

            /**천천히 원상복귀되는 경우의 처리...*/
            UpdateExtendedRestore();
        }
    }

    private void OnDrawGizmos()
    {
        #region Omit
        if (HoldingPoint == null) return;

        Vector3 grabPos = HoldingPoint.transform.position;
        Vector3 rootPos = StartPosition;

        Vector3 root2GrabDir = (grabPos-rootPos).normalized;
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawLine(rootPos, rootPos + root2GrabDir * _fullyExtendedLen);

        Gizmos.color = UnityEngine.Color.white;
        Gizmos.DrawLine(rootPos, grabPos);
        #endregion
    }



    //===========================================
    /////          Core methods             /////
    //===========================================
    private void UpdateLookAtTarget(Vector3 targetPos)
    {
        #region Omit
        if (_dataCount<2) return;

        /********************************************
         *   FABRIK 알고리즘으로, 대상을 가리키도록 한다...
         * ***/

        /**마지막 본에서 루트본까지 차례대로 트랜스폼을 변경한다...*/
        ApplyForwardIK(_dataCount - 2, targetPos);

        for (int i=_dataCount-3; i >= 0; i--){

            ref BoneData next = ref _datas[i + 1];
            ApplyForwardIK(i, next.Tr.position);
        }


        /****************************************************
         *   루트본이 원래 위치에 최대한 가깝게 이동하도록 보간...
         * ***/
        ref BoneData rootBone     = ref _datas[0];
        Transform    targetTr     = HoldingPoint.transform;

        int   leftCount     = _fabrikLimit;
        float root2forward  = 2f;

        /**루트본의 위치에 최대한 가깝게 배치시킨다...*/
        while (leftCount-- > 0 && root2forward > .05f)
        {
            /**루트본을 원래 위치에 붙여놓는다...*/
            rootBone.Tr.position = rootBone.OriginPos;
            
            for(int i=1; i<_dataCount-1; i++){

                ref BoneData prev = ref _datas[i-1];
                ApplyBackwardIK(i, prev.Tr.position);
            }

            root2forward = (rootBone.OriginPos - rootBone.Tr.position).sqrMagnitude;
        }

        LastBone2GrabSolver();

        /**마지막 위치를 기록한다...*/
        _lastExtendedLen = (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;
        #endregion
    }

    private void ApplyForwardIK(int applyIndex, Vector3 target)
    {
        #region Omit
        ref BoneData bone     = ref _datas[applyIndex];
        ref BoneData nextBone = ref _datas[applyIndex+1];

        Vector3 bone2Target = (target - bone.OriginPos).normalized;
        Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(bone.originDir, bone2Target);

        bone.Tr.position        = target + bone2Target*(-bone.originLength);
        bone.Tr.rotation        = (rotQuat*bone.OriginQuat);
        #endregion
    }

    private void ApplyBackwardIK(int applyIndex, Vector3 target)
    {
        #region Omit
        ref BoneData bone = ref _datas[applyIndex];
        ref BoneData next = ref _datas[applyIndex+1];

        Vector3 bone2Target = (target-bone.Tr.position).normalized;
        bone.Tr.position    = target + (bone2Target * -bone.originLength);
        #endregion
    }

    private void LastBone2GrabSolver()
    {
        #region Omit
        /****************************************************
         *   BackwardIK로 인한 잡은 부분에 마지막 본이 닿지 않는
         *   문제점을 해결한다...
         * ***/
        if (HoldingPoint == null) return;

        ref BoneData lastData = ref _datas[_dataCount - 1];
        Vector3      grabPos  = HoldingPoint.transform.position;
        float last2TargetLen  = (grabPos - lastData.Tr.position).magnitude;
        float        partLen  = (last2TargetLen * _boneCountDiv);   

        for(int i=1; i<_dataCount-1; i++)
        {
            ref BoneData curr = ref _datas[i];
            ref BoneData prev = ref _datas[i-1];
            ref BoneData next = ref _datas[i + 1];

            Vector3 prev2CurrDir = (curr.Tr.position - prev.Tr.position).normalized;
            curr.Tr.position = prev.Tr.position + (prev2CurrDir * (prev.originLength+partLen));
        }

        #endregion
    }

    private bool UpdateFullExtendedVibration()
    {
        #region Omit
        if (HoldingPoint == null) return false;

        float root2TargetLen  = (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;
        float extendedRatio   = (root2TargetLen * _fullyExtendedDiv);

        /****************************************
         *   줄이 한계치를 넘어섰다면 파괴된다...
         * ***/
        if(!IsBroken && extendedRatio>=MaxScale)
        {
            IsBroken = true;
            _GrabTarget = null;
            _brokenTime *= .5f;
            return true;
        }


        /****************************************
         *   완전히 당겨졌을 때의 처리를 한다...
         * ***/
        else if (extendedRatio>=1f)
        {
            /**줄이 당겨져서 완전히 펴졌을 때의 처리...*/
            if(_lastExtendedLen>0){

                _Yspeed = (root2TargetLen - _lastExtendedLen) * (UseStrongShake ? 12f : 3f);
               _lastExtendedLen = 0;
                StretchFull();
                OnFullyExtended?.Invoke();
            }


            /******************************************
             *   계산에 필요한 요소들을 모두 구한다...
             * ***/

            /**계산에 참조할 본들의 참조를 구한다...*/
            ref BoneData root    = ref _datas[0];
            ref BoneData last    = ref _datas[_dataCount-1];

            /**당겨지는 방향의 업벡터를 이용하여 배지어 제어점들을 구한다...*/
            Vector3 forward       = (HoldingPoint.transform.position - root.OriginPos);
            Vector3 forwardNormal = forward.normalized;
            Vector3 right         = Vector3.Cross(Vector3.up, forwardNormal).normalized;
            Vector3 up            = Vector3.Cross(forwardNormal, right).normalized;

            Vector3 a  = root.OriginPos;
            Vector3 cp = root.OriginPos + (forward*.5f) + (up*_Yspeed);
            Vector3 b  = HoldingPoint.transform.position;

            //Debug.DrawLine(cp, cp + up * _Yspeed, UnityEngine.Color.red);

            /**********************************************
             *   배지어 곡선을 기반으로하여 본을 업데이트 한다....
             * ***/
            float ratio = 0f;
            int   count = (_dataCount - 1);
            for(int i=0; i<count; i++)
            {
                ref BoneData curr = ref _datas[i];
                ref BoneData next = ref _datas[i+1];

                Vector3 currBezier = IpariUtility.GetBezier(ref a, ref cp, ref b, ratio);
                Vector3 nextBezier = IpariUtility.GetBezier(ref a, ref cp, ref b, (ratio += curr.lengthRatio));
                Vector3 curr2Next  = (nextBezier-currBezier).normalized;
                Quaternion rotQuat = IpariUtility.GetQuatBetweenVector(curr.originDir, curr2Next);

                //Debug.DrawLine(currBezier, nextBezier, UnityEngine.Color.blue);

                next.Tr.position = nextBezier;
                curr.Tr.rotation = (rotQuat * curr.OriginQuat);
            }

            /**반동이 점점 감소하는 효과를 적용한다...*/
            if((_boundTime-=Time.deltaTime)<=0f)
            {
                _Yspeed = (-_Yspeed * .7f);
                _boundTime = .05f;
            }

            return true;
        }

        /**마지막 위치를 기록한다...*/
        else _lastExtendedLen = (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;

        return false;
        #endregion
    }

    private void UpdateExtendedRestore()
    {
        #region Omit

        /************************************
         *   각 본들이 원래 위치로 돌아간다...
         * ***/
        int   Count = (_dataCount-1);
        float delta = (Time.deltaTime * 3f);

        for (int i=0; i<_dataCount-1; i++)
        {
            ref BoneData curr = ref _datas[i];
            ref BoneData next = ref _datas[i + 1];

            Vector3 currDir    = (next.Tr.position - curr.Tr.position).normalized;
            Quaternion rotQuat = IpariUtility.GetQuatBetweenVector(currDir, curr.originDir, delta);

            curr.Tr.position +=  (curr.OriginPos - curr.Tr.position) * delta;
            curr.Tr.rotation =  (rotQuat * curr.Tr.rotation);
        }
        #endregion
    }

    private bool UpdateBreakRestore()
    {
        #region Omit
        if (IsBroken == false) return false;

        /*******************************************
         *   빠르게 복귀하면서 점점 길이가 줄어든다...
         * ***/
        if (_brokenTime <= 0) return true;

        _brokenTime -= Time.deltaTime;
        float progressRatio = (_brokenTime * _brokenDiv);

        /**모든 본들의 크기를 줄인다.....*/
        for( int i=0; i<_dataCount-1; i++ )
        {
            ref BoneData curr = ref _datas[i];
            ref BoneData next = ref _datas[i + 1];

            Vector3 currDir = (next.Tr.position - curr.Tr.position).normalized;
            next.Tr.position = curr.Tr.position + (currDir * curr.originLength * progressRatio);
        }

        /**모두 줄어들었을 경우의 처리를 한다...*/
        if(progressRatio<=0f){

            OnPullRelease?.Invoke();
            IsDestroy = true;
            Destroy(gameObject);
        }
        return true;
        #endregion
    }

    private void WriteLastBoneTransform()
    {
        #region Omit

        /**각 본들의 마지막 위치를 기록한다...*/
        for (int i = 0; i < _dataCount - 1; i++)
        {
            ref BoneData curr = ref _datas[i];
            ref BoneData next = ref _datas[i + 1];

            curr.LastDir = (next.Tr.position - curr.Tr.position).normalized;
            curr.LastPos = curr.Tr.position;
        }
        #endregion
    }




    //============================================
    //////          Public methods           /////
    //============================================
    public void StretchFull()
    {
        #region Omit
        if (_dataCount < 2) return;

        ref BoneData rootBone    = ref _datas[0];
        ref BoneData rootDirBone = ref _datas[1];

        Vector3 rootDir = (rootDirBone.Tr.position - rootBone.Tr.position).normalized;
        for (int i = 1; i < _dataCount-1; i++)
        {
            if (_datas[i].Tr == null) continue;
            
            ref BoneData currBone = ref _datas[i];
            Quaternion   rotQut   = IpariUtility.GetQuatBetweenVector(currBone.originDir, rootDir);
            currBone.Tr.rotation  = (rotQut * currBone.OriginQuat);
        }
        #endregion
    }

    public Vector3 GetBonePosition( int index )
    {
        #region Omit
        if (_dataCount==0 || _datas==null) return Vector3.zero;  

        index = Mathf.Clamp(index, 0, _dataCount - 1);
        return _datas[index].Tr.position;
        #endregion
    }

    public Vector3 GetBoneDir(int index)
    {
        #region Omit
        if (_dataCount == 0 || _datas == null) return Vector3.zero;

        index = Mathf.Clamp(index, 0, _dataCount - 1);
        ref BoneData curr = ref _datas[index];
        ref BoneData next = ref _datas[index+1];

        return (next.Tr.position - curr.Tr.position).normalized;
        #endregion
    }

    public float GetBoneLength(int index)
    {
        #region Omit
        if (_dataCount == 0 || _datas == null) return 0f;

        return _datas[index].originLength;
        #endregion
    }
}
