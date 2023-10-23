using FMOD;
using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossCrab;

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
        public Vector3    goalNormal;
    }
    #endregion

    //======================================
    /////           Fields              ////
    //======================================
    private BossCrabSowingSeedsDesc _desc;
    private readonly ThrowDesc[]    _targets;
    private BossCrab                _bossCrab;

    private float _changeTimeDiv = 1f;
    private bool  _isShoot       = false;
    private bool  _ignoreRelease = false;
    private int   _progress      = 0;

    //========================================
    //////        Override methods        ////
    //========================================
    public BossCrabSowingSeedsState( AIStateMachine stateMachine, ref BossCrabSowingSeedsDesc desc, BossCrab bossCrab )
    :base( stateMachine )
    {
        _desc = desc;
        _targets = new ThrowDesc[_desc.count];
        _bossCrab = bossCrab;
    }

    public override void Enter()
    {
        #region Omit
        AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsSpitSeeds);
        _changeTimeDiv = (1f / _desc.changeTime);
        curTimer       = 0f;
        _isShoot       = false;
        _ignoreRelease = false;
        _progress      = 0;
        #endregion
    }

    public override void Exit()
    {
        #region Omit
        /**������ ��� ��ũ�� �����Ѵ�...*/
        for (int i=0; i<_desc.count; i++){

            ref ThrowDesc desc = ref _targets[i];
            GameObject.Destroy(desc.marker);
        }

        AISM.Animator.ResetTrigger(BossCrabAnimation.Trigger_IsSpitSeeds);
        #endregion
    }

    public override void Update()
    {
        base.Update();
        if (Player.Instance.isDead) return;

        /**************************************
         *   ������ �����Ѵ�....
         * ***/
        //ApplySeedsIgonre();


        /*************************************
         *   ���� Ʈ���ſ� ���� ���� ����....
         * ***/
        if (!_bossCrab.StateTrigger) return;

        switch (_progress++){

                /**������ ��´�...*/
                case (0):
                {
                    CreateMarker();
                    PositionLuncher();
                    AISM.NextPattern();
                    break;
                }

                /**���¸� Ż���Ѵ�...*/
                case (1):
                {
                    AISM.NextPattern();
                    break;
                }
        }

        _bossCrab.StateTrigger = false;
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

           Search(ref _targets[i]);
        }


        /*****************************************
         *   marker ��ü�� �����ϰ�, ������ ��Ƶд�.
         * ****/
        for(int i=0; i<_desc.count; i++)
        {
            ref ThrowDesc desc = ref _targets[i];

            GameObject newMarker    = GameObject.Instantiate(_desc.MarkerPrefab);
            Transform  newMarkerTr  = newMarker.transform;
            Quaternion newQuat      = IpariUtility.GetQuatBetweenVector(newMarkerTr.up, -desc.goalNormal);

            newMarker.GetComponentInChildren<Transform>().localScale = (Vector3.one*.3f);
            newMarker.transform.SetPositionAndRotation( _targets[i].goalPos, newQuat);

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
                (_desc.flightTime * Random.Range(.5f,1f))
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

                desc.collider           = obj.GetComponent<Collider>();
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

    private void Search(ref ThrowDesc desc)
    {
        #region Omit
        /*************************************************************
         *   �÷��̾ ���������ϴ� �� �ݰ濡���� ������ ��ġ���� ��ȯ�Ѵ�...
         * ***/
        Vector3 unitDir  = Random.onUnitSphere;
        float   randRad  = Random.Range(0.0f, _desc.rad);

        RaycastHit result;
        IpariUtility.GetPlayerFloorinfo(

            out result, 
            ~(1 << LayerMask.NameToLayer("Player")), 
            (unitDir * randRad), 
            1f
        );

        desc.goalPos    = result.point + (result.normal*.03f);
        desc.goalNormal = result.normal;
        #endregion
    }

    public void ShootDelay()
    {
        #region Omit
        if (!_isShoot && curTimer >= _desc.delayTime)
        {
            FModAudioManager.PlayOneShotSFX(
                FModSFXEventType.Broken
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
