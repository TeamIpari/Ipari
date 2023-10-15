using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/************************************************************
 *   원형 메시에 모래가 침식하는 효과를 적용하는 컴포넌트입니다.
 * ***/
public sealed class CircleSandScript : SandScriptBase
{
    private struct VertexSample
    {
        public int index;
        public int overlapCount;
    }

    //======================================================
    /////            Property and Fields               /////
    //======================================================
    private MeshCollider _meshCollider;
    private MeshRenderer _renderer;
    private MeshFilter   _filter;

    private Mesh        _mesh;
    private Vector3[]   _vertices;
    private int         _centerIndex = 0;

    private Vector3     _startCenter    = Vector3.zero;
    private Vector3     _centerScaleDiv = Vector3.one;



    //=============================================
    /////          Override methods           /////
    //=============================================
    protected override void OnSandStart()
    {
        #region Omit
        _meshCollider = GetComponent<MeshCollider>();
        _renderer     = GetComponent<MeshRenderer>();
        _filter       = GetComponent<MeshFilter>();

        /**메시를 가져오고 중점을 찾는다....*/
        if(_filter && _filter.mesh!=null){

            _mesh           = new Mesh { name = "SandMesh(Clone)" };
            _mesh.vertices  = _vertices = (Vector3[])_filter.sharedMesh.vertices.Clone();
            _mesh.triangles = (int[])_filter.mesh.triangles.Clone(); 
            _mesh.uv        = (Vector2[])_filter.mesh.uv.Clone();
            _centerIndex    = FindCenterVertex(_mesh.triangles);

            Vector3 sandScale = transform.localScale;
            _startCenter      = _mesh.vertices[_centerIndex];
            _centerScaleDiv   = new Vector3()
            {
                x = (1f / sandScale.x),
                y = (1f / sandScale.y),
                z = (1f / sandScale.z),
            };
        }

        /**Renderer로부터 Material을 가져온다...*/
        if(_renderer!=null){

            _SandMat = _renderer.material;
        }
        #endregion
    }

    protected override void OnDrawSandGizmos()
    {
        #region Omit
        if (_filter==null){

            _filter = GetComponent<MeshFilter>();
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(_filter.sharedMesh, transform.position, transform.rotation, transform.localScale);
        #endregion
    }

    protected override void UpdateSandMesh(Vector3 currCenterOffset)
    {
        #region Omit
        if (_mesh == null) return;

        currCenterOffset.Scale(_centerScaleDiv);
        _vertices[_centerIndex] = (_startCenter + currCenterOffset);
        _mesh.vertices          = _vertices;

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _filter.mesh             = _mesh;
        _meshCollider.sharedMesh = _mesh;
        #endregion
    }



    //============================================
    //////          Utilty methods          /////
    //===========================================
    private int FindCenterVertex(int[] indices)
    {
        #region Omit
        if (indices.Length < 3) return 0;

        VertexSample s1 = new VertexSample { index = indices[0],  overlapCount = 0 };
        VertexSample s2 = new VertexSample { index = indices[1],  overlapCount = 0 };
        VertexSample s3 = new VertexSample { index = indices[2],  overlapCount = 0 };

        /*********************************************
         *   가장 많은 참조수를 가지는 정점을 반환한다...
         * ***/
        int Count = indices.Length; 
        for(int i=3; i<Count; i+=3)
        {
            WriteAndRaiseCountSample(indices[i],   ref s1, ref s2, ref s3);
            WriteAndRaiseCountSample(indices[i+1], ref s1, ref s2, ref s3);
            WriteAndRaiseCountSample(indices[i+2], ref s1, ref s2, ref s3);
        }

        return GetMaxCountSample(s1, s2, s3);
        #endregion
    }

    private void WriteAndRaiseCountSample(int vertex, ref VertexSample s1, ref VertexSample s2, ref VertexSample s3)
    {
        #region Omit
        if (s1.index == vertex) s1.overlapCount++;
        else if (s2.index == vertex) s2.overlapCount++;
        else if(s3.index== vertex) s3.overlapCount++;
        #endregion
    }

    private int GetMaxCountSample( params VertexSample[] samples )
    {
        #region Omit
        int count = 0;
        int index = 0;

        for(int i=2; i>=0; i--)
        {
            ref VertexSample sample = ref samples[i];   

            /**가장 큰놈만 걸러낸다...*/
            if(count<sample.overlapCount){

                count = sample.overlapCount;
                index = i;
            }
        }

        return index;
        #endregion
    }

}
