using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   코코넛 크랩 보스가 구현된 컴포넌트입니다.
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
             *   모든 프로퍼티들을 표시한다...
             * ***/
            GUI_Intialized();

            GUI_ShowPrefabs();


            /**변경사항이 있다면 값을 갱신한다...*/
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
             *    모든 프로퍼티들을 초기화한다...
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
             *    모든 스타일들을 초기화한다...
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
             *   프리팹들의 목록을 표시 여부에 따라서 표시한다..
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
    [SerializeField][Tooltip("생성되는 씨앗의 개수.")]
    public int         SeedCount = 10;

    [Header("Prefabs")]

    [SerializeField][Tooltip("생성할 씨앗의 원본을 나타내는 프리팹.")]
    public GameObject   SeedPrefab;

    [SerializeField][Tooltip("생성할 집게팔의 원본을 나타내는 프리팹.")]
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
