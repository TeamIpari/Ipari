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
             *   모든 본들을 표시한다.
             * ***/
            BoneData[]  datas = targetObj._datas;
            int         Count = datas.Length;

            Handles.BeginGUI();
            {
                Vector3 prevPos = (datas[0].Tr!=null? HandleUtility.WorldToGUIPoint(datas[0].Tr.position):Vector3.zero);

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
                if(targetObj.GrabTarget!=null){

                    Vector3 grabPos    = targetObj.GrabTarget.transform.position;
                    Vector3 grabGUIPos = HandleUtility.WorldToGUIPoint(grabPos);

                    if (GUI_ShowBoneButton(grabGUIPos, GrabNodeButtonStyle))
                    {
                        selectionData = targetObj.GrabTarget.transform;
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

                    GameObject value = (GameObject)EditorGUILayout.ObjectField("Grab Target", GrabTargetProperty.objectReferenceValue, typeof(GameObject), true);
                    if (scope.changed) GrabTargetProperty.objectReferenceValue = value;
                }

                /**루트모션 사용여부...*/
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
    public float   NormalizedExtendedLength 
    {
        get
        {
            bool NotGrabbed     = (GrabTarget == null);
            bool dataIsNull     = (_datas == null);
            bool dataIsEmpty    = (_datas.Length == 0);
            bool IndexOneIsNull = (_datas[0].Tr == null);

            if (NotGrabbed || dataIsNull || dataIsEmpty || IndexOneIsNull){

                return 0f;
            }

            float Root2Target = (GrabTarget.transform.position - _datas[0].Tr.position).magnitude;

            /**최대길이 연산자가 초기화되지않았다면 초기화.*/
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
            bool NotGrabbed = (GrabTarget == null);
            bool dataIsNull = (_datas == null);
            bool dataIsEmpty = (_datas.Length == 0);
            bool IndexOneIsNull = (_datas[0].Tr == null);

            if (NotGrabbed || dataIsNull || dataIsEmpty || IndexOneIsNull){

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
    [SerializeField][HideInInspector] 
    private BoneData[] _datas;

    [SerializeField][HideInInspector]
    private int        _dataCount = -1;

    private const float _SpringValue       = .025f;
    private float       _SpringSpeed       = 0f;

    private float       _fullyExtendedLen = -1f;
    private float       _fullyExtendedDiv  = -1f;
    private float       _boneCountDiv      = 1f;
    private float       _prevExtendedRatio = 0f;

    private static readonly Vector3 _gravity = new Vector3(0.05f, -9.811f, .05f);


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
         *   이벤트 대리자 초기화....
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

            data.OriginPos       = data.Tr.position;
            data.OriginQuat      = data.Tr.rotation;
            data.PrevQuat        = data.Tr.rotation;
            data.originDir       = (next.Tr.position-data.Tr.position).normalized;
            data.originLength    = (next.Tr.position - data.Tr.position).magnitude;
            data.originLengthDiv = (1f / data.originLength);
            data.lengthRatio     = (data.originLength * _fullyExtendedDiv);
            
        }
        #endregion
    }

    private void Update()
    {
        /***************************************
         *   잡은 오브젝트가 존재할 경우의 처리...
         * ***/
        if (GrabTarget != null && _dataCount >= 2)
        {
            ref BoneData rootData       = ref _datas[0];
            ref BoneData rootDirData    = ref _datas[1];
            ref BoneData middleData     = ref _datas[Mathf.RoundToInt((_dataCount - 1) * .5f)];
            ref BoneData lastData       = ref _datas[_dataCount - 2];
            ref BoneData lastDirData    = ref _datas[_dataCount - 1];

            /**각 본들이 목표로 할 대상까지의 방향을 정한다....*/
            Transform targetTr = GrabTarget.transform;
            Vector3 rootDir = (rootDirData.Tr.position - rootData.Tr.position).normalized;
            Vector3 targetDir = (targetTr.position - rootData.Tr.position);

            float targetLength = targetDir.magnitude;
            float extendedRatio = (targetLength * _fullyExtendedDiv);

            targetDir.Normalize();

            float rotExtendedRatio = Mathf.Clamp(extendedRatio, 0f, 1f);

            bool isFullyExtended = (rotExtendedRatio >= 1f);
            float addedDelta = (isFullyExtended ? Time.deltaTime * 10f : _boneCountDiv * .1f);
            float currDelta = (_boneCountDiv * addedDelta);

            int Count = (_dataCount - 1);
            Vector3 targetPos    = targetTr.position;
            Vector3 last2GrabDir = (targetPos - lastData.Tr.position);
            float last2Grab = last2GrabDir.magnitude;
            float currRatio = 0f;

            last2GrabDir.Normalize();

            for (int i = 0; i < Count; i++)
            {

                ref BoneData nextData = ref _datas[i + 1];
                ref BoneData data = ref _datas[i];

                /**루트모션과의 차이를 구한다...*/
                Vector3 oriDir2TargetDir = (targetDir - data.originDir) * rotExtendedRatio;
                Vector3 goalDir = (data.originDir + oriDir2TargetDir);
                Vector3 boneDir = (nextData.Tr.position - data.Tr.position).normalized;
                Quaternion rotQuat = GetQuatBetweenVector(boneDir, goalDir.normalized, currRatio += Time.deltaTime * .5f);

                data.Tr.rotation *= (rotQuat);
                data.PrevQuat = data.Tr.rotation;
            }

        }

    }



    //===========================================
    /////          Core methods             /////
    //===========================================
    private bool DatasIsValid()
    {
        #region Omit
        int Count = 0;
        if (_datas == null || (Count=_datas.Length) < 2) return false;

        for(int i=0; i<Count; i++)
        {
            if (_datas[i].Tr == null) return false;
        }

        return true;
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
