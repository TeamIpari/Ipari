using System.Collections;
using System.Collections.Generic;
using static BossCrab;
using UnityEngine;
using IPariUtility;

public sealed class BossCrabHitState : AIHitState
{
    //===============================================
    //////               Fields                //////
    //===============================================
    private BossCrab     _bossCrab;
    private int          _progress            = 0;

    /**dissolve ���� �ʵ�....*/
    private const float  _dissolveDuration    = 3f;
    private const float  _dissolveDurationDiv = (1f/_dissolveDuration);
    private Material[]   _dissolveMats;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabHitState(AIStateMachine stateMachine, BossCrab bossCrab)
    : base(stateMachine)
    {
        #region Omit
        _bossCrab = bossCrab;

        /**BossCrab�� Rednerer�κ��� �ʿ��� ���͸����� �����´�...*/
        Renderer renderer = AISM.Transform.Find("Boss_Crab_Mesh").GetComponent<Renderer>();
        _dissolveMats = renderer.sharedMaterials;
        _dissolveMats[0].SetFloat("_Dissolve", 0f);
        _dissolveMats[1].SetFloat("_Dissolve", 0f);
        #endregion
    }

    public override void Enter()
    {
        #region Omit
        base.Enter();

        AISM.Animator.speed = 1f;
        _bossCrab.CurrentSuperArmorCount = _bossCrab.GetSuperArmorGainCount;

        /************************************
         *   ������ ���� ���....
         * ****/
        if(AISM.character.HP<=0f)
        {
            _dissolveMats[0].SetFloat("_Dissolve", 0f);
            _dissolveMats[1].SetFloat("_Dissolve", 0f);

            _progress = 1;
            AISM.Animator.speed = 0f;
            AISM.Animator.Play(BossCrabAnimation.Die, 0, .32f);
            CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .8f, .022f);
            _bossCrab.SetStateTrigger(1f);
            return;
        }


        /****************************************
         *   ������ �������� �Ծ��� ���...
         * ****/
        AISM.Animator.CrossFade(BossCrabAnimation.Hit, .1f, 0, 0f);
        _progress = 0;
        _bossCrab.SetStateTrigger(.3f);
        CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .6f, .027f);
        #endregion
    }

    public override void Exit()
    {
        base.Exit();
        //AISM.character.IsHit = false;
    }

    public override void Update()
    {
        #region Omit
        /************************************************
         *   ������ ȿ���� �����Ѵ�....
         * *****/
        if (_progress > 4 && _dissolveMats != null){

            float progressRatio = Mathf.Clamp((curTimer += Time.deltaTime) * _dissolveDurationDiv, 0f, 1.02f);
            _dissolveMats[0].SetFloat("_Dissolve", progressRatio);
            _dissolveMats[1].SetFloat("_Dissolve", progressRatio);
        }


        /*****************************************************
         *   ���� Ʈ���Ű� ��ȿ�ϸ�, ��Ȳ�� ���� ������ �����Ѵ�...
         * ****/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**���� �������� �Ѿ��....*/
                case (0):
                {
                    _bossCrab.StateTrigger = false;
                    AISM.NextPattern();
                    break;
                }

                /**�������� �԰� ���� �ø���...*/
                case (1):
                {
                    _bossCrab.ShowHPUI(false, .6f);
                    AISM.Animator.speed = .8f;
                    CameraManager.GetInstance().CameraShake(1f, CameraManager.ShakeDir.ROTATE, .5f);
                    IpariUtility.PlayGamePadVibration(1f, 1f, .1f);
                    break;
                }

                /**��������...*/
                case (2):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**������ ���� ����ȿ��...*/
                case (3):
                {
                    /**���� ����Ʈ�� �����Ѵ�....*/
                    if(_bossCrab.DeathSFXPrefab)
                    {
                        GameObject newSFX           = GameObject.Instantiate(_bossCrab.DeathSFXPrefab);
                        newSFX.transform.position   = new Vector3(-0.25f, -0.0100011826f, -1.3f);
                        GameObject.Destroy(newSFX, 4f); 
                    }

                    FModAudioManager.PlayOneShotSFX(

                        FModSFXEventType.Crab_Atk3Smash, 
                        _bossCrab.transform.position, 
                        2f
                    );

                    FModAudioManager.ApplyBGMFade(0f, 3f, 0, true);
                    IpariUtility.PlayGamePadVibration(.6f, .6f, .5f);
                    CameraManager.GetInstance().CameraShake(.6f, CameraManager.ShakeDir.ROTATE, 1f);

                    _bossCrab.SetStateTrigger(2.5f);
                    break;
                }

                /**������ �� ������ ȿ���� �����Ѵ�.*/
                case (4):
                {
                    curTimer = 0f;
                    _bossCrab.SetStateTrigger(_dissolveDuration);
                    break;
                }

                /**�ɰԺ����� �������...*/
                case (5):
                {
                    _bossCrab.DropableHorn?.DropItem();
                    GameObject.Destroy(_bossCrab.gameObject);
                    break;
                }

        }

        _bossCrab.StateTrigger = false;

        #endregion
    }
}
