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
 *   ����� �� �ִ� ȿ���� �����ϴ� ������Ʈ�Դϴ�...
 * **/
public sealed class PullableObject : MonoBehaviour
{
    #region Editor_Extension
    /****************************************
     *   ������ Ȯ���� ���� private class...
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

            /**������Ƽ�� ǥ���Ѵ�....*/
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

                /**���� ����Ǿ��ٸ� �����Ѵ�...*/
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
             *   ��� ������ ǥ���Ѵ�.
             * ***/
            BoneData[]  datas = targetObj._datas;
            int         Count = datas.Length;

            Handles.BeginGUI();
            {
                Vector3 prevPos = (datas[0].Tr!=null? HandleUtility.WorldToGUIPoint(datas[0].Tr.position):Vector3.zero);

                /**���� ������Ʈ�� �ִٸ� Ʈ�������� ������ �� �ֵ��� �Ѵ�...*/
                if (targetObj.HoldingPoint != null)
                {
                    Vector3 grabPos = targetObj.HoldingPoint.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.HoldingPoint.transform;
                    }
                }

                /**GUI�� ǥ���Ѵ�....*/
                for (int i = 0; i < Count; i++){

                    if (datas[i].Tr == null) continue;
                    Vector3 pos    = datas[i].Tr.position;
                    Vector3 guiPos = HandleUtility.WorldToGUIPoint(pos);

                    Handles.color = BONE_QUAD_COLOR;
                    Handles.DrawLine( prevPos, guiPos, 1f );

                    prevPos = guiPos;

                    /**�ش� GUI��ư�� ������ Ʈ�������� ������ �� �ֵ��� �Ѵ�..*/
                    if (GUI_ShowBoneButton(guiPos, NodeButtonStyle))
                    {
                        selectionData = datas[i].Tr;
                    }
                }

