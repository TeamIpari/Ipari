using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    private float _respawnTime;
    private float _currentTime;
    private bool _isAlive;

    private float gravityValue;
    private bool isGrounded;
    private float playerSpeed;
    
    public DeathState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;    
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.GameOver);
        //player.SoundHandler.SetTrigger("isDeath");
        _isAlive = false;
        isGrounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
        playerSpeed = player.playerSpeed;

        player.UIManager.ActiveGameUI(GameUIType.Death, true);
        //player.CameraManager.SwitchCamera(CameraType.Death);
        _respawnTime = player.respawnTime;
        _currentTime = 0;

        // ChapterRestart 만든 후 적용 해야함.
        //RemoveCheckPoint();
        // changeState의 player.idle은 AliveState로 바꿔줘야함 or Alive Animation 출력.
        // stateMachine.ChangeState(player.idle);
    }

    public override void HandleInput()
    {
        base.HandleInput();
        
        _currentTime += Time.deltaTime;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (_currentTime >= _respawnTime && !_isAlive)
        {
            RemoveCheckPoint();
            _currentTime = 0;
            //player.CameraManager.SwitchCamera(CameraType.Main);
            _isAlive = true;
            //stateMachine.ChangeState(player.idle);
        }

        else if( _currentTime >= _respawnTime ) 
        {
            stateMachine.ChangeState(player.idle);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = player.controller.isGrounded;

        // 바닥과 닿아 있을 때는 중력 적용 X
        if (isGrounded && gravityVelocity.y < 0)
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
        player.isDead = false;
        _isAlive = false;
        _currentTime = 0;
    }

    // 체크포인트로 보낼 시
    private void RemoveCheckPoint()
    {
        //player.UIManager.ActiveGameUI(GameUIType.Death, true);
        player.animator.Rebind();
        player.GameManager.Coin -= 5;
        //체크 포인트로 이동 구현 해야함
        player.GameManager.Respawn();
    }
    
    // 챕터로 보낼 시
    private void RemoveChapter()
    {
        // 챕터 재시작 구현해야함
    }
}
