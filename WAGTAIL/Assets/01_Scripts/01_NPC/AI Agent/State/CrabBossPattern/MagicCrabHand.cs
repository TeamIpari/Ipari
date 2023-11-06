using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPariUtility.IpariUtility;

/*****************************************************
 *   분신 집게팔로 내려찍는 기능이 구현된 컴포넌트입니다...
 * ***/
[RequireComponent(typeof(Animator))]
public sealed class MagicCrabHand : MonoBehaviour
{
    //===============================================
    //////        Property and Fields           /////
    //===============================================
    public Animator CrabHandAnimator    { get { return _animator; } }
    public float    AttackReadyDuration { get { return _AttackReadyDuration; } set { _AttackReadyDuration = (value < 0 ? 0f : value); }  }
    public float    AttackDuration      { get { return _AttackDuration; } set { _AttackDuration = (value < 0 ? 0f : value); } }
    public float    AttackRange         { get { return _AttackRange; } set { _AttackRange = (value < 0 ? 0f : value); } }

    [SerializeField] public  Transform      targetTransform;
    [SerializeField] public  GameObject     MarkerPrefab;
    [SerializeField] public  GameObject     SpawnSFXprefab;
    [SerializeField] public  GameObject     AttackSFXPrefab;
    [SerializeField] public  GameObject     SonicBoomSFXPrefab;
    [SerializeField] public  AnimationCurve curve;
    [SerializeField] private float          _AttackReadyDuration = 1f;
    [SerializeField] private float          _AttackDuration      = .5f;
    [SerializeField] private float          _AttackRange         = 1f;
    [SerializeField] public  bool           IsAttack             = false;

    [SerializeField, Min(0f)] public float SpawnSFXScale     = .5f;
    [SerializeField, Min(0f)] public float AttackSFXScale    = .1f;
    [SerializeField, Min(0f)] public float SonicBoomSFXScale = 4f;


    private Animator       _animator;
    private Coroutine      _progressCoroutine;
    private Vector3        _startScale = Vector3.one;
    private ParticleSystem _attackSFXIns, _sonicBoomSFXIns;
    private Material       _glowMat;
    private Transform      _ringTr;



    //=======================================
    /////        Magic methods          /////
    //=======================================
    private void Awake()
    {
        #region Omit
        _animator   = GetComponent<Animator>();
        _startScale = transform.localScale;

        /**해당 콜라이더의 트리거 체크를 한다...*/
        Collider collider = GetComponent<Collider>();   
        if(collider!=null){

            collider.isTrigger = true;
        }

        /**애니메이션 커브가 없을 경우, 초기화.*/
        if(curve==null || curve.length==0)
        {
            curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(.810f, .405f);
            curve.AddKey(1f, -1f);
        }

        /**집게 머터리얼을 가져온다....*/
        _glowMat = transform.GetChild(0).GetComponent<Renderer>().material;
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Omit
        /**충돌했는지 체크를 한다...*/
        if (!IsAttack) return;


        /****************************************
         *   플레이어가 점프 상태이면 스킵한다...
         * ***/
        if (other.CompareTag("Player"))
        {
            Player.Instance.isDead = true;
        }

        /**공통적인 처리...*/
        #endregion
    }



    //=======================================
    /////         Core methods           ////
    //========================================
    public void StartAttack( Vector3 startPos )
    {
        #region Omit
        if (_progressCoroutine!=null) StopCoroutine(_progressCoroutine);
        transform.position = startPos;
        _progressCoroutine = StartCoroutine(AttackProgress());
        IsAttack = false;

        if(SpawnSFXprefab!=null){

            GameObject newSFX = GameObject.Instantiate(SpawnSFXprefab);
            newSFX.transform.position   = transform.position;
            newSFX.transform.localScale = (Vector3.one* SpawnSFXScale);
            Destroy(newSFX, .8f);
        }
        #endregion
    }

