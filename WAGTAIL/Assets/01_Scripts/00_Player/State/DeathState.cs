using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    private float _respawnTime;
    private float _currentTime;
    
    public DeathState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;    
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();
        player.UIManager.ActiveGameUI(GameUIType.Death, true);
        player.CameraManager.SwitchCamera(CameraType.Death);
        _respawnTime = player.respawnTime;
        _currentTime = 0;

        // ChapterRestart ���� �� ���� �ؾ���.
        //RemoveCheckPoint();
        // changeState�� player.idle�� AliveState�� �ٲ������ or Alive Animation ���.
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

        if (_currentTime >= _respawnTime)
        {
            RemoveCheckPoint();
            _currentTime = 0;
            stateMachine.ChangeState(player.idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.isDead = false;
    }

    // üũ����Ʈ�� ���� ��
    private void RemoveCheckPoint()
    {
        //player.UIManager.ActiveGameUI(GameUIType.Death, true);
        player.animator.Rebind();
        player.GameManager.Coin -= 5;
        //üũ ����Ʈ�� �̵� ���� �ؾ���
        player.GameManager.Respawn();
    }
    
    // é�ͷ� ���� ��
    private void RemoveChapter()
    {
        // é�� ����� �����ؾ���
    }
}
