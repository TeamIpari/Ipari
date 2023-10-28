using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static BossCrab;

public sealed class BossCrabEgoStampState : AIAttackState
{
    //======================================
    ////            Fields              ////
    //======================================

    /**���� ����...*/
    private BossCrab       _bossCrab;
    private ParticleSystem _collectFX;
    private EgoCrabHand    _handIns;
    private Material       _EgoMat;
    private Renderer       _renderer;

    /**���� ����...*/
    private float          _glowgoalPow  = 0f;
    private float          _glowTimeDiv  = 1f;
    private float          _glowTime     = 0f;
    private float          _glowStartPow = 0f;

    private float          _progress    = 0;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabEgoStampState(AIStateMachine stateMachine, EgoCrabHand handIns, BossCrab bossCrab, float collectDuration=1f)
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
             *   �ɰ��� ���Կ� ���� ������ �����´�....
             * ***/
            Transform arm  = AISM.Transform.Find("Boss_Crab_Con").Find("Crab_Body_Bone").Find("L_Tong01");
            Transform tong = arm.Find("Bone011").Find("Bone012").Find("Bone012_internal");
            _renderer      = tong.GetComponent<Renderer>();


            /***************************************************
             *   ������ ���͸��� ���� ���纻�� ������ �����´�.... 
             * ****/
            _EgoMat = _renderer.materials[1];
            _EgoMat.SetFloat("_alpha", 0f);


            /******************************************************
             *   �⸦ ������ ����Ʈ�� ���� ������ �����´�...
             * ***/
            _collectFX = tong.Find("Cras ( ready) Ver.3 180").GetComponent<ParticleSystem>();
            _collectFX.gameObject.SetActive(false);

        }
        catch { Debug.LogWarning("BossCrabEgoStampState: ������ �������µ� �����Ͽ����ϴ�..."); }

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
         *   ������ ���͸����� ���ϱ⸦ �����Ѵ�...
         * ****/
        float progressRatio = Mathf.Clamp01((_glowTime+=Time.deltaTime) * _glowTimeDiv);
        float distance      = (_glowgoalPow - _glowStartPow)*progressRatio;

        _EgoMat.SetFloat("_alpha",  (_glowStartPow + distance));



        /****************************************
         *   ���� Ʈ���ſ� ���� ȿ���� �����Ѵ�...
         * ***/
        if (_bossCrab.StateTrigger == false) return;
        switch (_progress++){

                /**�ִϸ��̼� �ӵ��� ������ �Ѵ�....*/
                case (0):
                {
                    AISM.Animator.speed = 2f;
                    break;
                }

                /**���Ը� ��ȯ�Ѵ�...*/
                case (1):
                {
                    AISM.Animator.speed = 1f;

                    if (_handIns != null){

                        _handIns.gameObject.SetActive(true);
                        _handIns.targetTransform    = Player.Instance.transform;
                        _handIns.transform.position = new Vector3(-2.57999992f, 6.17999983f, -5.75f);
                        _handIns?.StartCrabHand(_handIns.transform.position);
                    }
                    break;
                }

                /**�������� ���� �����̸� �ش�...*/
                case (2):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_RiseIdle, .1f);
                    _bossCrab.SetStateTrigger(2f);
                    break;
                }

                /**������� �ִϸ��̼��� ����Ѵ�....*/
                case (3):
                {
                    _glowgoalPow    = _glowTime = 0f;
                    _glowStartPow   = _EgoMat.GetFloat("_alpha");
                    AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_Down, .1f);
                    break;
                }

                /**�н������� ����....*/
                case (4):
                {
                    _handIns.IsAttack = true;
                    break;
                }

                /**Idle�� �����Ѵ�...*/
                case (5):
                {
                    AISM.Animator.CrossFade(BossCrabAnimation.Idle, .3f);
                    _bossCrab.SetStateTrigger(1f);
                    break;
                }

                /**���¸� ����������...*/
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
