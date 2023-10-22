using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************************************
 *   플레이어가 들고 던지고 충돌이 일어나면 코인이 나오는 컴포넌트입니다.
 * ****/
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ThrowObject))]
public sealed class CoinFlowerObject : MonoBehaviour
{
    //================================================
    ////                 Property                 ////
    //================================================
    public int   CoinSpawnCount  { get { return _CoinSpawnCount; } set { _CoinSpawnCount = (value<0?0:value); } }
    public float CoinFlightTime  { get { return _CoinFlightTime; } set { _CoinFlightTime = (value < 0f ? 0f : value); } }
    public float CoinSpawnRadian { get { return _CoinSpawnRadian; } set { _CoinSpawnRadian = (value < 0f ? 0f : value); } }
    public float CoinExplodePower{ get { return _CoinExpldePower; } set { _CoinExpldePower = (value < 0f ? 0f : value); } }
    public bool  IsExplosion     { get; private set; } = false;

    [SerializeField] public  GameObject  CoinPrefab;
    [SerializeField] public  GameObject  ExplodeSFX;
    [SerializeField] public bool         ExplodeCollision   = true;
    [SerializeField] public  bool        UseCoinMagnet      = false;
    [SerializeField] public  string      FlowerExplodeAnim  = string.Empty;
    [SerializeField] private float       _CoinFlightTime    = 2f;
    [SerializeField] private float       _CoinSpawnRadian   = 1f;
    [SerializeField] private float       _CoinExpldePower = 1f;
    [SerializeField] private int         _CoinSpawnCount   = 3;


    
    //===============================================
    /////                Fields                 /////
    //===============================================
    private ThrowObject _throwObj;
    private Animator    _animator;
    private bool        _pickUp = false;



    //===============================================
    //////            Magic methods            //////
    //===============================================
    private void Start()
    {
        #region Omit
        _animator = GetComponent<Animator>();
        _throwObj = GetComponent<ThrowObject>();
        _throwObj.OnThrow.AddListener(() => { _pickUp = true; });
        #endregion
    }

    private void OnCollisionEnter(Collision other)
    {
        #region Omit
        /**플레이어 이외의 콜라이더와 충돌했을 경우....*/
        if (_pickUp && !IsExplosion && !other.gameObject.CompareTag("Player"))
        {
            if (ExplodeCollision) ExplostionFlower();

            if(_animator!=null) _animator.Play("");
            IsExplosion = true;
        }
        #endregion
    }



    //==================================================
    ///////              Core methods             //////
    //==================================================
    private Vector3 Search()
    {
        #region Omit
        float r = Random.Range(0.0f, CoinSpawnRadian);

        Vector3 getPoint = Random.onUnitSphere;
        getPoint.y = 0.1f;

        Vector3 vec = (getPoint * r) + transform.position;

        return new Vector3(vec.x, 0.1f, vec.z);
        #endregion
    }

    private void IgnoreOtherCoins(ScoreObject target, params ScoreObject[] others)
    {
        #region Omit
        int Count = others.Length;
        for(int i=0; i<Count; i++ )
        {
            if (others[i] == target) continue;
            Physics.IgnoreCollision(target.Collider, others[i].Collider);
        }

        #endregion
    }

    public void ExplostionFlower()
    {
        #region Omit
        if (CoinPrefab == null) return;

        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Flowers_Burst,
            transform.position,
            2f
        );

        /********************************************
         *   꽃이 펼쳐졌을 때 다수의 코인을 생성한다....
         * ***/
        if (CoinPrefab != null){

            Vector3       flowerPos = transform.position;
            ScoreObject[] coins     = new ScoreObject[_CoinSpawnCount];    

            for (int i = 0; i < CoinSpawnCount; i++)
            {
                /**새로운 코인을 생성후, 자석효과와 튀어오르는 가속도를 준다...*/
                ScoreObject newCoin   = GameObject.Instantiate(CoinPrefab).GetComponent<ScoreObject>();
                Transform   coinTr    = newCoin.transform;

                newCoin.UseRigidbody(false,true);
                newCoin.ItemGetType         = ScoreObject.GetKind.RaiseUp;
                newCoin.UseMagnetMovement   = UseCoinMagnet;
                newCoin.MagnetMoveDelayTime = 1.3f;

                Vector3 flyPow = Random.onUnitSphere;
                flyPow.y       = Random.Range(2f, 3f);
                newCoin.Body.velocity = (flyPow * CoinExplodePower);

                /**코인의 위치지정...*/
                Vector3 coinPos     = flowerPos;
                Quaternion coinQuat = Quaternion.Euler(90f, 0f, 0f);
                coinTr.SetPositionAndRotation(coinPos, coinQuat);

                coins[i] = newCoin;
            }

            /**각각 충돌을 무시하도록 한다...*/
            for(int i=0; i<CoinSpawnCount; i++){

                IgnoreOtherCoins(coins[i], coins);
            }

        }



        /************************************************
         *    꽃이 펼쳐졌을 때의 이펙트를 생성한다....
         * ****/
        if (ExplodeSFX!= null)
        {
            GameObject foldoutVFX = Instantiate(
                ExplodeSFX, 
                transform.position, 
                transform.rotation
            );

            Destroy(foldoutVFX, 7);
        }

        Destroy(gameObject);
        #endregion
    }
}
