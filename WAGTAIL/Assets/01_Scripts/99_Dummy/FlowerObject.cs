using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;
using System.Text;


/**************************************************
 *   ��� �� �ִ� ���� �������� ������ ������Ʈ�Դϴ�.
 * ****/
[RequireComponent(typeof(Animator))]
public sealed class FlowerObject : MonoBehaviour
{
    private struct LeafDesc
    {
        public Transform  leafTr;
        public Quaternion OriginQuat;
    }

    //===============================================
    //////              Property               //////
    //==============================================
    [SerializeField] 
    public GameObject CoinPrefab;

    [SerializeField] 
    public GameObject FoldoutSFX;

    [SerializeField]
    public float      LookTargetRatio = .1f;

    [SerializeField, Min(0f)] 
    public float      CoinSpawnRadian = .5f;

    [SerializeField, Min(0f)] 
    public float      CoinFlightTime  = 2f;

    [SerializeField, Min(0)] 
    public int        CointSpawnCount = 5;



    //===============================================
    //////               Fields               //////
    //==============================================
    private Animator        _FlowerAnimator;
    private PullableObject  _PullableStem;
    private LeafDesc[]      _upLeafs = new LeafDesc[4];
    private int             _lookAtBoneIndex = 0;



    //======================================================
    ///////              Magic methods                //////
    //======================================================
    private void Start()
    {
        #region Omit
        _FlowerAnimator = GetComponent<Animator>();
        if(_PullableStem = transform.Find("PullingVine_Rig").GetComponent<PullableObject>())
        {
            _lookAtBoneIndex = Mathf.RoundToInt((_PullableStem.BoneCount-1)* LookTargetRatio);
        }
        

        /**IK�� ������ �� �ٻ�͵��� �����´�....*/
        Transform flowerTr = transform;
        for(int i=0; i<=3; i++)
        {
            ref LeafDesc desc = ref _upLeafs[i];

            desc.leafTr     = flowerTr.Find($"CoinFlower_Up_Zero{i+1}");
            desc.OriginQuat = desc.leafTr.rotation;
        }
        #endregion
    }

    private void LateUpdate()
    {
        #region Omit
        /***********************************************
         *    ���� ����� ���� IK�� �����Ѵ�.....
         * ****/
        if (_PullableStem == null) return;

        /**��꿡 �ʿ��� ��ҵ��� ���Ѵ�....*/
        Vector3   goalPos      = _PullableStem.GetBonePosition(_lookAtBoneIndex);
        Vector3   flowerPos    = transform.position;
        Vector3   pullingDir   = (goalPos - flowerPos).normalized;
        Quaternion rotQuat     = IpariUtility.GetQuatBetweenVector(transform.forward, pullingDir, .83f);
   
        _upLeafs[0].leafTr.rotation = (rotQuat * _upLeafs[0].OriginQuat);
        _upLeafs[1].leafTr.rotation = (rotQuat * _upLeafs[1].OriginQuat);
        _upLeafs[2].leafTr.rotation = (rotQuat * _upLeafs[2].OriginQuat);
        _upLeafs[3].leafTr.rotation = (rotQuat * _upLeafs[3].OriginQuat);

        #endregion
    }



    //=====================================================
    ///////             Public methods               //////
    //=====================================================
    public void FlowerFoldout()
    {
        #region Omit

        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Flowers_Burst, transform.position, 2f);
        _FlowerAnimator?.Play("PullingEnd");

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
                coinBody.velocity = IpariUtility.CaculateVelocity(Search(), flowerPos, CoinFlightTime);

                ScoreObject coinScore = newCoin.GetComponent<ScoreObject>();
                coinScore.SetTime(CoinFlightTime, 2f);

                Transform coinTr = newCoin.transform;
                Vector3 coinPos = flowerPos;
                Quaternion coinQuat = Quaternion.Euler(90f, 0f, 0f);
                coinTr.SetPositionAndRotation(coinPos, coinQuat);
            }
        }

        
        /************************************************
         *    ���� �������� ���� ����Ʈ�� �����Ѵ�....
         * ****/
        if(FoldoutSFX!=null){

            GameObject foldoutVFX = Instantiate(FoldoutSFX, transform.position, transform.rotation);
            Destroy(foldoutVFX, 7);
        }


        #endregion
    }

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

}