    private void SetCurrentMarkerTransform( GameObject marker )
    {
        #region Omit
        RaycastHit hit;
        if(Physics.Raycast( transform.position,
                            Vector3.down,
                            out hit,
                            5f,
                            1<<LayerMask.NameToLayer("Platform"),
                            QueryTriggerInteraction.Ignore ))
        {

            Transform  markerTr = marker.transform;
            Vector3    newPos   = hit.point + (hit.normal * .5f);
            Quaternion newQuat =  IpariUtility.GetQuatBetweenVector(markerTr.forward, hit.normal); 

            markerTr.SetPositionAndRotation(newPos, newQuat);
        }
        #endregion
    }

    private IEnumerator AttackProgress()
    {
        #region Omit
        if (targetTransform == null) yield break;


        /***************************************
         *   계산에 필요한 것들을 구한다...
         * ***/
        float attackReadyDiv = (1f / _AttackReadyDuration);
        float sizeupDiv      = (1f / .3f);
        float attackDiv      = (1f / _AttackDuration);
        float timeLeft       = .3f;
        float progressRatio  = 0f;

        Transform tr      = transform;
        Vector3 goalScale = _startScale;
        WaitForFixedUpdate waitTime = new WaitForFixedUpdate();


        /****************************************
         *   손이 등장한다...
         * ****/
        do
        {
            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio = (1f - Mathf.Clamp(timeLeft * sizeupDiv, 0f, 1f));
            _glowMat.SetFloat("_alpha", progressRatio );

            yield return null;

        } while (progressRatio < 1f);




        /******************************************
         *    대상을 추적한다...
         * ***/
        timeLeft = AttackReadyDuration;
        do
        {
            float deltaTime   = Time.fixedDeltaTime;
            progressRatio     = (1f - Mathf.Clamp((timeLeft-=deltaTime) * attackReadyDiv, 0f, 1f));

            Vector3 goalPos   = targetTransform.position + (Vector3.up * 3f);
            Vector3 updatePos = (goalPos - tr.position) * (deltaTime * 3f);

            Vector3 tong2Player = (goalPos-transform.position).normalized;
            tr.rotation =  (IPariUtility.IpariUtility.GetQuatBetweenVector(-transform.forward, tong2Player, deltaTime*3f) * tr.rotation);
            tr.position += updatePos;

            yield return waitTime;

        } while (IsAttack == false && progressRatio<1f);


        /******************************************
         *    대상을 향해 내려찍는다...
         * ***/
        timeLeft = _AttackDuration;

        /**내려찍는 곳까지의 거리를 구한다...*/
        RaycastHit hit;
        LayerMask  layer = LayerMask.GetMask("Platform");

        bool isHit = Physics.SphereCast(
            
            tr.position,
            2f,
            Vector3.down,
            out hit,
            (targetTransform.position - tr.position).magnitude * 1.5f,
            layer
        );

        /**내려찍을 곳이 있다면, 내려찍는 동작을 실행한다...*/
        if (!isHit) yield break;
        Vector3   startPos = tr.position;
        Transform playerTr = Player.Instance.transform;
        do
        {
            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio    = (1f - Mathf.Clamp(timeLeft * attackDiv, 0f, 1f));
            float curveValue = curve.Evaluate(progressRatio);
            float updatePos  = (hit.point.y - startPos.y) * curveValue;

            Vector3 lookPos = (playerTr.position - transform.position).normalized;
            lookPos.y += Mathf.Clamp(curveValue, 0f, float.MaxValue);

            tr.rotation = (IPariUtility.IpariUtility.GetQuatBetweenVector(-transform.forward, lookPos, deltaTime*10f) * tr.rotation);
            tr.position = startPos + (Vector3.down * updatePos);
            yield return null;

        } while (progressRatio < 1f);

        /********************************************************
         *   집게가 내려쳤을 때 발생하는 이펙트와 진동을 적용한다...
         * ****/
        IsAttack = false;
        CameraManager.GetInstance()?.CameraShake(.4f, CameraManager.ShakeDir.ROTATE, .6f);
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_Atk3Smash);
        IpariUtility.PlayGamePadVibration(.1f, .1f, .2f);


