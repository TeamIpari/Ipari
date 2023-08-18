using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossRoomFildManager :MonoBehaviour
{
    // 기본 인스텐스를 사용
    private static BossRoomFildManager instance;
    public static BossRoomFildManager Instance
    {
        get { return instance; }
    }


    [System.Serializable]
    public class TileMap : SerializableDictionary<Vector2, GameObject >
    {

    }

    // 생성할 타일을 저장함.
    public GameObject[] Tiles;
    public int TwitterSize;
    public int YSize;
    public int StoneTwitterSize;
    public int StoneYSize;
    public Vector3 Offset;
    
    public TileMap BossFild;

    public Vector3 TargetPos;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 돌들을 배치
        Initialized();
    }

    public void BrokenPlatform(float TwitterPos)
    {
        float X = (TwitterPos - Offset.x ), Y = 0;
        float FindY = Y * (-StoneYSize);
        while (BossFild.ContainsKey(new Vector2(X, FindY)))
        {
            BossFild[new Vector2(X, FindY)].GetComponentInChildren<IEnviroment>().ExecutionFunction(2.5f);
            Y++;
            FindY = Y * (-StoneYSize);
        }
    }

    public Vector3 PlayerOnTilePos
    {
        get { return TargetPos; }
        set { TargetPos = value; }

    }

    public void Initialized()
    {
        Vector2 spawnPos = Vector2.zero;
        for (int y = 0; y < YSize; y++)
        {
            for (int x = 0; x < TwitterSize; x++)
            {
                spawnPos.Set(x * StoneTwitterSize, -y * StoneYSize);
                GameObject CreateTile = GameObject.Instantiate(Tiles[Random.Range(0, Tiles.Length)], this.transform);
                BossFild.Add(spawnPos, CreateTile);
                CreateTile.transform.localPosition = new Vector3(spawnPos.x, 0, spawnPos.y) + Offset;
            }
        }
    }

    public void EnableBrokenPlatformComponent()
    {
        foreach(var curTile in BossFild)
        {
            curTile.Value.gameObject.GetComponentInChildren<BrokenPlatform>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
