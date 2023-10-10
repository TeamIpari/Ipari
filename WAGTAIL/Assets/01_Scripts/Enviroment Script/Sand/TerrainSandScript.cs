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
            _intakeMap = new float[radius, radius];
            _radiusDiv = (1f / _intakeradius);
        }
    }


    private Terrain     _terrain;
    private TerrainData _terrainData;
    private float[,]    _heightMap;
    private float[,]    _intakeMap;
    private Vector3     _terrainSizeDiv;

    private float _defaultY     = 0f;
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

        /**터레인의 침식/깊이맵을 구한다....*/
        int radius = Mathf.RoundToInt(IntakeRadius);
        _intakeMap = new float[radius, radius];
        _heightMap = _terrainData.GetHeights(

            0,0,
            _terrainData.heightmapResolution,
            _terrainData.heightmapResolution
        );

        /**크기에 대한 나눗셈 계산을 미리 구한다....*/
        _terrainSizeDiv = new Vector3
        {
            x = (1f / _terrainData.size.x),
            y = (1f / _terrainData.size.y),
            z = (1f / _terrainData.size.z)
        };

        _defaultY = _terrain.SampleHeight(SandIntakeCenterOffset);

        #endregion
    }

    protected override void OnSandStart()
    {
        SandMat = _terrain.materialTemplate;
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
        Vector3 tPos   = transform.position;
        Vector3 center = (currCenter - tPos);
        center.Scale(_terrainSizeDiv);

        int radius       = Mathf.RoundToInt(_intakeradius);
        float radiusHalf = (_intakeradius * .5f);

        int centerX = Mathf.FloorToInt(center.x * _terrainData.size.x);
        int centerZ = Mathf.FloorToInt(center.z * _terrainData.size.z);
        float centerY = Mathf.FloorToInt(center.y * _terrainData.size.y);

        Vector2 center2 = new Vector2(centerX+radiusHalf, centerZ+radiusHalf);


        /********************************************
         *   중심점에 알맞게 인테이크 맵을 갱신한다....
         * ***/
        //for (int x = 0; x < radius; x++)
        //{
        //    for (int z = 0; z < radius; z++){

        //        Vector2 currPos      = new Vector2(x, z);
        //        Vector2 center2Curr  = (currPos - center2);
        //        float outerRatio     = (1f - (center2Curr.magnitude * _radiusDiv));
        //        _intakeMap[x, z]     = centerY;
        //    }
        //}
        _intakeMap[1, 1] = 1f;

        _defaultY += Time.deltaTime * .01f;
        Debug.Log($"defaultY: {centerY}");

        _terrainData.SetHeights(centerX, centerZ, _intakeMap);


        #endregion
    }
}
