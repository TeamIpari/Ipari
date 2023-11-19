using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    private float _respawnTime;
    private float _currentTime;
    private bool _isAlive;

    private float _gravityValue;
    private float _playerSpeed;
    
    public DeathState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;    
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();
        InterativeUI.HideUI();
        if (player.currentInteractable != null)
        {
            player.currentInteractable.GetComponent<ThrowObject>().ResetPoint();
        }
        player.isDead = false;
        GameManager.GetInstance().deathCount++;
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.GameOver);
        _isAlive = false;
        _gravityValue = player.gravityValue;
        _playerSpeed = player.playerSpeed;

        if (player.UIManager == null) return;

        player.UIManager.ActiveGameUI(GameUIType.Death, true);
        _respawnTime = player.respawnTime;
        _currentTime = 0;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        _currentTime += Time.deltaTime;
        
        if (_currentTime >= _respawnTime && !_isAlive)
        {
            RemoveCheckPoint();
            _currentTime = 0;
            _isAlive = true;
        }

        else if( _currentTime >= _respawnTime ) 
        {
            stateMachine.ChangeState(player.idle);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += _gravityValue * Time.deltaTime;

        // 바닥과 닿아 있을 때는 중력 적용 X
        if (player.controller.isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        if (_isAlive)
        {
            player.controller.Move(1.5f * gravityVelocity * Time.deltaTime);
        }

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity),
                player.rotationDampTime);
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (player.UIManager != null)
            player.UIManager.GetGameUI(GameUIType.Death).GetComponent<Animator>().SetTrigger("fadeout");
        player.isCarry = false;
        player.isFlight = false;
        player.isIdle = false;
        player.isPull = false;
        player.isDead = false;
        _isAlive = false;
        _currentTime = 0;
        player.invincibleDurTime = player.invincibleTime;
    }

    // 체크포인트로 보낼 시
    private void RemoveCheckPoint()
    {
        //player.UIManager.ActiveGameUI(GameUIType.Death, true);
        player.animator.Rebind();
        player.GameManager.Coin -= 5;
        ChildPlatformsShaker obj = GameObject.Find("Floor2")?.GetComponent<ChildPlatformsShaker>();
        //체크 포인트로 이동 구현 해야함
        if (obj != null)
            player.GameManager.WrapPlayerPosition(obj.GetWavePlatform().position + Vector3.up * 2f);
        else
            player.GameManager.Respawn();
    }
    
    // 챕터로 보낼 시
    private void RemoveChapter()
    {
        // 챕터 재시작 구현해야함
    }
}
