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

    /**dissolve 관련 필드....*/
    private const float  _dissolveDuration    = 3f;
    private const float  _dissolveDurationDiv = (1f/_dissolveDuration);
    private Material[]   _dissolveMats;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabHitState(AIStateMachine stateMachine, BossCrab bossCrab)
    : base(stateMachine)
    {
        _bossCrab = bossCrab;
    }

    public override void Enter()
    {
        #region Omit
        base.Enter();

        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Crab_BoomBurst);
        IpariUtility.PlayGamePadVibration(1f, 1f, .08f);

        _bossCrab.StateTrigger = false;
        _bossCrab.ClearStateTriggerDelay();
        _bossCrab.PopHPUIStack();

        Debug.Log($"데미지 입음!!!");

        AISM.Animator.speed = 1f;

        /************************************
         *   보스가 죽을 경우....
         * ****/
        if((AISM.character.HP-=10)<=0f)
        {
            _progress = 1;
            AISM.Animator.speed = 0f;
            AISM.Animator.Play(BossCrabAnimation.Die, 0, .05f);
            CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .8f, .022f);
            _bossCrab.SetStateTrigger(1f);
            return;
        }


        /****************************************
         *   보스가 데미지를 입었을 경우...
         * ****/
        AISM.Animator.CrossFade(BossCrabAnimation.Hit, .1f, 0, 0f);
        _progress = 0;
        _bossCrab.SetStateTrigger(.3f);
        CameraManager.GetInstance().CameraShake(.5f, CameraManager.ShakeDir.ROTATE, .6f, .022f);
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
         *   디졸브 효과를 적용한다....
         * *****/
        if (_progress >= 4 && _dissolveMats != null){

            float progressRatio = Mathf.Clamp((curTimer += Time.deltaTime) * _dissolveDurationDiv, 0f, 1.02f);
            _dissolveMats[0].SetFloat("_Dissolve", progressRatio);
            _dissolveMats[1].SetFloat("_Dissolve", progressRatio);
        }


        /*****************************************************
         *   상태 트리거가 유효하면, 상황에 따른 로직을 적용한다...
         * ****/
        if (_bossCrab.StateTrigger == false) return;

        switch(_progress++){

                /**다음 패턴으로 넘어간다....*/
                case (0):
                {
                    _bossCrab.StateTrigger = false;
                    AISM.NextPattern();
                    break;
                }

                /**데미지를 입고 몸을 올린다...*/
                case (1):
                {
                    _bossCrab.ShowHPUI(false, .6f);
                    AISM.Animator.speed = .8f;
                    CameraManager.GetInstance().CameraShake(1f, CameraManager.ShakeDir.ROTATE, .5f);
                    IpariUtility.PlayGamePadVibration(1f, 1f, .1f);
                    break;
                }

                /**쓰러진다...*/
                case (2):
                {
                    AISM.Animator.speed = 1f;
                    break;
                }

                /**쓰러진 후의 진동효과...*/
                case (3):
                {
                    /**먼지 이펙트를 생성한다....*/
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

                /**쓰러진 후 디졸브 효과를 적용한다.*/
                case (4):
                {
                    /**BossCrab의 Rednerer로부터 필요한 머터리얼을 가져온다...*/
                    Renderer renderer = AISM.Transform.Find("Boss_Crab_Mesh").GetComponent<Renderer>();
                    _dissolveMats     = renderer.materials;

                    curTimer = 0f;
                    _bossCrab.SetStateTrigger(_dissolveDuration+.1f);
                    break;
                }

                /**꽃게보스가 사라진다...*/
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
