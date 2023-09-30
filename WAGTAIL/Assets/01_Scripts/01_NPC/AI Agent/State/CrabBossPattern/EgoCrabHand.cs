using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 *   �н� �����ȷ� ������� ����� ������ ������Ʈ�Դϴ�...
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

        /**�ش� �ݶ��̴��� Ʈ���� üũ�� �Ѵ�...*/
        Collider collider = GetComponent<Collider>();   
        if(collider!=null){

            collider.isTrigger = true;
        }

        /**�ִϸ��̼� Ŀ�갡 ���� ���, �ʱ�ȭ.*/
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
        /**�浹�ߴ��� üũ�� �Ѵ�...*/
        if (!_isAttack) return;


        /****************************************
         *   �÷��̾ ���� �����̸� ��ŵ�Ѵ�...
         * ***/
        if (other.CompareTag("Player"))
        {
            Player.Instance.isDead = true;
        }

        /**�������� ó��...*/
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
         *   ��꿡 �ʿ��� �͵��� ���Ѵ�...
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
         *   ���� �����Ѵ�...
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
         *    ����� �����Ѵ�...
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
         *    ����� ���� ������´�...
         * ***/
        _isAttack = true;
        timeLeft = _AttackDuration;

        /**������� �������� �Ÿ��� ���Ѵ�...*/
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

        /**������� ������ �����Ѵ�...*/
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


        /**������ ��ģ �� ���...*/
        timeLeft = .7f;
        while ((timeLeft -= Time.deltaTime) > 0f) yield return null;


        /****************************************
         *   ���� �������...
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
