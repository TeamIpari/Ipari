using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************************************
 *   플레이어가 아무런 행동을 못하는 state입니다.
 * ***/
public sealed class StiffenState : State
{
    //=======================================================
    /////             Property and fields              //////
    //=======================================================
    public float StiffenTime     = 1f;
    public float StiffenLeft     = 1f;



    //=====================================================
    //////           Override methods                //////
    //=====================================================
    public StiffenState(Player _player, StateMachine _stateMachine)
    :base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StiffenLeft = StiffenTime;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(StiffenTime>0){

            StiffenTime -= Time.deltaTime;

            /**플레이어의 경직이 해제된다....*/
            if(StiffenTime<0)
            {
                player.movementSM.ChangeState(player.idle);
            }

        }
    }
}
