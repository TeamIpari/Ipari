using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   ���ڳ� ũ�� ������ ������ ������Ʈ�Դϴ�.
 * ***/
public sealed class BossCrab : Enemy
{
    #region Editor_Extension
#if UNITY_EDITOR
    [CustomEditor(typeof(BossCrab))]
    private sealed class BossCrabEditor : Editor
    {
        //===================================
        //////          Fields          /////
        //===================================
        private SerializedProperty SeedCountProperty;
        private SerializedProperty SeedPrefabProperty;
        private SerializedProperty CrabHandProperty;

        private static GUIStyle BoldLabelStyle;
        private static GUIStyle BoldFoldStyle;

        private bool _foldout = true;


        //==============================================
        //////         Override methods           //////
        //==============================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();  


            /***********************************
             *   ��� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            GUI_Intialized();

            GUI_ShowPrefabs();


            /**��������� �ִٸ� ���� �����Ѵ�...*/
            if(GUI.changed){

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
            if(SeedCountProperty==null){

                SeedCountProperty = serializedObject.FindProperty("BombCount");
            }

            if(SeedPrefabProperty==null)
            {
                SeedPrefabProperty = serializedObject.FindProperty("SeedPrefab");
            }

            if(CrabHandProperty==null){

                CrabHandProperty = serializedObject.FindProperty("CrabHandPrefab");
            }


            /*****************************************
             *    ��� ��Ÿ�ϵ��� �ʱ�ȭ�Ѵ�...
             * ***/
            if(BoldLabelStyle==null)
            {
                BoldLabelStyle = new GUIStyle(GUI.skin.label);
                BoldLabelStyle.fontStyle= FontStyle.Bold;   
            }

            if(BoldFoldStyle==null)
            {
                BoldFoldStyle = new GUIStyle(EditorStyles.foldout);
                BoldFoldStyle.fontStyle= FontStyle.Bold;    
            }

            #endregion
        }

        private void GUI_ShowPrefabs()
        {
            #region Omit
            if (SeedPrefabProperty == null || CrabHandProperty == null) return;

            /***********************************************
             *   �����յ��� ����� ǥ�� ���ο� ���� ǥ���Ѵ�..
             * ***/
            if(_foldout=EditorGUILayout.BeginFoldoutHeaderGroup(_foldout, "Prefabs", BoldFoldStyle))
            {
                SeedPrefabProperty.objectReferenceValue = EditorGUILayout.ObjectField(
                    "Seed Prefab", 
                    SeedPrefabProperty.objectReferenceValue,
                    typeof(GameObject), 
                    true
                );

                CrabHandProperty.objectReferenceValue = EditorGUILayout.ObjectField(
                    "Crab Hand Prefab",
                    CrabHandProperty.objectReferenceValue,
                    typeof(GameObject),
                    true
                );

            }
            #endregion
        }

    }
#endif
    #endregion

    //===========================================
    //////          Property                /////
    //===========================================
    [SerializeField][Tooltip("�����Ǵ� ������ ����.")]
    public int         SeedCount = 10;

    [Header("Prefabs")]

    [SerializeField][Tooltip("������ ������ ������ ��Ÿ���� ������.")]
    public GameObject   SeedPrefab;

    [SerializeField][Tooltip("������ �������� ������ ��Ÿ���� ������.")]
    public GameObject   CrabHandPrefab;



    //==========================================
    //////              Fields              ////
    //==========================================




    //===============================================
    //////          Magic methods               /////
    //===============================================
    private void Awake()
    {
        
    }



    //============================================
    ////////         Core methods           //////
    //============================================
    private void Pattern_Initialized()
    {
        #region Omit

        

        #endregion
    }



}
