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
        private SerializedProperty RecoveryCompleteProperty;
        private SerializedProperty GrabTargetProperty;
        private SerializedProperty FullyExtendedProperty;


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
                if (targetObj.GrabTarget != null)
                {
                    Vector3 grabPos = targetObj.GrabTarget.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.GrabTarget.transform;
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
                if (targetObj.GrabTarget != null){

                    Vector3 grabPos = targetObj.GrabTarget.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.GrabTarget.transform;
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

                targetObj = (target as PullableObject);
                dataListProperty = serializedObject.FindProperty("_datas");
                GrabTargetProperty = serializedObject.FindProperty("GrabTarget");
                BeginSnappedProperty = serializedObject.FindProperty("OnBeginSnapped");
                RecoveryCompleteProperty = serializedObject.FindProperty("OnStrechRecoveryComplete");
                FullyExtendedProperty = serializedObject.FindProperty("OnFullyExtended");
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

            EditorGUILayout.TextField("Extended Ratio", $"{targetObj.ExtendedLength} ({targetObj.NormalizedExtendedLength}%)");

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

                    GameObject value = (GameObject)EditorGUILayout.ObjectField("Grab Target", GrabTargetProperty.objectReferenceValue, typeof(GameObject), true);
                    if (scope.changed) GrabTargetProperty.objectReferenceValue = value;
                }

                /**��Ʈ��� ��뿩��...*/
                using ( var scope = new EditorGUI.ChangeCheckScope()){

                    EditorGUILayout.ToggleLeft("RootMotion", true, GUILayout.Width(90f));
                }

            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        private void GUI_ShowEvents()
        {
            #region Omit
            if (BeginSnappedProperty == null || RecoveryCompleteProperty == null) return;

            EditorGUILayout.PropertyField(BeginSnappedProperty);
            EditorGUILayout.PropertyField(RecoveryCompleteProperty);
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
        [System.NonSerialized] public Quaternion InverseQuat;
        [System.NonSerialized] public Vector3    PrevPos;
        [System.NonSerialized] public Vector3    originDir;
        [System.NonSerialized] public float     originLength;
        [System.NonSerialized] public float     originLengthDiv;
        [System.NonSerialized] public float     lengthRatio;
    }
    #endregion

    //=========================================
    /////            Property             /////
    //=========================================
    public float   FullyExtendedLength
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
    public float   NormalizedExtendedLength 
    {
        get
        {
            if (GrabTarget == null || _datas == null || _datas.Length == 0 || _datas[0].Tr == null){

                return 0f;
            }

            float Root2Target = (GrabTarget.transform.position - _datas[0].Tr.position).magnitude;

            /**�ִ���� �����ڰ� �ʱ�ȭ�����ʾҴٸ� �ʱ�ȭ.*/
            if(_fullyExtendedDiv<0) {

                _fullyExtendedDiv = (1f / FullyExtendedLength);
            }

            return ( Root2Target * _fullyExtendedDiv );
        }
    }
    public float   ExtendedLength
    {
        get
        {
            if (GrabTarget == null || _datas == null || _datas.Length == 0 || _datas[0].Tr == null){

                return 0f;
            }

            return (GrabTarget.transform.position - _datas[0].Tr.position).magnitude;
        }
    }
    public Vector3 RootPoint
    {
        get
        {
            bool dataIsNull     = (_datas==null);
            bool dataIsEmpty    = (_datas.Length == 0);
            bool IndexOneIsNull = (_datas[0].Tr==null);

            if(dataIsNull || dataIsEmpty || IndexOneIsNull) return Vector3.zero;

            return _datas[0].Tr.position;
        }
    }

    [SerializeField] public float            BreakingExtensionLength = 1f;
    [SerializeField] public GameObject       GrabTarget; 
    [SerializeField] public PullableObjEvent OnBeginSnapped;
    [SerializeField] public PullableObjEvent OnStrechRecoveryComplete;
    [SerializeField] public PullableObjEvent OnFullyExtended;



    //=======================================
    //////            Fields            /////
    //=======================================
    [SerializeField, HideInInspector] 
    private BoneData[] _datas;

    [SerializeField, HideInInspector]
    private int        _dataCount = -1;

    private const int   _fabrikLimit       = 100;
    private const float _SpringValue       = .025f;
    private float       _SpringSpeed       = 0f;

    private float       _fullyExtendedLen = -1f;
    private float       _fullyExtendedDiv  = -1f;
    private float       _boneCountDiv      = 1f;


    //===========================================
    /////          Magic methods            /////
    //===========================================
    private void Awake()
    {
        #region Omit
        _fullyExtendedLen      = FullyExtendedLength;
        _fullyExtendedDiv      = (1f/ _fullyExtendedLen);
        _boneCountDiv          = (1f/_datas.Length);

        Debug.Log($"FullyLen: {_fullyExtendedLen}");

        /**************************************
         *   �̺�Ʈ �븮�� �ʱ�ȭ....
         * ***/
        if (OnBeginSnapped == null){

            OnBeginSnapped = new PullableObjEvent();
        }

        if(OnStrechRecoveryComplete==null){

            OnStrechRecoveryComplete= new PullableObjEvent();
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
            data.InverseQuat     = Quaternion.Inverse(data.Tr.rotation);
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

        /**������� ���� ���� ó��...*/
        if(GrabTarget!=null){

            LookAtTarget(GrabTarget.transform.position);
            return;
        }

        /**���󺹱͵� ���� ó��...*/
        
    }

    private void OnDrawGizmos()
    {
        #region Omit
        if (GrabTarget == null) return;

        Vector3 grabPos = GrabTarget.transform.position;
        Vector3 rootPos = RootPoint;

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
    private bool LookAtTarget(Vector3 targetPos)
    {
        #region Omit
        if (_dataCount<2) return false;

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
        ref BoneData rootDirBone  = ref _datas[1];
        ref BoneData lastBone     = ref _datas[_dataCount - 1];
        Transform    targetTr     = GrabTarget.transform;

        int   leftCount     = _fabrikLimit;
        float root2forward  = 2f;

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

        return true;
        #endregion
    }

    private void ApplyForwardIK(int applyIndex, Vector3 target)
    {
        #region Omit
        ref BoneData bone     = ref _datas[applyIndex];
        ref BoneData nextBone = ref _datas[applyIndex+1];

        Vector3 bone2Target = (target - bone.OriginPos).normalized;
        Quaternion rotQuat  = GetQuatBetweenVector(bone.originDir, bone2Target);

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

    private Quaternion GetQuatBetweenVector(Vector3 from, Vector3 to, float ratio=1f)
    {
        #region Omit
        float angle     = Vector3.Angle(from, to) * ratio;
        Vector3 cross   = Vector3.Cross(from, to);
        return Quaternion.AngleAxis(angle, cross);
        #endregion
    }




    //============================================
    //////          Public methods           /////
    //============================================
    public void Hold( GameObject grabTarget )
    {

    }

    public void UnHold()
    {
            
    }


}
