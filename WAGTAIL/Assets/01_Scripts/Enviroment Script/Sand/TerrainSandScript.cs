using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 *   터레인을 대상으로 모래가 침식하는 효과를 적용하는 컴포넌트입니다.
 * ***/

[RequireComponent(typeof(Terrain))]
[RequireComponent(typeof(TerrainCollider))]
public sealed class TerrainSandScript : SandScriptBase
{
    //=====================================================
    //////            Property and fields              ////
    //=====================================================
    public float IntakeRadius
    {
        get { return _intakeradius; }
        set
        {
            bool isChanged = (_intakeradius!=value);
            _intakeradius = (value<0f? 0f:value);

            /**값이 바뀌었을 경우*/
            if (isChanged == false) return;
            int radius = Mathf.RoundToInt(IntakeRadius);
            //_intakeMap = new float[radius, radius];
            _radiusDiv = (1f / _intakeradius);
        }
    }


    private Terrain     _terrain;
    private TerrainData _terrainData;

    private float[,]    _heightMap;
    private float[,]    _heightMapOrigin;
    private float[,]    _heightAlphaMap;

    private Vector3     _terrainWH;
    private Vector2Int  _terrainWHInt;
    private Vector3     _terrainSize;
    private Vector3     _terrainSizeDiv;

    private float _intakeradius = 10f;
    private float _radiusDiv    = 0f;



    //===========================================
    //////         Override methods         /////
    //===========================================
    protected override void OnSandAwake()
    {
        #region Omit
        /*****************************************************
         *   해당 터레인의 높이맵을 가져오는 초기화를 진행한다...
         * ***/
        _radiusDiv = (1f/_intakeradius);
        
        /**해당 터레인의 데이터의 복사본을 생성한다...*/
        _terrain = Terrain.activeTerrain;
        _terrain.terrainData = _terrainData = Instantiate(_terrain.terrainData);

        /**해당 터레인의 높이맵을 초기화한다.....*/
        _terrainWH       = (Vector3.one * _terrainData.heightmapResolution);
        _terrainWHInt    = (Vector2Int.one * _terrainData.heightmapResolution);
        _terrainSize     = _terrainData.size;
        _terrainSizeDiv  = new Vector3
        {
            x = (1f / _terrainSize.x),
            y = (1f / _terrainSize.y),
            z = (1f / _terrainSize.z)
        };

        _heightMapOrigin = _terrainData.GetHeights(0,0, _terrainWHInt.x, _terrainWHInt.y);
        _heightMap       = new float[_heightMapOrigin.GetLength(0), _heightMapOrigin.GetLength(1)];
        _heightAlphaMap  = new float[_heightMapOrigin.GetLength(0), _heightMapOrigin.GetLength(1)];

        Debug.Log($"WH: {_terrainWH}/ size: {_terrainSize}/ div: {_terrainSizeDiv}");

        #endregion
    }

    protected override void OnSandStart()
    {
        _SandMat = _terrain.materialTemplate;
    }

    protected override Vector3 GetWorldCenterPosition( Vector3 currCenter )
    {
        return currCenter;
    }

    protected override float SampleHeight(Vector3 worldPosition)
    {
        return _terrain.SampleHeight(worldPosition);
    }

    protected override void UpdateSandMesh( Vector3 currCenter )
    {
        #region Omit
        /******************************************
         *   계산에 필요한 요소들을 모두 구한다...
         * ***/
        currCenter.Scale(_terrainSizeDiv);

        Vector3Int center = new Vector3Int
        {
            x = Mathf.RoundToInt(_terrainWH.x * currCenter.x),
            y = 0,
            z = Mathf.RoundToInt(_terrainWH.z * currCenter.z)
        };

        _heightMapOrigin[center.x, center.z] = 1f;
        _terrainData.SetHeights(0, 0, _heightMapOrigin);

        //Vector3 center = SandIntakeCenterOffset;
        //center.Scale(_terrainSizeDiv);

        //int centerX = Mathf.RoundToInt(_terrainData.heightmapResolution * Mathf.Clamp01(center.x));
        //int centerZ = Mathf.RoundToInt(_terrainData.heightmapResolution * Mathf.Clamp01(center.z));

        //_heightMap[centerX, centerZ] = 1f;
        //_terrainData.SetHeights(0, 0, _heightMap);

        //Debug.Log($"size: {_heightMap.GetLength(0)}/ re: {_terrainData.heightmapResolution}/ ({centerX},{0},{centerZ}): {center}");

        //int   radius     = Mathf.RoundToInt(_intakeradius);
        //float radiusHalf = (_intakeradius * .5f);

        //int centerX     = Mathf.RoundToInt(_heightMap.GetLength(0)-1);
        //int centerZ     = Mathf.RoundToInt(_heightMap.GetLength(1)-1);
        //float centerY   = Mathf.FloorToInt(center.y * _terrainData.size.y);

        //_heightMap[137, 259] = 1f;
        //_terrainData.SetHeights(0, 0, _heightMap);
        #endregion
    }
}
