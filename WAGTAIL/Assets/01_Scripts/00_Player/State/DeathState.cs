using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    /// <summary>
    /// ��� �� ����� ���� -10 �� üũ ����Ʈ�� �̵�
    /// ������ �������� 10 ���� ���� �� é���� �������� �̵�.
    /// üũ ����Ʈ�� �̵� �� ����� �� �͵�
    /// 
    /// 
    ///
    /// ===============================
    /// é���� �������� �̵� �� ����� �� �͵�
    /// 
    ///
    /// ===============================
    /// </summary>
    public DeathState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;    
    }

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
    }

    public override void Exit()
    {
        base.Exit();
        player.isDead = false;
    }

    // üũ����Ʈ�� ���� ��
    private void RemoveCheckPoint()
    {
        
    }
    
    // é�ͷ� ���� ��
    private void RemoveChapter()
    {
        
    }
    
    
}
