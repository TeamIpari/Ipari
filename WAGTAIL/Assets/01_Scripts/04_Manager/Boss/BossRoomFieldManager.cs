using Cinemachine;
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
    
    public TileMap BossField;

    public Vector3 TargetPos;

    [Header("CameraShakeValue")]
    public float ShakePower = 1.5f;
    public float ShakeTime = 0.18f;
    public float EndShakePower = 5f;
    public float EndShakeTime = 2.5f;

    [Header("Shake ReAction Parameter")]
    public GameObject ReActionObject;
    
    private GameObject[] reActionPools;
    [SerializeField] private GameObject deathZone; 
    [SerializeField] private GameObject potal; 
    public float spawnDelay;
    public int Count;
    private bool reAction = false;
    private bool dontUpdate = false;


    public Vector3 PlayerOnTilePos
    {
        get { return TargetPos; }
        set { TargetPos = value; }
    }


    //=======================================
    //////        Magic Methods          ////
    //=======================================
    private void Awake()
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

    private void Start()
    {
        
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
        Vector3 pos = BossField[new Vector2(x, y)].transform.position;
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
        BossField[new Vector2(x, y)].GetComponentInChildren<IEnviroment>().ExecutionFunction(0);
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
        //Debug.Log("AA");
        CameraManager.GetInstance().CameraShake(0.5f, CameraManager.ShakeDir.ROTATE, 0.8f, 0.05f);
    }

    //======================================
    /////           public Methods     /////
    //======================================
    public Vector3 GetTilePos(int x, int y)
    {
        Vector3 vec = this.BossField[new Vector2(x * StoneXSize, y * (-StoneYSize))].transform.position;
        return new Vector3(vec.x, 13f, vec.z);
    }

    public void BreakingPlatform(float xPos, bool reAction = false)
    {
        // 내려 찍기 -> 2.5초 후 내려 찍음.
        // .Attack Delay = 2.5f
        int X = (int)(xPos - Offset.x ) + 9, Y = 0;
        int FindY = Y * (-StoneYSize);
        X = X <= 7 ? X - 1 : X;

        while (BossField.ContainsKey(new Vector2(X, FindY)))
        {
            StartCoroutine(BrokenDelayCo(X, FindY));
            Y++;
            FindY = Y * (-StoneYSize);
        }
        this.reAction = reAction;
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_VineSmash);

        if (this.reAction)
        {
            StartCoroutine(SpawnReActionObject());
            this.reAction = false;
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

                BossField.Add(spawnPos, CreateTile);
                CreateTile.transform.localPosition = new Vector3(spawnPos.x, 0, spawnPos.y) + Offset;
                CreateTile.GetComponentInChildren<BrokenPlatform>().ShakeSpeed = ShakeSpeed;
            }
        }
    }

    public void ResetField()
    {
        //StopAllCoroutines();
        foreach(var tile in BossField)
        {
            tile.Value.GetComponent<BrokenPlatform>().ReSetPlatform();
        }
    }

    public void EnableBrokenPlatformComponent(string nextScene = "")
    {
        foreach(var curTile in BossField)
        {
            curTile.Value.gameObject.GetComponentInChildren<IEnviroment>().ExecutionFunction(0.5f);
        }
        //CameraShake(EndShakePower, EndShakeTime);
        CameraManager.GetInstance().CameraShake(0.5f, CameraManager.ShakeDir.ROTATE, EndShakeTime, 0.05f);

        deathZone.SetActive(false);
        potal.SetActive(true);
    }

    public Transform GetSafetyTile()
    {
        BrokenPlatform bp;
        // 멀쩡한 플랫폼을 반환.
        do
        {
            bp = BossField.GetRandomValue<Vector2, GameObject>(BossField).GetComponent<BrokenPlatform>();
        } while (bp.isBroken);

        return bp.transform;
    }
}
