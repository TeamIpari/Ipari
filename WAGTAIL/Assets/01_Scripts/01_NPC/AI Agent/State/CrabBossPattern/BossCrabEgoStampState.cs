using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BossCrab;

public sealed class BossCrabEgoStampState : AIAttackState
{
    //======================================
    ////            Fields              ////
    //======================================
    private EgoCrabHand _handIns;
    private Material    _EgoMat;
    private Renderer    _renderer;
    private BossCrab    _bossCrab;
    private float       _power    = 0f;
    private float       _progress = 0;

    private float _idleDuration = 2f;


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabEgoStampState(AIStateMachine stateMachine, EgoCrabHand handIns, BossCrab bossCrab)
    : base(stateMachine)
    {
        #region Omit
        _handIns = handIns;
        _bossCrab = bossCrab;

        Transform arm      = AISM.Transform.Find("Boss_Crab_Con").Find("Crab_Body_Bone").Find("L_Tong01");
        Transform tong     = arm.Find("Bone011").Find("Bone012").Find("Bone012_internal");
        _renderer = tong.GetComponent<Renderer>();

        #endregion
    }

    public override void Enter()
    {
        _progress = 0;
        curTimer  = 0;
        AISM.Animator.speed = .5f;
        AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
    }

    public override void Update()
    {
        /*******************************************
         *   일정시간 후 내려친다...
         * ****/
        if(curTimer>0)
        {
            curTimer -= Time.deltaTime;

            /**타이머가 완료되었을 경우...*/
            if (curTimer <= 0f){

                switch (_progress++)
                {
                        /**내려찍는 애니메이션을 재생한다....*/
                        case (3):
                        {
                            AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
                            break;
                        }

                        /**상태를 빠져나간다...*/
                        case (6):
                        {
                            AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
                            AISM.NextPattern();
                            break;
                        }
                }
            }
        }

        /****************************************
         *   상태 트리거에 따라서 효과를 적용한다...
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

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

                        _handIns.gameObject.SetActive(true);
                        _handIns.targetTransform    = Player.Instance.transform;
                        _handIns.transform.position = _bossCrab.transform.position + (Vector3.up * 3f) + (_bossCrab.transform.forward * -2f);
                        _handIns?.StartCrabHand(AISM.character.transform.position);
                    }
                    break;
                }

                /**내려찍을 때의 딜레이를 준다...*/
                case (2):
                {
                    curTimer = _idleDuration;
                    break;
                }

                /**분신집게의 공격....*/
                case (4):
                {
                    _handIns.IsAttack = true;
                    curTimer = 1f;
                    break;
                }

                /**최종 대기시간 부여.*/
                case (5):
                {
                    curTimer = 1f;
                    break;
                }
        }

        _bossCrab.StateTrigger = false;

    }

    public override void Exit()
    {
        AISM.Animator.ResetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