        /************************************************
         *   내려찍었을 때의 모래먼지 이펙트를 실행한다....
         * ****/

        /**내려찍는 이펙트가 없다면 생성한다...*/
        if(_attackSFXIns==null && AttackSFXPrefab != null){

            _attackSFXIns = GameObject.Instantiate(AttackSFXPrefab).GetComponent<ParticleSystem>();
            _attackSFXIns.transform.localScale = (Vector3.one * AttackSFXScale);

            ParticleSystem.MainModule main = _attackSFXIns.main;
            main.loop = false;
        }

        if(_attackSFXIns!=null)
        {
            Transform sfxTr    = _attackSFXIns.transform;
            Vector3 newPos     = hit.point + (hit.normal * .5f);
            Quaternion newQuat = IpariUtility.GetQuatBetweenVector(sfxTr.up, hit.normal);

            sfxTr.SetPositionAndRotation(newPos, newQuat);
            sfxTr.transform.localScale = (Vector3.one * AttackSFXScale);
            _attackSFXIns.Play(true);
        }


        /************************************************
         *    소닉붐 이펙트를 실행한다....
         * *****/

        /**소닉붐 이펙트가 없다면 생성한다...*/
        if(_sonicBoomSFXIns==null && SonicBoomSFXPrefab!=null){

            _sonicBoomSFXIns = GameObject.Instantiate(SonicBoomSFXPrefab).GetComponent<ParticleSystem>();

            Transform sonicSFXTr  = _sonicBoomSFXIns.transform;
            _ringTr               = sonicSFXTr.Find("FX_SonicBoom_Ring");
            sonicSFXTr.rotation   = Quaternion.Euler(-180f, 0f, 0f);

            int childCount = sonicSFXTr.childCount;
            for(int i=0; i<childCount; i++){

                sonicSFXTr.GetChild(i).localScale = (Vector3.one * SonicBoomSFXScale);
            }

            ParticleSystem.MainModule main = _sonicBoomSFXIns.main;
            main.loop = false;
        }

        if(_sonicBoomSFXIns!=null){

            Transform sonicSFXTr2   = _sonicBoomSFXIns.transform;
            Vector3 sonicNewPos     = transform.position + (hit.normal * 2f);
            sonicSFXTr2.position = sonicNewPos;

            Vector3    ringPos  = hit.point + (hit.normal * .5f);
            Quaternion ringQuat = IpariUtility.GetQuatBetweenVector(_ringTr.forward, hit.normal) * _ringTr.rotation;
            _ringTr.SetPositionAndRotation(ringPos, ringQuat);  
            _sonicBoomSFXIns.Play(true);
        }


        /************************************************
         *   집게가 땅의 면과 맞닿도록 회전시킨다....
         * ****/

        /**계산에 필요한 것들을 모조리 구한다....*/
        timeLeft             = .7f;
        Quaternion startQuat = transform.rotation;
        Vector3    startDir  = -transform.forward;
        Vector3    right     = -Vector3.Cross(-transform.forward, hit.normal);
        Vector3    forward   = Vector3.Cross(right, hit.normal);

        Quaternion rotQuat = IpariUtility.GetQuatBetweenVector(startDir, forward);
        tr.rotation = (rotQuat * startQuat);

        while ((timeLeft -= Time.deltaTime)>0f) yield return null;


        /****************************************
         *   손이 사라진다...
         * ****/
        timeLeft = .3f;

        do{

            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio = Mathf.Clamp01(timeLeft * sizeupDiv);
            //tr.localScale = (goalScale * progressRatio);
            _glowMat.SetFloat("_alpha", progressRatio);

            yield return null;

        } 
        while (progressRatio < 1f);

        #endregion
    }
}
