using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/************************************************
 *  주어진 메시를 점을 나열하여 늘리는 컴포넌트입니다...
 * ***/
[AddComponentMenu("IpariUtility/SplineComponent")]
public sealed class SplineMesh : MonoBehaviour
{
    public struct SplinePoint
    {
        public Vector3 Point;
        
    }

    #region Editor_Extension
#if UNITY_EDITOR
    [CustomEditor(typeof(SplineMesh))]
    private sealed class SplineComponentEditor : Editor
    {
        //====================================
        ////          Property             ///
        //====================================
        SerializedProperty SplinePointsProperty;
        SerializedProperty SplinePointsCountProperty;
        SerializedProperty MeshProperty;



        //===========================================
        /////        Override methods           /////
        //===========================================
        public override void OnInspectorGUI()
        {

        }



        //=======================================
        /////          GUI methods          /////
        //=======================================
        private void GUI_Initialized()
        {
            #region Omit
            /**********************************
             *   모든 프로퍼티들을 초기화한다...
             * ***/
            if(SplinePointsCountProperty==null){

                SplinePointsProperty = serializedObject.FindProperty("_splinePoints");
            }

            if(SplinePointsCountProperty==null){

                SplinePointsCountProperty = serializedObject.FindProperty("_splinePointsCount");
            }

            if(MeshProperty==null){

                MeshProperty = serializedObject.FindProperty("SplineMesh");
            }
            #endregion
        }



    }
#endif
    #endregion

    //============================================
    /////       Property and fields           ////
    //============================================
    public Vector3[] SplinePoints { get { return _splinePoints; } }

    [SerializeField] Mesh      Mesh;
    [SerializeField] Vector3[] _splinePoints;
    [SerializeField] int       _splinePointsCount = 0;



    //============================================
    /////           Magic methods             ////
    //============================================
    private void Start()
    {
        #region Omit
        /**스플라인 정점들이 존재하지 않으면 할당..*/
        if (_splinePoints==null){

            _splinePoints      = new Vector3[10];
            _splinePointsCount = _splinePoints.Length;
        }
        #endregion
    }

    private void OnDrawGizmosSelected()
    {
        
    }



}
