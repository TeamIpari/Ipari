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
         *   �����ð� �� ����ģ��...
         * ****/
        if(curTimer>0)
        {
            curTimer -= Time.deltaTime;

            /**Ÿ�̸Ӱ� �Ϸ�Ǿ��� ���...*/
            if (curTimer <= 0f){

                switch (_progress++)
                {
                        /**������� �ִϸ��̼��� ����Ѵ�....*/
                        case (3):
                        {
                            AISM.Animator.SetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
                            break;
                        }

                        /**���¸� ����������...*/
                        case (5):
                        {
                            AISM.NextPattern();
                            break;
                        }
                }
            }
        }

        /****************************************
         *   ���� Ʈ���ſ� ���� ȿ���� �����Ѵ�...
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**�ִϸ��̼� �ӵ��� ������ �Ѵ�....*/
                case (0):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**���Ը� ��ȯ�Ѵ�...*/
                case (1):
                {
                    AISM.Animator.speed = 1f;

                    if (_handIns != null){

                        _handIns.gameObject.SetActive(true);
                        _handIns.targetTransform = Player.Instance.transform;
                        _handIns?.StartCrabHand(AISM.character.transform.position);
                    }
                    break;
                }

                /**�������� ���� �����̸� �ش�...*/
                case (2):
                {
                    curTimer = _idleDuration;
                    break;
                }

                /**�н������� ����....*/
                case (4):
                {
                    _handIns.IsAttack = true;
                    curTimer = 1f;
                    break;
                }

                /**���� ���ð� �ο�.*/
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
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
