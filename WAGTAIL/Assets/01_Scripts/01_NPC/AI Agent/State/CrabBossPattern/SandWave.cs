using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 *   �� ����Ʈ�� ���� ������ ����� ������ ������Ʈ�Դϴ�...
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
        /**�ݶ��̴��� �ʱ�ȭ�Ѵ�...*/
        if(_collider = GetComponent<SphereCollider>()){

            _collider.isTrigger = true;
            transform.localScale = Vector3.one;
        }

        /**�ִϸ��̼� Ŀ�긦 �ʱ�ȭ�Ѵ�...*/
        if(waveCurve==null || (waveCurve!=null && waveCurve.length==0) ){

            waveCurve = new AnimationCurve();
            waveCurve.AddKey(0f, 0f);
            waveCurve.AddKey(0.869f, .523f);
            waveCurve.AddKey(1f, 1f);
            waveCurve.postWrapMode = WrapMode.Clamp;
        }

        /*********************************************
         *   Sand FX���� �ʱ�ȭ��Ų��....
         * ***/
        if (SandFX != null){

            _FXLists = new GameObject[Pricision];

            /**����Ʈ���� �����Ѵ�...*/
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

        /**���ĵ��� ���۵Ǿ��� ��, �����ð� ����Ѵ�...*/
        if(_delayTime>0f)
        {
            _delayTime -= Time.deltaTime;
            return;
        }

        _timeLeft -= Time.deltaTime;

        /*************************************************
         *   ���� ������� �����µ� �ʿ��� ��ҵ��� ���Ѵ�...
         * *****/
        Vector3 center      = transform.position;
        float progressRatio = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float currRadian    = 0f;
        float currRadius    = WaveMaxRadius * waveCurve.Evaluate(progressRatio);

        _collider.radius = currRadius;

        /**���� �������� ȭ�� ��鸲�� �����Ѵ�....*/
        if((_shakeTime-=Time.deltaTime)<=0f){

            CameraManager.GetInstance().CameraShake(3f, .5f);
            _shakeTime = Mathf.Clamp(_shakeTime -= 1f, 0f, _shakeMaxTime);
        }


        /********************************************
         *    ����Ʈ���� ����� �������� �̵���Ų��...
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

        /**������ �Ǿ��� ���...*/
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

        /**�浹�ߴ��� üũ�� �Ѵ�...*/
        if (SandTarget != null && SandTarget.PlayerOnSand == false) return;
        if (!(center2TargetLen> (compareLen*compareLen))) return;


        /****************************************
         *   �÷��̾ ���� �����̸� ��ŵ�Ѵ�...
         * ***/
        if (other.CompareTag("Player")){

            if (Player.Instance.movementSM.currentState == Player.Instance.jump) return;

            Player.Instance.isDead = true;
        }


        /******************************************
         *   Rigidbody�� ������ ���� ���� ó��...
         * ****/
        else if(other.CompareTag("interactable") && (body=other.attachedRigidbody)!=null){

            float dst   = (compareLen - center2TargetLen);
            Vector3 pow = (-center2Target.normalized * dst * body.mass*30);
            pow.y = 0f;

            body.velocity = pow;
        }


        /**�������� ó��...*/
        #endregion
    }



    //==========================================
    /////           Core methods            ////
    //==========================================
    public void StartWave()
    {
        #region Omit
        /**************************************
         *   ���ĵ��� ����Ų��....
         * ****/
        if (_FXLists == null) return;

        /**�����Ǿ��� FX���� Ȱ��ȭ��Ų��...*/
        for (int i = 0; i < Pricision; i++) {

            _FXLists[i].transform.position = transform.position;
            _FXLists[i].SetActive(true);
        }

        /**�ĵ� ��꿡 �ʿ��� ��� ��ҵ��� ���Ѵ�...*/
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
         *   �־��� worldPos�� ���������� ���Դ��� �˻��Ѵ�.
         * ***/
        float center2TargetLen = ( worldPos - transform.position ).sqrMagnitude;
        float progressRatio    = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float compareLen       = WaveMaxRadius * waveCurve.Evaluate(progressRatio)-1f;
        compareLen *= compareLen;

        return (center2TargetLen > compareLen);
        #endregion
    }


}
