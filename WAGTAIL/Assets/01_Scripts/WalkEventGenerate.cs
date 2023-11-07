using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**임시 테스트용.....*/
public sealed class WalkEventGenerate : MonoBehaviour
{
    private static FModSFXEventType[] sfxLists = new FModSFXEventType[]
    {
        FModSFXEventType.Player_Landed,
        FModSFXEventType.Player_Hit,
        FModSFXEventType.Player_Dead,
        FModSFXEventType.Player_Walk,
        FModSFXEventType.Broken,
        FModSFXEventType.Crab_Smash,
        FModSFXEventType.Mushroom_Jump,
        FModSFXEventType.Put_KoKoShi,
        FModSFXEventType.Get_Bead
    };

    private float delay = 0f;

    //==============================================
    //////          Magic methods             //////
    //==============================================
    private void Update()
    {
        #region
        if ((delay -= Time.deltaTime) > 0f) return;

        RaycastHit hit;
        bool isHit = IpariUtility.GetPlayerFloorinfo(
            out hit, 
            ~(1<<LayerMask.NameToLayer("Player")),
            (Vector3.up*1f),
            1f
        );

        if(isHit)
        {
            delay = 3f;
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain == null) return;

            int layer = GetLayer(ConvertPosition(transform.position, terrain), terrain);
            layer = System.Math.Clamp(layer, 0, sfxLists.Length - 1);

            Test(terrain);
            Debug.Log($"layer: {layer}({sfxLists[layer]})");
            FModAudioManager.SetBusVolume(FModBusType.Master, 1f);
            FModAudioManager.PlayOneShotSFX(sfxLists[layer]);
        }
        #endregion
    }

    Vector2 ConvertPosition(Vector3 pos, Terrain terrainObject)
    {
        #region Omit
        Vector3 terrainPosition = pos - terrainObject.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / terrainObject.terrainData.size.x, 0,
        terrainPosition.z / terrainObject.terrainData.size.z);

        float xCoord = Mathf.Clamp(mapPosition.x * terrainObject.terrainData.alphamapWidth, 0, terrainObject.terrainData.alphamapWidth - 1);
        float zCoord = Mathf.Clamp(mapPosition.z * terrainObject.terrainData.alphamapHeight, 0, terrainObject.terrainData.alphamapHeight - 1);

        return new Vector3((int)xCoord, (int)zCoord);
        #endregion
    }

    private void Test(Terrain terrain)
    {
        #region Omit
        TerrainLayer[] layers = terrain.terrainData.terrainLayers;

        int Count = layers.Length;
        for(int i=0; i<Count; i++)
        {
            Debug.Log($"({i}): {layers[i].name}");
        }


        #endregion
    }

    int GetLayer(Vector2 position, Terrain terrainObject)
    {
        #region Omit
        float[,,] aMap = terrainObject.terrainData.GetAlphamaps((int)position.x, (int)position.y, 1, 1);
        int tLayer = 0;
        float lastHighest = 0;
        for (int x = 0; x < aMap.GetLength(0); x++)
        {
            for (int y = 0; y < aMap.GetLength(1); y++)
            {
                for (int z = 0; z < aMap.GetLength(2); z++)
                {
                    if (aMap[x, y, z] > lastHighest)
                    {
                        lastHighest = aMap[x, y, z];
                        tLayer = z;
                    }
                }
            }
        }
        return tLayer;
        #endregion
    }



}
