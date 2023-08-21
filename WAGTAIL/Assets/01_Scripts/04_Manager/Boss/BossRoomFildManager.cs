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
    public GameObject[] OddTile;
    public GameObject[] EvenTile;
    public int XSize;
    public int YSize;
    public int StoneXSize;
    public int StoneYSize;
    public Vector3 Offset;
    public float ShakeSpeed = 0.0f;
    
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

    public void BrokenPlatform(float XPos)
    {
        float X = (XPos - Offset.x ), Y = 0;
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
            for (int x = 0; x < XSize; x++)
            {
                spawnPos.Set(x * StoneXSize, -y * StoneYSize);
                // GameObject CreateTile = GameObject.Instantiate(Tiles[Random.Range(0, Tiles.Length)], this.transform);

                GameObject CreateTile;
                if ((x + y) % 2 == 0) CreateTile = GameObject.Instantiate(EvenTile[Random.Range(0, EvenTile.Length)], this.transform);
                else CreateTile = GameObject.Instantiate(OddTile[Random.Range(0, OddTile.Length)], this.transform);

                BossFild.Add(spawnPos, CreateTile);
                CreateTile.transform.localPosition = new Vector3(spawnPos.x, 0, spawnPos.y) + Offset;
                CreateTile.GetComponentInChildren<BrokenPlatform>().ShakeSpeed = ShakeSpeed;
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
