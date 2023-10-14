using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 *   모래 이펙트가 점점 퍼지는 기능이 구현된 컴포넌트입니다...
 * ***/
[RequireComponent(typeof(SphereCollider))]
public class SandWave : MonoBehaviour
{
    //===============================================
    //////        Property and Fields           /////
    //===============================================
    [SerializeField] public AnimationCurve waveCurve;
    [SerializeField] public SandScript     SandTarget;
    [SerializeField] public GameObject     SandFX;
    [SerializeField] public float          FXScale = 1f;
    [SerializeField] public float          WaveDuration;
    [SerializeField] public float          WaveMaxRadius;
    [SerializeField] public int            Pricision = 10;


    private GameObject[]   _FXLists;
    private SphereCollider _collider;
    private float          _radianDiv = 0f;

    private float           _timeLeft = -1f;
    private float           _shakeTimeLeft = 0f;
    private float           _shakeTime = .5f;
    private float           _shakeMaxTime = .8f;
    private float           _timeDiv = 1f;
    private float           _delayTime = 0f;



    //========================================
    /////          Magic methods          ////
    //========================================
    private void Awake()
    {
        #region Omit
        /**콜라이더를 초기화한다...*/
        if(_collider = GetComponent<SphereCollider>()){

            _collider.isTrigger = true;
            transform.localScale = Vector3.one;
        }

        /**애니메이션 커브를 초기화한다...*/
        if(waveCurve==null || (waveCurve!=null && waveCurve.length==0) ){

            waveCurve = new AnimationCurve();
            waveCurve.AddKey(0f, 0f);
            waveCurve.AddKey(0.869f, .523f);
            waveCurve.AddKey(1f, 1f);
            waveCurve.postWrapMode = WrapMode.Clamp;
        }

        /*********************************************
         *   Sand FX들을 초기화시킨다....
         * ***/
        if (SandFX != null){

            _FXLists = new GameObject[Pricision];

            /**이펙트들을 생성한다...*/
            for (int i=0; i< Pricision; i++){

                _FXLists[i] = Instantiate(SandFX);

                ParticleSystem            system = _FXLists[i].GetComponent<ParticleSystem>();
                ParticleSystem.MainModule module = system.main;

                module.scalingMode          = ParticleSystemScalingMode.Local;
                system.transform.localScale = (Vector3.one * FXScale);
                system.transform.position   = transform.position;
                system.transform.SetParent(transform);
                system.gameObject.SetActive(false);
            }

            _radianDiv = (Mathf.PI * 2f) / Pricision;
        }
        #endregion
    }

    private void Update()
    {
        #region Omit
        if (_timeLeft < 0f || _FXLists==null) return;

        /**모래파도가 시작되었을 때, 일정시간 대기한다...*/
        if(_delayTime>0f)
        {
            _delayTime -= Time.deltaTime;
            return;
        }

        _timeLeft -= Time.deltaTime;

        /*************************************************
         *   원형 모양으로 퍼지는데 필요한 요소들을 구한다...
         * *****/
        Vector3 center      = transform.position;
        float progressRatio = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float currRadian    = 0f;
        float currRadius    = WaveMaxRadius * waveCurve.Evaluate(progressRatio);

        _collider.radius = currRadius;

        /**점점 빨라지는 화면 흔들림을 구현한다....*/
        if((_shakeTime-=Time.deltaTime)<=0f){

            CameraManager.GetInstance().CameraShake(3f, .5f);
            _shakeTime = Mathf.Clamp(_shakeTime -= 1f, 0f, _shakeMaxTime);
        }


        /********************************************
         *    이펙트들을 진행된 구간으로 이동시킨다...
         * ****/
        float radius = _collider.radius;
        for (int i = 0; i < Pricision; i++){

            Vector3 dir    = new Vector3(  Mathf.Cos(currRadian), 0f, Mathf.Sin(currRadian));
            Vector3 newPos = center + (dir * radius);
            RaycastHit hit;

            if(Physics.Raycast(

                newPos,
                Vector3.down,
                out hit,
                10f,
                 1 << LayerMask.NameToLayer("Platform")
            )) 
            newPos.y = hit.point.y;

            _FXLists[i].transform.position = newPos;
            currRadian += _radianDiv;
        }

        /**마무리 되었을 경우...*/
        if (progressRatio >= 1f)
        {
            for (int i = 0; i < Pricision; i++){

                _FXLists[i].SetActive(false);
            }

            _collider.radius = 0f;
        }

        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        #region Omit
        Rigidbody body;
        Vector3 center2Target  = (other.transform.position - transform.position);
        float center2TargetLen = center2Target.sqrMagnitude;
        float progressRatio    = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float compareLen       = WaveMaxRadius * waveCurve.Evaluate(progressRatio) - 3f;

        /**충돌했는지 체크를 한다...*/
        if (SandTarget != null && SandTarget.PlayerOnSand == false) return;
        if (!(center2TargetLen> (compareLen*compareLen))) return;


        /****************************************
         *   플레이어가 점프 상태이면 스킵한다...
         * ***/
        if (other.CompareTag("Player")){

            if (Player.Instance.movementSM.currentState == Player.Instance.jump) return;

            Player.Instance.isDead = true;
        }


        /******************************************
         *   Rigidbody를 가지고 있을 때의 처리...
         * ****/
        else if(other.CompareTag("interactable") && (body=other.attachedRigidbody)!=null){

            float dst   = (compareLen - center2TargetLen);
            Vector3 pow = (-center2Target.normalized * dst * body.mass*30);
            pow.y = 0f;

            body.velocity = pow;
        }


        /**공통적인 처리...*/
        #endregion
    }



    //==========================================
    /////           Core methods            ////
    //==========================================
    public void StartWave()
    {
        #region Omit
        /**************************************
         *   모래파도를 일으킨다....
         * ****/
        if (_FXLists == null) return;

        /**생성되었던 FX들을 활성화시킨다...*/
        for (int i = 0; i < Pricision; i++) {

            _FXLists[i].transform.position = transform.position;
            _FXLists[i].SetActive(true);
        }

        /**파도 계산에 필요한 모든 요소들을 구한다...*/
        _radianDiv = (Mathf.PI * 2f) / Pricision;
        _timeLeft  = WaveDuration;
        _timeDiv   = 1f / WaveDuration;

        _shakeTime = 0f;
        _shakeTime = _shakeMaxTime;
        _delayTime = .1f;
        #endregion
    }

    private bool CheckHitWave( Vector3 worldPos )
    {
        #region Omit

        /**************************************************
         *   주어진 worldPos가 판정안으로 들어왔는지 검사한다.
         * ***/
        float center2TargetLen = ( worldPos - transform.position ).sqrMagnitude;
        float progressRatio    = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float compareLen       = WaveMaxRadius * waveCurve.Evaluate(progressRatio)-1f;
        compareLen *= compareLen;

        return (center2TargetLen > compareLen);
        #endregion
    }


}
