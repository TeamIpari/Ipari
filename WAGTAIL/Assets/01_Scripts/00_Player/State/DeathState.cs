using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    /// <summary>
    /// ��� �� ����� ���� -10 �� üũ ����Ʈ�� �̵�
    /// ������ �������� 10 ���� ���� �� é���� �������� �̵�.
    /// üũ ����Ʈ�� �̵� �� ����� �� �͵�
    /// ===============================
    private Transform _checkPoint;
    private GameObject _deathUI;
    
    /// ===============================
    /// é���� �������� �̵� �� ����� �� �͵�
        
    /// ===============================
    /// </summary>
    public DeathState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;    
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();

        

        if (player.coin > 0)
        {
            RemoveCheckPoint();
        }

        else if(player.coin == 0)
        {
            RemoveChapter();
        }
        
        // changeState�� player.idle�� AliveState�� �ٲ������ or Alive Animation ���.
        stateMachine.ChangeState(player.idle);
    }

    public override void Exit()
    {
        base.Exit();
        player.isDead = false;
    }

    // üũ����Ʈ�� ���� ��
    private void RemoveCheckPoint()
    {
        //üũ ����Ʈ�� �̵� ���� �ؾ���
        player.transform.position = _checkPoint.position;
        // UI ���̵��� ���̵� �ƿ�
        
    }
    
    // é�ͷ� ���� ��
    private void RemoveChapter()
    {
        // é�� ����� �����ؾ���
    }

    private void OnRespawnUI()
    {
        
    }

}
