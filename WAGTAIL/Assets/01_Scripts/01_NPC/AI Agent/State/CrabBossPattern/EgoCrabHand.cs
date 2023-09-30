using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 *   분신 집게팔로 내려찍는 기능이 구현된 컴포넌트입니다...
 * ***/
[RequireComponent(typeof(Animator))]
public sealed class EgoCrabHand : MonoBehaviour
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
    [SerializeField] public  AnimationCurve curve;
    [SerializeField] private float          _AttackReadyDuration = 1f;
    [SerializeField] private float          _AttackDuration = 1f;
    [SerializeField] private float          _AttackRange = 1f;


    private Animator  _animator;
    private Coroutine _progressCoroutine;
    private Vector3   _startScale = Vector3.one;
    private bool      _isAttack = false;



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
        if(curve==null)
        {
            curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(.810f, .405f);
            curve.AddKey(1f, -1f);
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Omit
        /**충돌했는지 체크를 한다...*/
        if (!_isAttack) return;


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
    public void StartCrabHand( Vector3 startPos )
    {
        if(_progressCoroutine!=null) StopCoroutine(_progressCoroutine);
        transform.position = startPos;
        _progressCoroutine = StartCoroutine(AttackProgress());
        _isAttack = false;
    }

    private IEnumerator AttackProgress()
    {
        #region Omit
        if (targetTransform == null) yield break;

        _isAttack = false;

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
            tr.localScale = (goalScale * progressRatio);

            yield return null;

        } while (progressRatio < 1f);


        /******************************************
         *    대상을 추적한다...
         * ***/
        timeLeft = _AttackReadyDuration;
        do
        {
            float deltaTime = Time.fixedDeltaTime;
            timeLeft -= deltaTime;

            progressRatio = (1f - Mathf.Clamp(timeLeft * attackReadyDiv, 0f, 1f));
            Vector3 goalPos = targetTransform.position + (Vector3.up * 3f);
            Vector3 updatePos = (goalPos - tr.position) * (deltaTime * 3f);
            tr.position += updatePos;

            yield return waitTime;

        } while (progressRatio < 1f);


        /******************************************
         *    대상을 향해 내려찍는다...
         * ***/
        _isAttack = true;
        timeLeft = _AttackDuration;

        /**내려찍는 곳까지의 거리를 구한다...*/
        RaycastHit hit;
        if (!Physics.SphereCast(
            
            tr.position,
            2f,
            Vector3.down,
            out hit,
            (targetTransform.position - tr.position).magnitude * 1.5f,
             1 << LayerMask.NameToLayer("Platform")))
        {
            yield break;
        }

        /**내려찍는 동작을 실행한다...*/
        Vector3 startPos = tr.position;

        do
        {
            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio = (1f - Mathf.Clamp(timeLeft * attackDiv, 0f, 1f));
            float updatePos = (hit.point.y - startPos.y) * curve.Evaluate(progressRatio);

            tr.position = startPos + (Vector3.down * updatePos);
            yield return null;

        } while (progressRatio < 1f);


        _isAttack = false;
        CameraManager.GetInstance()?.CameraShake(3f, .2f);


        /**공격을 마친 후 대기...*/
        timeLeft = .7f;
        while ((timeLeft -= Time.deltaTime) > 0f) yield return null;


        /****************************************
         *   손이 사라진다...
         * ****/
        timeLeft = .3f;
        do
        {
            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio = (Mathf.Clamp(timeLeft * sizeupDiv, 0f, 1f));
            tr.localScale = (goalScale * progressRatio);

            yield return null;

        } while (progressRatio < 1f);


        yield break;
        #endregion
    }


}
