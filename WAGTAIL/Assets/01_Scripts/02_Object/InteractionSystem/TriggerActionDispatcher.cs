using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/********************************************************************
 *  Trigger 영역 및 함수 등록에 대한 편리한 기능을 제공하는 컴포넌트입니다..
 * ***/
[AddComponentMenu("Triggerable/TriggerActionDispatcher")]
[RequireComponent(typeof(BoxCollider))]
public sealed class TriggerActionDispatcher : MonoBehaviour
{
    #region Define_TriggerUnityEvent
    [System.Serializable]
    public sealed class UnityEventTrigger : UnityEvent
    {
    }
    #endregion

    #region Editor_Extension
#if UNITY_EDITOR
    /*************************************
     *  에디터 확장을 위한 private class...
     * **/
    [CustomEditor(typeof(TriggerActionDispatcher))]
    private sealed class TriggerActionDispatcherEditor : Editor
    {
        //=====================================
        /////           Fields            /////
        //=====================================
        private SerializedProperty EnterProperty;
        private SerializedProperty ExitProperty;
        private SerializedProperty TagListProperty;
        private SerializedProperty ColorProperty;

        private bool             _foldout = true;
        private static GUIStyle  _boldStyle;
        private static GUIStyle  _foldBoldStyle;


        //=======================================
        /////       Override methods         ////
        //=======================================
        public override void OnInspectorGUI()
        {
            GUI_Initialized();

            serializedObject.Update();

            /*********************************
             *  모든 Property들을 표시한다....
             * ***/
            GUI_ShowColor();

            GUI_ShowTagList();

            GUI_DrawLine(10f);

            GUI_ShowEvents();

            /**변경사항이 있다면 갱신..*/
            if (GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
        }



        //====================================
        /////        GUI methods         ////
        //====================================
        private void GUI_Initialized()
        {
            #region Omit
            /*******************************************
             *   모든 SerializedProperty들을 초기화한다..
             * ***/
            if (EnterProperty==null){

                EnterProperty = serializedObject.FindProperty("OnTriggerEnterEvent");
            }

            if (ExitProperty == null){

                ExitProperty = serializedObject.FindProperty("OnTriggerExitEvent");
            }

            if(TagListProperty==null){

                TagListProperty = serializedObject.FindProperty("_InspectorAddTags");
            }

            if(ColorProperty==null){

                ColorProperty = serializedObject.FindProperty("GizmosColor");
            }

            /**모든 Style들을 초기화한다...*/
            if(_boldStyle==null){

                _boldStyle = EditorStyles.boldLabel;
            }

            if(_foldBoldStyle==null){

                _foldBoldStyle = EditorStyles.foldout;
                _foldBoldStyle.fontStyle= FontStyle.Bold;
            }


            #endregion
        }

        private void GUI_ShowProperty(SerializedProperty property)
        {
            if (property == null) return;
            EditorGUILayout.PropertyField(property, true);
        }

        private void GUI_ShowEvents()
        {
            #region Omit
            if (EnterProperty == null || ExitProperty == null) return;

            EditorGUILayout.LabelField("Trigger Events", _boldStyle);
            EditorGUILayout.PropertyField(EnterProperty, true);
            EditorGUILayout.PropertyField(ExitProperty, true);
            #endregion
        }

        private void GUI_ShowTagList()
        {
            #region Omit
            if (TagListProperty == null) return;

            using(var scope = new EditorGUI.IndentLevelScope())
            {
                /*******************************************
                 *   접기 토글 및 태그 리스트를 표시한다..
                 * ***/
                _foldout = EditorGUILayout.Foldout(_foldout, "Triggerable Tags", _foldBoldStyle);


                /**********************************************
                 *  접히지 않은 상태라면 모든 태그 목록들을 표시한다.
                 * ***/
                if (_foldout)
                {
                    EditorGUI.indentLevel++;
                    /////////////////////////////////////
                    int Count = TagListProperty.arraySize;
                    for (int i = 0; i < Count; i++){

                        /**현재 Tag를 표시한다.*/
                        EditorGUILayout.BeginHorizontal();
                        using (var changeScope = new EditorGUI.ChangeCheckScope())
                        {
                            SerializedProperty element = TagListProperty.GetArrayElementAtIndex(i);
                            string tag = EditorGUILayout.TagField($"Tags ({i})",element.stringValue);
                            
                            /**값이 바뀌었다면...*/
                            if(changeScope.changed){

                                element.stringValue = tag;
                            }

                            /**삭제 버튼을 누르면 해당 원소를 삭제한다.*/
                            if(GUILayout.Button("-", GUILayout.Width(40f))){

                                TagListProperty.DeleteArrayElementAtIndex(i);
                                i--;
                                Count--;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    /**추가 버튼을 누르면 원소를 추가한다.*/
                    if (GUILayout.Button("+")){

                        TagListProperty.InsertArrayElementAtIndex(Count);
                        SerializedProperty element = TagListProperty.GetArrayElementAtIndex(Count);
                        element.stringValue = UnityEditorInternal.InternalEditorUtility.tags[0];

                    }
                    ///////////////////////////////////
                    EditorGUI.indentLevel--;
                }
                
                scope.Dispose();
            }

            #endregion
        }

        private void GUI_ShowColor()
        {
            #region Omit
            if (ColorProperty == null) return;

            ColorProperty.colorValue = EditorGUILayout.ColorField("Trigger Color", ColorProperty.colorValue);
            #endregion
        }

        private void GUI_DrawLine(float space = 0f, float subOffset = 0f)
        {
            #region Omit
            EditorGUILayout.Space(15f);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15 + subOffset, rect.y), new Vector2(rect.width + 15 - subOffset * 2, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f+space);
            #endregion
        }


    }
#endif
    #endregion

    //==========================================
    /////           Property               /////
    //==========================================
    [SerializeField] public UnityEventTrigger OnTriggerEnterEvent;
    [SerializeField] public UnityEventTrigger OnTriggerExitEvent;
    [SerializeField] private Color GizmosColor = Color.red;



    //==========================================
    /////            Fields               /////
    //==========================================
#if UNITY_EDITOR
    private Collider     _collider;
#endif
    /**Trigger Tag 관련...*/
    [SerializeField] private string[] _InspectorAddTags;

    private int _tagFlags = 0;
    private static Dictionary<string, int> _tagLists = new Dictionary<string, int>();
    private static int _tagIndex = 0;


    //====================================
    /////       Magic methods         ////
    //====================================
    private void Start()
    { 
        #region Omit
        if (OnTriggerEnterEvent==null){

            OnTriggerEnterEvent = new UnityEventTrigger();
        }

        if (OnTriggerExitEvent == null){

            OnTriggerExitEvent = new UnityEventTrigger();
        }

        Collider collider= GetComponent<Collider>();
        if(collider!=null){

            collider.isTrigger = true;
        }

        /***********************************************
         *   Inspector에서 추가한 tag list들을 추가한다...
         * **/
        if (_InspectorAddTags == null) return;

        int Count = _InspectorAddTags.Length;
        for(int i=0; i<Count; i++)
        {
            AddTriggerTag(_InspectorAddTags[i]);
        }
        _InspectorAddTags = null;
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTriggerTag(other.tag)==false) return;
        OnTriggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (CompareTriggerTag(other.tag) == false) return;
        OnTriggerExitEvent?.Invoke();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        #region Omit
        /************************************
         *  화면에 Trigger 범위를 출력한다...
         * **/
        if (_collider==null) _collider = GetComponent<Collider>();
        if (_collider != null)
        {
            Bounds bounds = _collider.bounds;
            Vector3 pos   = transform.position;
            Color   color = GizmosColor;
            color.a = .5f;

            Gizmos.color = color;
            Gizmos.DrawCube(bounds.center, bounds.size);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        #endregion
    }
#endif



    //====================================
    /////      Public methods         ////
    //====================================
    public void AddTriggerTag(string addTag)
    {
        #region Omit
        if (_tagIndex>31){

            throw new Exception("TriggerActionDispatcher: 사용할 수 있는 Tag의 수는 최대 32개 까지입니다.");
        }

        /**tag가 존재하지 않을 경우, 그 키를 가져온다.*/
        if (_tagLists.ContainsKey(addTag)==false){

            _tagLists.Add(addTag, (1<<_tagIndex));
            _tagIndex++;
        }

        _tagFlags |= _tagLists[addTag];
        #endregion
    }

    public void RemoveTriggerTag(string removeTag)
    {
        #region Omit
        if (_tagLists.ContainsKey(removeTag))
        {
            _tagFlags &= ~_tagLists[removeTag];
        }
        #endregion
    }

    public bool CompareTriggerTag(string tag)
    {
        if (_tagFlags == 0) return true;
        if (_tagLists.ContainsKey(tag) == false) return false;

        int flag = _tagLists[tag];
        return (_tagFlags & flag)!=0;
    }


}
