using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 *   �ͷ����� ������� �𷡰� ħ���ϴ� ȿ���� �����ϴ� ������Ʈ�Դϴ�.
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

            /**���� �ٲ���� ���*/
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
         *   �ش� �ͷ����� ���̸��� �������� �ʱ�ȭ�� �����Ѵ�...
         * ***/
        _radiusDiv = (1f/_intakeradius);
        
        _terrain = Terrain.activeTerrain;
        _terrain.terrainData = _terrainData = Instantiate(_terrain.terrainData);

        int radius = Mathf.RoundToInt(IntakeRadius);
        _intakeMap = new float[radius, radius];
        _heightMap = _terrainData.GetHeights(

            0,0,
            _terrainData.heightmapResolution,
            _terrainData.heightmapResolution
        );

        /**ũ�⿡ ���� ������ ����� �̸� ���Ѵ�....*/
        _terrainSizeDiv = new Vector3
        {
            x = (1f / _terrainData.size.x),
            y = 1f,
            z = (1f / _terrainData.size.z)
        };

        _defaultY = 0f;

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
         *   ��꿡 �ʿ��� ��ҵ��� ��� ���Ѵ�...
         * ***/
        Vector3 tPos   = transform.position;
        Vector3 center = (SandIntakeCenterOffset - tPos);

        int radius       = Mathf.RoundToInt(_intakeradius);
        float radiusHalf = (_intakeradius * .5f);

        int centerX = Mathf.RoundToInt(center.x + radiusHalf);
        int centerZ = Mathf.RoundToInt(center.z + radiusHalf);
        float centerY = (currCenter.y - _defaultY);

        Vector2 center2 = new Vector2(centerX, centerZ);

        /********************************************
         *   �߽����� �˸°� ������ũ ���� �����Ѵ�....
         * ***/
        for (int x = 0; x < radius; x++)
        {
            for (int z = 0; z < radius; z++){

                Vector2 currPos      = new Vector2(x, z);
                Vector2 center2Curr  = (currPos - center2);
                float outerRatio   = (1f - (center2Curr.magnitude * _radiusDiv));
                _intakeMap[x, z] = _defaultY;
            }
        }

        _defaultY += Time.deltaTime * .01f;
        Debug.Log($"defaultY: {_defaultY}");

        _terrainData.SetHeights(centerX, centerZ, _intakeMap);


        #endregion
    }
}
