using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    Terrain _t;

    private void Start()
    {
        _t = GetComponent<Terrain>();
        TerrainData data  = _t.terrainData;

        Transform playerTr = Player.Instance.transform;
        Vector2 pos = new Vector2(playerTr.position.x, playerTr.position.z);

        float[,] h = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);
        h[100, 100] = 100f;
        data.SetHeights(0, 0, h);
    }


}
