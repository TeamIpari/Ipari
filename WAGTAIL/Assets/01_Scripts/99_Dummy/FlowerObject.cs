using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;
using System.Text;


/**************************************************
 *   당길 수 있는 꽃의 움직임이 구현된 컴포넌트입니다.
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
    public GameObject PullableObj;

    [SerializeField] 
    public GameObject FoldoutSFX;

    [SerializeField]
    public float      LookTargetRatio = .1f;

    [SerializeField]
    public float     LookRotationRatio = .1f;

    [SerializeField, Min(0f)] 
    public float      CoinSpawnRadian = .5f;

    [SerializeField, Min(0f)] 
    public float      CoinFlightTime  = 2f;

    [SerializeField, Min(0)] 
    public int        CointSpawnCount = 5;

    [SerializeField, Min(0f)]
    public float     FlowerDestroyDuration = .3f;



    //===============================================
    //////               Fields               //////
    //==============================================
    private Animator        _FlowerAnimator;
    private PullableObject  _PullableStem;

    private const int       _upLeafMaxCount  = 4;
    private int             _upLeafCount     = 0;
    private int             _lookAtBoneIndex = 0;
    private LeafDesc[]      _upLeafs         = new LeafDesc[4];



    //======================================================
    ///////              Magic methods                //////
    //======================================================
    private void Start()
    {
        #region Omit
        /************************************************************
         *   해당 꽃이 가지는 PullableObject에 대한 초기화를 진행한다...
         * ****/
        if( PullableObj && (_PullableStem = PullableObj.GetComponent<PullableObject>()))
        {
            _FlowerAnimator  = GetComponent<Animator>();
            _lookAtBoneIndex = Mathf.RoundToInt((_PullableStem.BoneCount-1)* LookTargetRatio);

            /**PullableObject에 대리자를 등록한다....*/
            _PullableStem.OnPullStart.AddListener(() => { 
                _FlowerAnimator.Play("PullingStart"); 
            });

            _PullableStem.OnPullRelease.AddListener(() => {
                _FlowerAnimator.Play("Idle"); 
            });

            _PullableStem.OnBreak.AddListener(() => { 
                _FlowerAnimator.Play("PullingEnd"); 
                FlowerFoldout();
            });
        }
        

        /*******************************************************************
         *   PullableObject가 당겨지는 곳을 바라볼 잎사귀들의 참조를 가져온다...
         * ****/
        Transform flowerTr = transform;
        for(int i=0; i<_upLeafMaxCount; i++)
        {
            ref LeafDesc desc = ref _upLeafs[_upLeafCount];
            
            /**해당 이름의 잎사귀가 존재하지 않는다면 스킵한다...*/
            if((desc.leafTr = flowerTr.Find($"CoinFlower_Up_Zero{i + 1}"))==null)
                continue;

            /**해당 잎사귀의 최초 쿼터니언 값을 캐싱한다...*/
            desc.OriginQuat = desc.leafTr.rotation;
            _upLeafCount++;
        }
        #endregion
    }

    private void LateUpdate()
    {
        #region Omit
        /***********************************************
         *    꽃이 당겨질 때의 IK를 적용한다.....
         * ****/
        if (_PullableStem == null || _upLeafCount< _upLeafMaxCount) return;

        /**계산에 필요한 요소들을 구한다....*/
        Vector3   goalPos      = _PullableStem.GetBonePosition(_lookAtBoneIndex);
        Vector3   flowerPos    = transform.position;
        Vector3   pullingDir   = (goalPos - flowerPos).normalized;
        Quaternion rotQuat     = IpariUtility.GetQuatBetweenVector(_upLeafs[0].leafTr.forward, pullingDir, Time.deltaTime*50f);
   
        _upLeafs[0].leafTr.rotation = (rotQuat * _upLeafs[0].leafTr.rotation);
        _upLeafs[1].leafTr.rotation = (rotQuat * _upLeafs[1].leafTr.rotation);
        _upLeafs[2].leafTr.rotation = (rotQuat * _upLeafs[2].leafTr.rotation);
        _upLeafs[3].leafTr.rotation = (rotQuat * _upLeafs[3].leafTr.rotation);
        #endregion
    }



    //=====================================================
    ///////              Core methods                //////
    //=====================================================
    public void FlowerFoldout()
    {
        #region Omit
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Flowers_Burst, transform.position, 2f);

        /********************************************
         *   꽃이 펼쳐졌을 때 다수의 코인을 생성한다....
         * ***/
        if (CoinPrefab != null){

            Vector3 flowerPos = transform.position;
            for (int i = 0; i < CointSpawnCount; i++)
            {
                /**새로운 코인을 생성한다.....*/
                ScoreObject newCoin = GameObject.Instantiate(CoinPrefab).GetComponent<ScoreObject>();
                newCoin.SetTime(CoinFlightTime, 2f);
                newCoin.UseMagnetMovement = true;
                newCoin.ItemGetType       = ScoreObject.GetKind.None;

                newCoin.UseRigidbody(true,true);
                newCoin.Body.velocity = IpariUtility.CaculateVelocity(Search(), flowerPos, CoinFlightTime);

                Transform coinTr = newCoin.transform;
                Vector3 coinPos = flowerPos;
                Quaternion coinQuat = Quaternion.Euler(90f, 0f, 0f);
                coinTr.SetPositionAndRotation(coinPos, coinQuat);
            }
        }

        
        /************************************************
         *    꽃이 펼쳐졌을 때의 이펙트를 생성한다....
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

    public void DestroyFlower()
    {
        StartCoroutine(DestroyProgress());
    }

    private IEnumerator DestroyProgress()
    {
        #region Omit
        float leftTime     = FlowerDestroyDuration;
        float leftTimeDiv  = (1f/leftTime);
        Vector3 startScale = transform.localScale;

        _FlowerAnimator.enabled = false;

        /**꽃이 점점 작아지다가 파괴된다....*/
        while((leftTime-=Time.deltaTime)>=0f)
        {
            float progressRatio = (leftTime * leftTimeDiv);
            transform.localScale = (startScale*progressRatio);
            yield return null;
        }

        Destroy(gameObject);

        #endregion
    }

}
