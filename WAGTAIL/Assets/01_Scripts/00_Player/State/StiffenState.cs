using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************************************
 *   �÷��̾ �ƹ��� �ൿ�� ���ϴ� state�Դϴ�.
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

            /**�÷��̾��� ������ �����ȴ�....*/
            if(StiffenTime<0)
            {
                player.movementSM.ChangeState(player.idle);
            }

        }
    }
}
