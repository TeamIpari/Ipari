using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static BossCrab;

public sealed class BossCrabMagicCrabHandState : AIAttackState
{
    //======================================
    ////            Fields              ////
    //======================================

    /**참조 관련...*/
    private BossCrab       _bossCrab;
    private ParticleSystem _collectFX;
    private MagicCrabHand    _handIns;
    private Material       _EgoMat;
    private Renderer       _renderer;

    /**로직 관련...*/
    private float          _glowgoalPow  = 0f;
    private float          _glowTimeDiv  = 1f;
    private float          _glowTime     = 0f;
    private float          _glowStartPow = 0f;

    private float          _progress    = 0;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabMagicCrabHandState(AIStateMachine stateMachine, MagicCrabHand handIns, BossCrab bossCrab, float collectDuration=1f)
    :base(stateMachine)
    {
        #region Omit
        _glowTimeDiv = (1f / collectDuration);
        _bossCrab = bossCrab;

        _handIns = handIns;
        _handIns.AttackReadyDuration = 3f;
        _handIns.transform.position += (Vector3.up*100f);


        try
        {
            /****************************************************
             *   기를 모으는 이펙트에 대한 참조를 가져온다...
             * ***/
            Transform arm  = AISM.Transform.Find("Boss_Crab_Con").Find("Crab_Body_Bone").Find("L_Tong01");
            Transform tong = arm.Find("Bone011").Find("Bone012");
            _collectFX = tong.Find("Cras ( ready) Ver.3 180").GetComponent<ParticleSystem>();
            _collectFX.gameObject.SetActive(false);


            /***************************************************
             *   집게의 머터리얼에 대한 복사본의 참조를 가져온다.... 
             * ****/
            _renderer = AISM.Transform.Find("Boss_Crab_Mesh").GetComponent<Renderer>();
            _EgoMat = _renderer.materials[2];
            _EgoMat.SetFloat("_alpha", 0f);

        }
        catch { Debug.LogWarning("BossCrabEgoStampState: 참조를 가져오는데 실패하였습니다..."); }

        #endregion
    }

    public override void Enter()
    {
        #region Omit
        {
            _progress = _glowTime = _glowStartPow = 0;
            _glowgoalPow = 1f;

            AISM.Animator.speed = .4f;
            AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_TongRise, .3f);

            _collectFX.gameObject.SetActive(true);
            _collectFX.Play(true);
        }
        #endregion
    }

    public override void Update()
    {
        #region Omit

        /****************************************
         *   집게의 머터리얼의 진하기를 조절한다...
         * ****/
        float progressRatio = Mathf.Clamp01((_glowTime+=Time.deltaTime) * _glowTimeDiv);
        float distance      = (_glowgoalPow - _glowStartPow)*progressRatio;

        _EgoMat.SetFloat("_alpha",  (_glowStartPow + distance));



        /****************************************
         *   상태 트리거에 따라서 효과를 적용한다...
         * ***/
        if (_bossCrab.StateTrigger == false) return;
        switch (_progress++){

                /**애니메이션 속도를 빠르게 한다....*/
                case (0):
                {
                    AISM.Animator.speed = 2f;
                    break;
                }

                /**집게를 소환한다...*/
                case (1):
                {
                    AISM.Animator.speed = 1f;

                    if (_handIns != null){

                        /**소환될 때의 진동을 적용한다...*/
                        CameraManager.GetInstance().CameraShake(.3f, CameraManager.ShakeDir.ROTATE, .5f);
                        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_BoomBurst);
                        IpariUtility.PlayGamePadVibration(.1f, .1f, .1f);

                        _handIns.gameObject.SetActive(true);
                        _handIns.targetTransform    = Player.Instance.transform;
                        _handIns.transform.position = new Vector3(-2.57999992f, 6.17999983f, -5.75f);
                        _handIns?.StartAttack(_handIns.transform.position);
                    }
                    break;
                }

                /**내려찍을 때의 딜레이를 준다...*/
                case (2):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_RiseIdle, .1f);
                    _bossCrab.SetStateTrigger(2f);
                    break;
                }

                /**내려찍는 애니메이션을 재생한다....*/
                case (3):
                {
                    _glowgoalPow    = _glowTime = 0f;
                    _glowStartPow   = _EgoMat.GetFloat("_alpha");
                    AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_Down, .1f);
                    break;
                }

                /**분신집게의 공격....*/
                case (4):
                {
                    _handIns.IsAttack = true;
                    break;
                }

                /**Idle로 지정한다...*/
                case (5):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .3f);
                    _bossCrab.SetStateTrigger(1f);
                    break;
                }

                /**상태를 빠져나간다...*/
                case (6):
                {
                    AISM.NextPattern();
                    break;
                }
        }

        _bossCrab.StateTrigger = false;
        #endregion
    }

    public override void Exit()
    {
        _EgoMat.SetFloat("_alpha", 0f);

    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
