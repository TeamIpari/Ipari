using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/********************************************************************
 *  Trigger ���� �� �Լ� ��Ͽ� ���� ���� ����� �����ϴ� ������Ʈ�Դϴ�..
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
     *  ������ Ȯ���� ���� private class...
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
             *  ��� Property���� ǥ���Ѵ�....
             * ***/
            GUI_ShowColor();

            GUI_ShowTagList();

            GUI_DrawLine(10f);

            GUI_ShowEvents();

            /**��������� �ִٸ� ����..*/
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
             *   ��� SerializedProperty���� �ʱ�ȭ�Ѵ�..
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

            /**��� Style���� �ʱ�ȭ�Ѵ�...*/
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
                 *   ���� ��� �� �±� ����Ʈ�� ǥ���Ѵ�..
                 * ***/
                _foldout = EditorGUILayout.Foldout(_foldout, "Triggerable Tags", _foldBoldStyle);


                /**********************************************
                 *  ������ ���� ���¶�� ��� �±� ��ϵ��� ǥ���Ѵ�.
                 * ***/
                if (_foldout)
                {
                    EditorGUI.indentLevel++;
                    /////////////////////////////////////
                    int Count = TagListProperty.arraySize;
                    for (int i = 0; i < Count; i++){

                        /**���� Tag�� ǥ���Ѵ�.*/
                        EditorGUILayout.BeginHorizontal();
                        using (var changeScope = new EditorGUI.ChangeCheckScope())
                        {
                            SerializedProperty element = TagListProperty.GetArrayElementAtIndex(i);
                            string tag = EditorGUILayout.TagField($"Tags ({i})",element.stringValue);
                            
                            /**���� �ٲ���ٸ�...*/
                            if(changeScope.changed){

                                element.stringValue = tag;
                            }

                            /**���� ��ư�� ������ �ش� ���Ҹ� �����Ѵ�.*/
                            if(GUILayout.Button("-", GUILayout.Width(40f))){

                                TagListProperty.DeleteArrayElementAtIndex(i);
                                i--;
                                Count--;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    /**�߰� ��ư�� ������ ���Ҹ� �߰��Ѵ�.*/
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
    /**Trigger Tag ����...*/
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
         *   Inspector���� �߰��� tag list���� �߰��Ѵ�...
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
         *  ȭ�鿡 Trigger ������ ����Ѵ�...
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

            throw new Exception("TriggerActionDispatcher: ����� �� �ִ� Tag�� ���� �ִ� 32�� �����Դϴ�.");
        }

        /**tag�� �������� ���� ���, �� Ű�� �����´�.*/
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