                /**���� ������Ʈ�� �ִٸ� Ʈ�������� ������ �� �ֵ��� �Ѵ�...*/
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
             *   ������ ���� �ִٸ� Ʈ�������� ������ �� �ֵ��� �Ѵ�...
             * ***/
            if(selectionData!=null){

                /*********************************************
                 *   �ش� ������Ʈ�� ��Ȱ��ȭ �Ǿ��ٸ� ��ŵ�Ѵ�...
                 * ***/
                Tools.current = Tool.None;
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    Vector3     newPos  = selectionData.position;
                    Vector3     guiPos = HandleUtility.WorldToGUIPoint(newPos);
                    Quaternion  newQuat = selectionData.rotation;

                   /*********************************************
                    *   ������ �����Ѵ�...
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
                     *   ���� ���õ� ������ ���� Ʈ������ ������ �Ѵ�..
                     * ***/
                    switch (toolType){

                            /**�̵� ������ ���...*/
                            case (Tool.Move):
                            {
                                newPos = Handles.PositionHandle(selectionData.position, newQuat);
                                break;
                            }

                            /**ȸ�� ������ ���...*/
                            case (Tool.Rotate):
                            {
                                newQuat = Handles.RotationHandle(selectionData.rotation, newPos);
                                break;
                            }
                    }

                    /**���� �ٲ���ٸ� �����Ѵ�...*/
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
             *   ��� ������Ƽ���� ǥ���Ѵ�...
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
             *   ��� ������Ƽ�� ��Ÿ�ϵ��� �ʱ�ȭ�Ѵ�..
             * ***/

            /**��Ÿ�� �ʱ�ȭ....*/
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

            /**����ȭ ������Ƽ �ʱ�ȭ...*/
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

                /**���� �ٲ���ٸ� �����Ѵ�..*/
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

            /**�̵� ������ ���...*/
            if (toolType == Tool.Move){

                EditorGUI.Vector3Field(fieldRect, "", selectionData.position);
            }

            /**ȸ�� ������ ���...*/
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
             *   ��� ���� ȸ������ �ʱ�ȭ�Ѵ�.
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
                /**GrabTarget �����ʵ�...*/
                using (var scope = new EditorGUI.ChangeCheckScope()){

                    GameObject value = (GameObject)EditorGUILayout.ObjectField("Holding Point", GrabTargetProperty.objectReferenceValue, typeof(GameObject), true);
                    if (scope.changed) GrabTargetProperty.objectReferenceValue = value;
                }

                /**��Ʈ��� ��뿩��...*/
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
             *   �� ������ ������ ������ ���Ѵ�.
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

            /**�ִ���� �����ڰ� �ʱ�ȭ�����ʾҴٸ� �ʱ�ȭ.*/
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

    /**���� �ݵ��� ���õ� �ʵ�...*/
    private float  _lastExtendedLen = 0f;
    private float  _Yspeed = 2f;
    private float  _boundTime  = 0f;


    /**���� �������� ���õ� �ʵ�...*/
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
         *   �̺�Ʈ �븮�� �ʱ�ȭ....
         * ***/
        if (OnPullRelease == null){

            OnPullRelease = new PullableObjEvent();
        }

        if (OnFullyExtended == null){

            OnFullyExtended = new PullableObjEvent();
        }


        /**************************************
         *  �� ���� �ʱ�ȭ....
         * ***/
        if (_datas==null){

            _datas = new BoneData[10];
        }

        _dataCount = _datas.Length;
        for(int i=0; i<_dataCount-1; i++){

            ref BoneData data = ref _datas[i];
            ref BoneData next = ref _datas[i+1];

            /**������ �����ִٸ� ������...*/
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

        /**������ ���� �ڽĺθ� ���踦 �����Ѵ�...*/
        #endregion
    }

    private void Update()
    {
        if (_dataCount < 2) return;

        /*************************************
         *   �ܺηκ��� ����� ����� ó���� �Ѵ�..
         * ***/
        if(HoldingPoint!=null){

            /**�����ϰ� ������� ���� ó��...*/
            if (!UpdateFullExtendedVibration())
            {
                /**������ ������� �ʾ��� ����� ó��...*/
                UpdateLookAtTarget(HoldingPoint.transform.position);
            }
            return;
        }


        /****************************************
         *   ������ٰ� �������� ���� ó���� �Ѵ�...
         * ***/

        /**�������� ���� ó��...*/
        if(!UpdateBreakRestore()){

            /**õõ�� ���󺹱͵Ǵ� ����� ó��...*/
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
         *   FABRIK �˰�������, ����� ����Ű���� �Ѵ�...
         * ***/

        /**������ ������ ��Ʈ������ ���ʴ�� Ʈ�������� �����Ѵ�...*/
        ApplyForwardIK(_dataCount - 2, targetPos);

        for (int i=_dataCount-3; i >= 0; i--){

            ref BoneData next = ref _datas[i + 1];
            ApplyForwardIK(i, next.Tr.position);
        }


        /****************************************************
         *   ��Ʈ���� ���� ��ġ�� �ִ��� ������ �̵��ϵ��� ����...
         * ***/
        ref BoneData rootBone     = ref _datas[0];
        Transform    targetTr     = HoldingPoint.transform;

        int   leftCount     = _fabrikLimit;
        float root2forward  = 2f;

        /**��Ʈ���� ��ġ�� �ִ��� ������ ��ġ��Ų��...*/
        while (leftCount-- > 0 && root2forward > .05f)
        {
            /**��Ʈ���� ���� ��ġ�� �ٿ����´�...*/
            rootBone.Tr.position = rootBone.OriginPos;
            
            for(int i=1; i<_dataCount-1; i++){

                ref BoneData prev = ref _datas[i-1];
                ApplyBackwardIK(i, prev.Tr.position);
            }

            root2forward = (rootBone.OriginPos - rootBone.Tr.position).sqrMagnitude;
        }

        LastBone2GrabSolver();

        /**������ ��ġ�� ����Ѵ�...*/
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
         *   BackwardIK�� ���� ���� �κп� ������ ���� ���� �ʴ�
         *   �������� �ذ��Ѵ�...
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
         *   ���� �Ѱ�ġ�� �Ѿ�ٸ� �ı��ȴ�...
         * ***/
        if(!IsBroken && extendedRatio>=MaxScale)
        {
            IsBroken = true;
            _GrabTarget = null;
            _brokenTime *= .5f;
            return true;
        }


        /****************************************
         *   ������ ������� ���� ó���� �Ѵ�...
         * ***/
        else if (extendedRatio>=1f)
        {
            /**���� ������� ������ ������ ���� ó��...*/
            if(_lastExtendedLen>0){

                _Yspeed = (root2TargetLen - _lastExtendedLen) * (UseStrongShake ? 12f : 3f);
               _lastExtendedLen = 0;
                StretchFull();
                OnFullyExtended?.Invoke();
            }


            /******************************************
             *   ��꿡 �ʿ��� ��ҵ��� ��� ���Ѵ�...
             * ***/

            /**��꿡 ������ ������ ������ ���Ѵ�...*/
            ref BoneData root    = ref _datas[0];
            ref BoneData last    = ref _datas[_dataCount-1];

            /**������� ������ �����͸� �̿��Ͽ� ������ ���������� ���Ѵ�...*/
            Vector3 forward       = (HoldingPoint.transform.position - root.OriginPos);
            Vector3 forwardNormal = forward.normalized;
            Vector3 right         = Vector3.Cross(Vector3.up, forwardNormal).normalized;
            Vector3 up            = Vector3.Cross(forwardNormal, right).normalized;

            Vector3 a  = root.OriginPos;
            Vector3 cp = root.OriginPos + (forward*.5f) + (up*_Yspeed);
            Vector3 b  = HoldingPoint.transform.position;

            //Debug.DrawLine(cp, cp + up * _Yspeed, UnityEngine.Color.red);

            /**********************************************
             *   ������ ��� ��������Ͽ� ���� ������Ʈ �Ѵ�....
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

            /**�ݵ��� ���� �����ϴ� ȿ���� �����Ѵ�...*/
            if((_boundTime-=Time.deltaTime)<=0f)
            {
                _Yspeed = (-_Yspeed * .7f);
                _boundTime = .05f;
            }

            return true;
        }

        /**������ ��ġ�� ����Ѵ�...*/
        else _lastExtendedLen = (HoldingPoint.transform.position - _datas[0].Tr.position).magnitude;

        return false;
        #endregion
    }

    private void UpdateExtendedRestore()
    {
        #region Omit

        /************************************
         *   �� ������ ���� ��ġ�� ���ư���...
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
         *   ������ �����ϸ鼭 ���� ���̰� �پ���...
         * ***/
        if (_brokenTime <= 0) return true;

        _brokenTime -= Time.deltaTime;
        float progressRatio = (_brokenTime * _brokenDiv);

        /**��� ������ ũ�⸦ ���δ�.....*/
        for( int i=0; i<_dataCount-1; i++ )
        {
            ref BoneData curr = ref _datas[i];
            ref BoneData next = ref _datas[i + 1];

            Vector3 currDir = (next.Tr.position - curr.Tr.position).normalized;
            next.Tr.position = curr.Tr.position + (currDir * curr.originLength * progressRatio);
        }

        /**��� �پ����� ����� ó���� �Ѵ�...*/
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

        /**�� ������ ������ ��ġ�� ����Ѵ�...*/
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
