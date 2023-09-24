using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("CameraShakeValue")]
    public float ShakePower = 1.5f;
    public float ShakeTime = 0.25f;
    public float ShakeTiming = 2.5f;

    [Header("Shake ReAction Parameter")]
    public GameObject ReActionObject;
    private GameObject[] reActionPools;
    public float spawnDelay;
    public int Count;
    private bool reAction = false;

    public Vector3 PlayerOnTilePos
    {
        get { return TargetPos; }
        set { TargetPos = value; }
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
        CreateReActionObject();
    }


    //======================================
    /////         Core Methods         /////
    //======================================


    //======================================
    /////           private Methods     /////
    //======================================
    private IEnumerator BrokenDelayCo(float x, float y)
    {
        yield return new WaitForSeconds(2.5f);
        //BossFild[new Vector2(x, y)].GetComponentInChildren<MovingPlatformBehavior>().OnObjectPlatformEnter(null, null, null, default, default);
        BossFild[new Vector2(x, y)].GetComponentInChildren<IEnviroment>().ExecutionFunction(0);
    }

    private void BrokenDelay(float x, float y)
    {
        BossFild[new Vector2(x, y)].GetComponentInChildren<IEnviroment>().ExecutionFunction(0);
    }

    private void CreateReActionObject()
    {
        reActionPools = new GameObject[Count];

        // 생성 하는 기능.
        for (int i = 0; i < Count; i++)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(ReActionObject);
            obj.transform.position = Vector3.zero;
            reActionPools[i] = obj;
            reActionPools[i].SetActive(false);
        }
    }

    private IEnumerator SpawnReActionObject()
    {
        int x, z;
        if (reActionPools[0] == null)
            CreateReActionObject();
        foreach (var fruit in reActionPools)
        {
            x = Random.Range(0, BossRoomFieldManager.Instance.XSize);
            z = Random.Range(0, BossRoomFieldManager.Instance.YSize);

            fruit.SetActive(true);
            fruit.transform.position = BossRoomFieldManager.Instance.GetTilePos(x, z);
            yield return new WaitForSeconds(spawnDelay * 0.001f);
        }

        yield return null;
    }

    private void CameraShake()
    {
        if (reAction)
        {
            StartCoroutine(SpawnReActionObject());
            reAction = false;
        }
        CameraManager.GetInstance().CameraShake(ShakePower, ShakeTime);
    }

    //======================================
    /////           public Methods     /////
    //======================================
    public Vector3 GetTilePos(int x, int y)
    {
        Vector3 vec = this.BossFild[new Vector2(x * StoneXSize, y * (-StoneYSize))].transform.position;
        return new Vector3(vec.x, 5f, vec.z);
    }
    public void BrokenPlatform(float xPos, bool reAction = false)
    {
        // 내려 찍기 -> 2.5초 후 내려 찍음.
        // .Attack Delay = 2.5f
        float X = (xPos - Offset.x ), Y = 0;
        float FindY = Y * (-StoneYSize);
        
        while (BossFild.ContainsKey(new Vector2(X, FindY)))
        {
            StartCoroutine(BrokenDelayCo(X, FindY));
            //BrokenDelay(X, FindY);
            Y++;
            FindY = Y * (-StoneYSize);
        }
        this.reAction = reAction;
        
        Invoke("CameraShake", ShakeTiming);
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
                CreateTile.GetComponentInChildren<BrokenPlatform>().ShakeSpeed = ShakeSpeed;
            }
        }
    }

    public void EnableBrokenPlatformComponent()
    {
        foreach(var curTile in BossFild)
        {
            //curTile.Value.gameObject.GetComponentInChildren<PlatformObject>().enabled = false;
            curTile.Value.gameObject.GetComponentInChildren<BrokenPlatform>().enabled = false;
        }
    }
}
