using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputManagerEntry;


public class BossRoomFieldManager :MonoBehaviour
{

    [System.Serializable]
    public class TileMap : SerializableDictionary<Vector2, GameObject >
    {

    }

    //========================================
    //////      Property And Fields      /////
    //========================================

    // 기본 인스텐스를 사용
    private static BossRoomFieldManager instance;
    public static BossRoomFieldManager Instance
    {
        get { return instance; }
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
    public Vector3 PlayerOnTilePos
    {
        get { return TargetPos; }
        set { TargetPos = value;
            Debug.Log($"Pos{TargetPos}");
        }
    }


    //=======================================
    //////        Magic Methods          ////
    //=======================================
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 돌들을 배치
        Initialized();
    }

    //======================================
    /////           private Methods     /////
    //======================================
    private IEnumerator BrokenDelay(float x, float y)
    {
        yield return new WaitForSeconds(2.5f);
        BossFild[new Vector2(x, y)].GetComponentInChildren<IEnviroment>().ExecutionFunction(0);
    }

    //======================================
    /////         Core Methods         /////
    //======================================

    public Vector3 GetTilePos(int x, int y)
    {
        Vector3 vec = this.BossFild[new Vector2(x * StoneXSize, y * (-StoneYSize))].transform.position;
        return new Vector3(vec.x, 5f, vec.z);
    }
    public void BrokenPlatform(float XPos)
    {
        // 내려 찍기 -> 2.5초 후 내려 찍음.
        // .Attack Delay = 2.5f
        float X = (XPos - Offset.x ), Y = 0;
        float FindY = Y * (-StoneYSize);
        while (BossFild.ContainsKey(new Vector2(X, FindY)))
        {
            StartCoroutine(BrokenDelay(X, FindY));
            Y++;
            FindY = Y * (-StoneYSize);
        }
    }

    public void Initialized()
    {
        Vector2 spawnPos = Vector2.zero;
        for (int y = 0; y < YSize; y++)
        {
            for (int x = 0; x < XSize; x++)
            {
                spawnPos.Set(x * StoneXSize, -y * StoneYSize);
                GameObject CreateTile;
                if ((x + y) % 2 == 0) CreateTile = GameObject.Instantiate(EvenTile[Random.Range(0, EvenTile.Length)], this.transform);
                else CreateTile = GameObject.Instantiate(OddTile[Random.Range(0, OddTile.Length)], this.transform);

                BossFild.Add(spawnPos, CreateTile);
                CreateTile.transform.localPosition = new Vector3(spawnPos.x, 0, spawnPos.y) + Offset;
                //CreateTile.GetComponentInChildren<BrokenPlatform>().ShakeSpeed = ShakeSpeed;
            }
        }
    }

    public void EnableBrokenPlatformComponent()
    {
        foreach(var curTile in BossFild)
        {
            curTile.Value.gameObject.GetComponentInChildren<PlatformObject>().enabled = false;
        }
    }
}
