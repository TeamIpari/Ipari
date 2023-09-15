using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/************************************************
 *  �־��� �޽ø� ���� �����Ͽ� �ø��� ������Ʈ�Դϴ�...
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
             *   ��� ������Ƽ���� �ʱ�ȭ�Ѵ�...
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
        /**���ö��� �������� �������� ������ �Ҵ�..*/
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
