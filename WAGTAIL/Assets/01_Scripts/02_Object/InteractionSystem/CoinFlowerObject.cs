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
    public int   CointSpawnCount { get { return _CointSpawnCount; } set { _CointSpawnCount = (value<0?0:value); } }
    public float CoinFlightTime  { get { return _CoinFlightTime; } set { _CoinFlightTime = (value < 0f ? 0f : value); } }
    public float CoinSpawnRadian { get { return _CoinSpawnRadian; } set { _CoinSpawnRadian = (value < 0f ? 0f : value); } }
    public bool  IsExplosion     { get; private set; } = false;

    [SerializeField] public  GameObject  CoinPrefab;
    [SerializeField] public  GameObject  ExplodeSFX;
    [SerializeField] public  bool        ExplodeCollision   = false;
    [SerializeField] public  bool        UseCoinMagnet      = false;
    [SerializeField] public string       FlowerExplodeAnim  = string.Empty;
    [SerializeField] private float       _CoinFlightTime    = 2f;
    [SerializeField] private float       _CoinSpawnRadian   = 1f;
    [SerializeField] private int         _CointSpawnCount   = 3;


    
    //===============================================
    /////                Fields                 /////
    //===============================================
    private ThrowObject _throwObj;
    private Animator    _animator;



    //===============================================
    //////            Magic methods            //////
    //===============================================
    private void Start()
    {
        #region Omit
        _animator = GetComponent<Animator>();
        _throwObj = GetComponent<ThrowObject>();
        #endregion
    }

    private void OnCollisionEnter(Collision other)
    {
        #region Omit
        /**�÷��̾� �̿��� �ݶ��̴��� �浹���� ���....*/
        if (!IsExplosion && !other.gameObject.CompareTag("Player"))
        {
            if (ExplodeCollision) ExplostionFlower();

            _animator?.Play("");
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

            Vector3 flowerPos = transform.position;
            for (int i = 0; i < CointSpawnCount; i++)
            {
                /**���ο� ������ �����Ѵ�.....*/
                GameObject newCoin = GameObject.Instantiate(CoinPrefab);

                Rigidbody coinBody = newCoin.AddComponent<Rigidbody>();
                coinBody.velocity  = IpariUtility.CaculateVelocity(Search(), flowerPos, CoinFlightTime);

                /**���ο� �ڼ� ����� ���ο� ���� �����Ѵ�...*/
                if(UseCoinMagnet){

                    ScoreObject coinScore = newCoin.GetComponent<ScoreObject>();
                    coinScore.SetTime(CoinFlightTime, 2f);
                }

                Transform coinTr    = newCoin.transform;
                Vector3 coinPos     = flowerPos;
                Quaternion coinQuat = Quaternion.Euler(90f, 0f, 0f);
                coinTr.SetPositionAndRotation(coinPos, coinQuat);
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
