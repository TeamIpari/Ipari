using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    /// <summary>
    /// 사망 시 용기의 구슬 -5 및 체크 포인트로 이동
    /// 구슬의 보유량이 0 보다 작을 시 챕터의 시작으로 이동.

    /// ===============================
    /// 챕터의 시작으로 이동 시 해줘야 할 것들
        
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
        
        /*
        if (player.coin > 0)
        {
            RemoveCheckPoint();
        }

        else if(player.coin == 0)
        {
            RemoveChapter();
        }*/
        RemoveCheckPoint();
        player.animator.Rebind();
        player.GameManager.Coin -= 5;
        // changeState의 player.idle은 AliveState로 바꿔줘야함 or Alive Animation 출력.
        stateMachine.ChangeState(player.idle);
    }

    public override void Exit()
    {
        base.Exit();
        player.isDead = false;
    }

    // 체크포인트로 보낼 시
    private void RemoveCheckPoint()
    {
        //체크 포인트로 이동 구현 해야함
        player.GameManager.Respawn();
        // UI 패이드인 패이드 아웃
        player.UIManager.ActiveGameUI(GameUIType.Death, true);
    }
    
    // 챕터로 보낼 시
    private void RemoveChapter()
    {
        // 챕터 재시작 구현해야함
    }
}
