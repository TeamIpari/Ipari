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


    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabEgoStampState(AIStateMachine stateMachine, EgoCrabHand handIns, BossCrab bossCrab)
    :base(stateMachine)
    {
        #region Omit
        _handIns = handIns;
        _bossCrab = bossCrab;

        try
        {
            Transform arm = AISM.Transform.Find("Boss_Crab_Con").Find("Crab_Body_Bone").Find("L_Tong01");
            Transform tong = arm.Find("Bone011").Find("Bone012").Find("Bone012_internal");
            _renderer = tong.GetComponent<Renderer>();
        }
        catch { Debug.LogWarning("BossCrabEgoStampState: Material�� ������ ���԰� �������� �ʽ��ϴ�."); }

        #endregion
    }

    public override void Enter()
    {
        _progress = 0;
        curTimer  = 0;
        AISM.Animator.speed = .5f;
        AISM.Animator.CrossFade(BossCrabAnimation.EgoTongAttack_TongRise, .3f);
    }

    public override void Update()
    {
        #region Omit
        /****************************************
         *   ���� Ʈ���ſ� ���� ȿ���� �����Ѵ�...
         * ***/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

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
                        _handIns.transform.position = _bossCrab.transform.position + (Vector3.up * 3f) + (_bossCrab.transform.forward * -2f);
                        _handIns?.StartCrabHand(AISM.character.transform.position);
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
        AISM.Animator.ResetTrigger(BossCrabAnimation.Trigger_IsEgoTongAttack);
    }

    public override void OntriggerEnter(Collider other)
    {
    }
}
