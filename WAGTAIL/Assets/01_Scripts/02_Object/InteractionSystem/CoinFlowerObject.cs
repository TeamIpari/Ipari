using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************************************
 *   �÷��̾ ��� ������ �浹�� �Ͼ�� ������ ������ ������Ʈ�Դϴ�.
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
        /**�÷��̾� �̿��� �ݶ��̴��� �浹���� ���....*/
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
         *   ���� �������� �� �ټ��� ������ �����Ѵ�....
         * ***/
        if (CoinPrefab != null){

            Vector3       flowerPos = transform.position;
            ScoreObject[] coins     = new ScoreObject[_CoinSpawnCount];    

            for (int i = 0; i < CoinSpawnCount; i++)
            {
                /**���ο� ������ ������, �ڼ�ȿ���� Ƣ������� ���ӵ��� �ش�...*/
                ScoreObject newCoin   = GameObject.Instantiate(CoinPrefab).GetComponent<ScoreObject>();
                Transform   coinTr    = newCoin.transform;

                newCoin.UseRigidbody(false,true);
                newCoin.ItemGetType         = ScoreObject.GetKind.RaiseUp;
                newCoin.UseMagnetMovement   = UseCoinMagnet;
                newCoin.MagnetMoveDelayTime = 1.3f;

                Vector3 flyPow = Random.onUnitSphere;
                flyPow.y       = Random.Range(2f, 3f);
                newCoin.Body.velocity = (flyPow * CoinExplodePower);

                /**������ ��ġ����...*/
                Vector3 coinPos     = flowerPos;
                Quaternion coinQuat = Quaternion.Euler(90f, 0f, 0f);
                coinTr.SetPositionAndRotation(coinPos, coinQuat);

                coins[i] = newCoin;
            }

            /**���� �浹�� �����ϵ��� �Ѵ�...*/
            for(int i=0; i<CoinSpawnCount; i++){

                IgnoreOtherCoins(coins[i], coins);
            }

        }



        /************************************************
         *    ���� �������� ���� ����Ʈ�� �����Ѵ�....
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
