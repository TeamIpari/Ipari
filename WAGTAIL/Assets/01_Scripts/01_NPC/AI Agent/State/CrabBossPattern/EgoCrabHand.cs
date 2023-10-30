using AmplifyShaderEditor;
using DG.Tweening.Core.Easing;
using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPariUtility.IpariUtility;

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
    [SerializeField] public  GameObject     SpawnSFXprefab;
    [SerializeField] public  GameObject     AttackSFXPrefab;
    [SerializeField] public  AnimationCurve curve;
    [SerializeField] private float          _AttackReadyDuration = 1f;
    [SerializeField] private float          _AttackDuration = .5f;
    [SerializeField] private float          _AttackRange = 1f;
    [SerializeField] public  bool           IsAttack = false;


    private Animator       _animator;
    private Coroutine      _progressCoroutine;
    private Vector3        _startScale = Vector3.one;
    private ParticleSystem _attackSFXIns;



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
        if (!IsAttack) return;


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
        #region Omit
        if (_progressCoroutine!=null) StopCoroutine(_progressCoroutine);
        transform.position = startPos;
        _progressCoroutine = StartCoroutine(AttackProgress());
        IsAttack = false;

        if(SpawnSFXprefab!=null){

            GameObject newSFX = GameObject.Instantiate(SpawnSFXprefab);
            newSFX.transform.position   = transform.position;
            newSFX.transform.localScale = (Vector3.one*.5f);
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
         *    ����� ���� ������´�...
         * ***/
        timeLeft = _AttackDuration;

        /**������� �������� �Ÿ��� ���Ѵ�...*/
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

        /**�������� ���� �ִٸ�, ������� ������ �����Ѵ�...*/
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
         *   ���԰� �������� �� �߻��ϴ� ����Ʈ�� ������ �����Ѵ�...
         * ****/
        IsAttack = false;
        CameraManager.GetInstance()?.CameraShake(.4f, CameraManager.ShakeDir.ROTATE, .6f);

        /**����Ʈ�� ���ٸ� �����Ѵ�...*/
        if(_attackSFXIns==null && AttackSFXPrefab != null){

            _attackSFXIns = GameObject.Instantiate(AttackSFXPrefab).GetComponent<ParticleSystem>();
            _attackSFXIns.transform.localScale = (Vector3.one * .5f);

            ParticleSystem.MainModule main = _attackSFXIns.main;
            main.loop = false;
        }

        Transform  sfxTr   = _attackSFXIns.transform;
        Vector3    newPos  = hit.point + (hit.normal * .5f);
        Quaternion newQuat = IpariUtility.GetQuatBetweenVector(sfxTr.up, hit.normal);

        sfxTr.SetPositionAndRotation(newPos, newQuat);
        _attackSFXIns.Play(true);


        /************************************************
         *   ���԰� ���� ��� �´굵�� ȸ����Ų��....
         * ****/

        /**��꿡 �ʿ��� �͵��� ������ ���Ѵ�....*/
        timeLeft             = .7f;
        Quaternion startQuat = transform.rotation;
        Vector3    startDir  = -transform.forward;
        Vector3    right     = -Vector3.Cross(-transform.forward, hit.normal);
        Vector3    forward   = Vector3.Cross(right, hit.normal);

        Quaternion rotQuat = IpariUtility.GetQuatBetweenVector(startDir, forward);
        tr.rotation = (rotQuat * startQuat);

        while ((timeLeft -= Time.deltaTime)>0f) yield return null;


        /****************************************
         *   ���� �������...
         * ****/
        timeLeft = .3f;

        do{

            float deltaTime = Time.deltaTime;
            timeLeft -= deltaTime;

            progressRatio = Mathf.Clamp01(timeLeft * sizeupDiv);
            tr.localScale = (goalScale * progressRatio);

            yield return null;

        } 
        while (progressRatio < 1f);

        #endregion
    }


}
