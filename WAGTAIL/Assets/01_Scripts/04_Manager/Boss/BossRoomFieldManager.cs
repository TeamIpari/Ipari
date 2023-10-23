using System.Collections;
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
    [SerializeField] private GameObject _interactionVFX;
    public GameObject DefaultTileMap;
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
    public float ShakeTime = 0.18f;
    public float EndShakePower = 5f;
    public float EndShakeTime = 2.5f;

    [Header("Shake ReAction Parameter")]
    public GameObject ReActionObject;
    
    private GameObject[] reActionPools;
    [SerializeField] private Deathzone deathZone; 
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
        DefaultTileMap.SetActive(false);
    }


    //======================================
    /////         Core Methods         /////
    //======================================


    //======================================
    /////           private Methods     /////
    //======================================
    private IEnumerator BrokenDelayCo(float x, float y)
    {
        yield return new WaitForSeconds(0.1f);
        //BossFild[new Vector2(x, y)].GetComponentInChildren<MovingPlatformBehavior>().OnObjectPlatformEnter(null, null, null, default, default);
        Vector3 pos = BossFild[new Vector2(x, y)].transform.position;
        Debug.Log("AA");

        GameObject obj = GameObject.Instantiate<GameObject>(_interactionVFX, new Vector3(pos.x, pos.y + 0.5f, pos.z - 1f), _interactionVFX.transform.rotation);
        Destroy(obj, 1);
    }

    private void SpawnVFX()
    {
        if (_interactionVFX != null)
        {
            GameObject exploVFX = Instantiate(_interactionVFX, gameObject.transform.position + Vector3.up * 0f, gameObject.transform.rotation);
            Destroy(exploVFX, 1);
        }

        else
            Debug.LogWarning("InteractionVFX was missing!");
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

        yield return new WaitForSeconds(0.1f);
        CameraShake(ShakePower, ShakeTime);
        if (reActionPools[0] == null)
            CreateReActionObject();
        
        foreach (var fruit in reActionPools)
        {
            x = Random.Range(1, XSize - 1);
            z = Random.Range(1, YSize - 1);

            fruit.SetActive(true);
            fruit.transform.position = GetTilePos(x, z);
            yield return new WaitForSeconds(spawnDelay * 0.01f);
        }

        yield return null;
    }

    private void CameraShake(float shakePower, float shakeTime )
    {
        CameraManager.GetInstance().CameraShake(shakePower, shakeTime);
    }

    //======================================
    /////           public Methods     /////
    //======================================
    public Vector3 GetTilePos(int x, int y)
    {
        Vector3 vec = this.BossFild[new Vector2(x * StoneXSize, y * (-StoneYSize))].transform.position;
        return new Vector3(vec.x, 13f, vec.z);
    }
    public void BreakingPlatform(float xPos, bool reAction = false)
    {
        // 내려 찍기 -> 2.5초 후 내려 찍음.
        // .Attack Delay = 2.5f
        int X = (int)(xPos - Offset.x ) + 9, Y = 0;
        int FindY = Y * (-StoneYSize);

        Debug.Log($"{X}, {FindY}");
        while (BossFild.ContainsKey(new Vector2(X, FindY)))
        {
            StartCoroutine(BrokenDelayCo(X, FindY));
            //BrokenDelay(X, FindY);
            Y++;
            FindY = Y * (-StoneYSize);
        }
        this.reAction = reAction;
        
        //Invoke("CameraShake", ShakeTiming);
        if (reAction)
        {
            StartCoroutine(SpawnReActionObject());
            reAction = false;
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
                CreateTile.GetComponentInChildren<BrokenPlatform>().ShakeSpeed = ShakeSpeed;
            }
        }
    }

    public void EnableBrokenPlatformComponent(string nextScene = "")
    {
        foreach(var curTile in BossFild)
        {
            //curTile.Value.gameObject.GetComponentInChildren<PlatformObject>().enabled = false;
            //curTile.Value.gameObject.GetComponentInChildren<BrokenPlatform>().enabled = false;
            curTile.Value.gameObject.GetComponentInChildren<IEnviroment>().ExecutionFunction(0.5f);
            //curTile.Value.gameObject.GetComponentInChildren<BrokenPlatform>().HideOnly(false);
        }
        CameraShake(EndShakePower, EndShakeTime);

        deathZone.enabled = false;
        Potal pt = deathZone.GetComponent<Potal>();
        pt.enabled = true;
        pt.nextChapterName = nextScene;
    }
}
