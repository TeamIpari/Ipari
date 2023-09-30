using FMOD;
using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public sealed class BossCrabSowingSeedsState : AIAttackState
{
    #region Define
    public struct BossCrabSowingSeedsDesc
    {
        public GameObject SeedPrefab;
        public GameObject MarkerPrefab;
        public Transform  shootPoint;
        public float      delayTime;
        public float      flightTime;
        public float      changeTime;
        public float      rad;
        public int        count;
    };

    private struct ThrowDesc
    {
        public Rigidbody  throwBody;
        public Collider   collider;
        public GameObject marker;
        public Vector3    goalPos;
    }
    #endregion

    //======================================
    /////           Fields              ////
    //======================================
    private BossCrabSowingSeedsDesc _desc;
    private readonly ThrowDesc[]    _targets;

    private float _changeTimeDiv = 1f;
    private bool  _isShoot       = false;
    private bool  _ignoreRelease = false;



    //========================================
    //////        Override methods        ////
    //========================================
    public BossCrabSowingSeedsState( AIStateMachine stateMachine, ref BossCrabSowingSeedsDesc desc )
    :base( stateMachine )
    {
        _desc = desc;
        _targets = new ThrowDesc[_desc.count];
    }

    public override void Enter()
    {
        AISM.Animator.SetTrigger("isAttack");
        _changeTimeDiv = (1f / _desc.changeTime);
        curTimer       = 0f;
        _isShoot       = false;
        _ignoreRelease = false;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        base.Update();
        if (Player.Instance.isDead) return;

        /***********************************
         *   Ÿ�̸ӿ� ���� ���º�ȭ ����...
         * ***/
        curTimer += Time.deltaTime;
        ShootDelay();
        ChangeState();
        ApplySeedsIgonre();
    }



    //==========================================
    ///////         Core methods            ////
    //==========================================
    protected override void ChangeState()
    {
        #region Omit
        /****************************************
         *   �� ��Ŀ���� ũ�Ⱑ ���� Ŀ������ �Ѵ�...
         * ***/
        int Count           = _desc.count;
        float progressRatio = Mathf.Clamp((curTimer * _changeTimeDiv), 0f, 1f);

        for( int i=0; i<Count; i++ )
        {
            GameObject marker = _targets[i].marker;
            if (marker == null) continue;

            marker.transform.localScale = (Vector3.one * progressRatio);
        }


        /**�ð��� �ٵǸ� ���� ���·� ��ȯ�Ѵ�...*/
        if(progressRatio>=1f) base.ChangeState();

        #endregion
    }

    private void CreateMarker()
    {
        #region Omit
        /*****************************************
         *   Player�� �ֺ���ġ���� �����Ѵ�....
         * ****/
        for (int i=0; i<_desc.count; i++){

            _targets[i].goalPos = Search();
        }


        /*****************************************
         *   marker ��ü�� �����ϰ�, ������ ��Ƶд�.
         * ****/
        for(int i=0; i<_desc.count; i++)
        {
            GameObject newMarker = GameObject.Instantiate(_desc.MarkerPrefab);
            newMarker.GetComponentInChildren<Transform>().localScale = Vector3.one*.3f;
            newMarker.transform.SetPositionAndRotation(

                  _targets[i].goalPos,
                  Quaternion.Euler(-90f, 0f, 0f)
            );

            _targets[i].marker = newMarker;
        }
        #endregion
    }

    private void PositionLuncher()
    {
        #region Omit
        for (int i= 0; i < _desc.count; i++)
        {
            ref ThrowDesc desc = ref _targets[i];

            /**�������� ���� ���ӵ��� ����Ѵ�...*/
            Vector3 vel = IpariUtility.CaculateVelocity(
                desc.goalPos, 
                _desc.shootPoint.position, 
                _desc.flightTime * Random.Range(.5f,1f)
            );


            /**���� ��ź�� �����Ѵ�...*/
            GameObject obj = GameObject.Instantiate(
                _desc.SeedPrefab, 
                _desc.shootPoint.position, 
                Quaternion.identity
            );


            /**���� ��ź���� ���ӵ��� ���Ѵ�...*/
            desc.throwBody = obj.GetComponent<Rigidbody>();
            if(desc.throwBody != null)
            {
                float torquePow = Random.Range(1f, 5f);

                desc.collider = obj.GetComponent<Collider>();
                desc.throwBody.velocity = vel;
                desc.throwBody.AddTorque(vel*torquePow);
            }
        }

        /**�� ���ѵ��� ���� �浹������ ���� �ʵ��� �Ѵ�...*/
        for(int i=0; i<_desc.count; i++){

            IgnoreCollisionOtherSeeds(ref _targets[i]);
        }
        #endregion
    }

    private void IgnoreCollisionOtherSeeds( ref ThrowDesc desc, bool value=true )
    {
        #region Omit
        if (desc.collider != null)
        {
            int Count = _desc.count;

            for(int i=0; i<Count; i++){

                ref ThrowDesc other = ref _targets[i];
                Physics.IgnoreCollision(desc.collider, other.collider, value );
            }
        }
        #endregion
    }

    private Vector3 Search()
    {
        #region Omit
        /*************************************************************
         *   �÷��̾ ���������ϴ� �� �ݰ濡���� ������ ��ġ���� ��ȯ�Ѵ�...
         * ***/
        Vector3 unitDir  = Random.onUnitSphere;
        float   randRad  = Random.Range(0.0f, _desc.rad);

        Vector3 result = (unitDir * randRad) + Player.Instance.transform.position;
        result.y = .1f;

        return result;
        #endregion
    }

    public void ShootDelay()
    {
        #region Omit
        if (!_isShoot && curTimer >= _desc.delayTime)
        {
            FModAudioManager.PlayOneShotSFX(
                FModSFXEventType.Broken,
                Vector3.zero,
                FModParamLabel.BrokenType.Wall
            );

            CreateMarker();
            PositionLuncher();
            _isShoot = true;
        }

        #endregion
    }

    public void ApplySeedsIgonre()
    {
        #region Omit
        if (!_ignoreRelease && curTimer >= (_desc.flightTime*.5f + _desc.delayTime))
        {
            /**�� ���ѵ��� ���� �浹������ ���� �ʵ��� �Ѵ�...*/
            for (int i = 0; i < _desc.count; i++){

                IgnoreCollisionOtherSeeds(ref _targets[i], false);
            }

            _ignoreRelease = true;
        }
        #endregion
    }
}
