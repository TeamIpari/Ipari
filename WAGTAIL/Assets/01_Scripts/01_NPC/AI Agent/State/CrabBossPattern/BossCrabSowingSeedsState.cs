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
        public Rigidbody      throwBody;
        public ParticleSystem marker;
        public Collider       collider;
        public Vector3        goalPos;
        public Vector3        goalNormal;
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
        #region Omit
        _desc = desc;
        _targets         = new ThrowDesc[_desc.count];
        _bossCrab        = bossCrab;
        #endregion
    }

    public override void Enter()
    {
        #region Omit
        AISM.Animator.CrossFade(BossCrabAnimation.SpitSeedsReady, .3f);
        _changeTimeDiv   = (1f / _desc.changeTime);
        curTimer         = 0f;
        _isShoot         = false;
        _ignoreRelease   = false;
        _progress        = 0;
        #endregion
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        #region Omit
        base.Update();
        if (Player.Instance.isDead) return;

        /**************************************
         *   로직을 갱신한다....
         * ***/
        ApplySeedsIgonre();


        /*************************************
         *   상태 트리거에 따른 로직 적용....
         * ***/
        if (!_bossCrab.StateTrigger) return;

        switch (_progress++){

                /**씨앗 뱉는 모션을 적용한다....*/
                case (0):
                {
                    AISM.Animator.speed = 1.5f;
                    AISM.Animator.CrossFade(BossCrabAnimation.SpittingSeeds, .1f);
                    break;
                }

                /**씨앗을 뱉는다...*/
                case (1):
                {
                    curTimer = _desc.flightTime * .7f;
                    AISM.Animator.speed = 1f;

                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Crab_SeedSpitOut, 
                        _bossCrab.transform.position, 
                        2f
                     );

                    CameraManager.GetInstance().CameraShake(.2f, CameraManager.ShakeDir.HORIZONTAL, .4f);
                    IpariUtility.PlayGamePadVibration(.1f, .1f, .1f);

                    CreateMarker();
                    PositionLuncher();
                    _bossCrab.SetStateTrigger(.1f);
                    break;
                }

                /**상태를 탈출한다...*/
                case (2):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .4f);
                    _bossCrab.SetStateTrigger(3f);
                    break;
                }

                /**다음 패턴으로 넘어간다...*/
                case (3):
                {
                    AISM.NextPattern();
                    break;
                }
        }

        _bossCrab.StateTrigger = false;

        #endregion
    }



    //==========================================
    ///////         Core methods            ////
    //==========================================
    protected override void ChangeState()
    {
        #region Omit
        /****************************************
         *   각 마커들의 크기가 점점 커지도록 한다...
         * ***/
        int Count           = _desc.count;
        float progressRatio = Mathf.Clamp((curTimer * _changeTimeDiv), 0f, 1f);

        for( int i=0; i<Count; i++ )
        {
            GameObject marker = _targets[i].marker.gameObject;
            if (marker == null) continue;

            marker.transform.localScale = (Vector3.one * progressRatio);
        }


        /**시간이 다되면 다음 상태로 전환한다...*/
        if(progressRatio>=1f) base.ChangeState();

        #endregion
    }

    private void CreateMarker()
    {
        #region Omit
        /*****************************************
         *   Player의 주변위치들을 저장한다....
         * ****/
        for (int i=0; i<_desc.count; i++){

           Search(ref _targets[i]);
        }


        /*****************************************
         *   marker 객체를 생성하고, 참조를 담아둔다.
         * ****/
        for(int i=0; i<_desc.count; i++)
        {
            ref ThrowDesc desc = ref _targets[i];

            //if(desc.marker==null){
            //    desc.marker = GameObject.Instantiate(_desc.MarkerPrefab).GetComponent<ParticleSystem>();
            //}

            //Transform  newMarkerTr  = desc.marker.transform;
            //Quaternion newQuat      = IpariUtility.GetQuatBetweenVector(newMarkerTr.up, -desc.goalNormal);

            //desc.marker.Play(true);
            //desc.marker.GetComponentInChildren<Transform>().localScale = (Vector3.one*.3f);
            //desc.marker.transform.SetPositionAndRotation( _targets[i].goalPos, newQuat);
        }
        #endregion
    }

    private void PositionLuncher()
    {
        #region Omit

        for (int i= 0; i < _desc.count; i++)
        {
            ref ThrowDesc desc = ref _targets[i];

            /**던져졌을 때의 가속도를 계산한다...*/
            Vector3 vel = IpariUtility.CaculateVelocity(
                desc.goalPos, 
                _desc.shootPoint.position, 
                _desc.flightTime * Random.Range(.5f, 1f),
                .5f
            );

            /**씨앗 폭탄을 생성한다...*/
            GameObject obj = GameObject.Instantiate(
                _desc.SeedPrefab, 
                _desc.shootPoint.position, 
                Quaternion.identity
            );

            /**씨앗 폭탄에게 가속도를 가한다...*/
            desc.throwBody = obj.GetComponent<Rigidbody>();
            if(desc.throwBody != null)
            {
                float torquePow = Random.Range(1f, 5f);

                desc.collider           = obj.GetComponent<Collider>();
                desc.throwBody.velocity = vel;
                desc.throwBody.AddTorque(vel*torquePow);
            }
        }

        /**각 씨앗들은 서로 충돌판정이 나지 않도록 한다...*/
        for (int i = 0; i < _desc.count; i++){

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
         *   플레이어를 기준으로하는 원 반경에서의 무작위 위치값을 반환한다...
         * ***/
        Vector3 unitDir  = Random.onUnitSphere;
        float   randRad  = Random.Range(0.0f, _desc.rad);
        unitDir.y = 0f;

        RaycastHit result;
        bool isHit = IpariUtility.GetPlayerFloorinfo(

            out result, 
            (1<<LayerMask.NameToLayer("Platform")), 
            (unitDir * randRad),
             5f
        );

        /******************************************
         *    위치를 제한시킨다...
         * *****/
        Vector3 center = new Vector3(-0.949999988f, 1.63f, -15.4200001f);
        Vector3 dir    = (result.point - center);
        float   dst    = dir.sqrMagnitude;
        if(dst > (5f*5f)){

            desc.goalPos = center + (dir.normalized*7f);
        }


        /**최종 결과물을 기록한다.....*/
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
            /**각 씨앗들은 서로 충돌판정이 나지 않도록 한다...*/
            for (int i = 0; i < _desc.count; i++){

                IgnoreCollisionOtherSeeds(ref _targets[i], false);
            }

            _ignoreRelease = true;
        }
        #endregion
    }
}
