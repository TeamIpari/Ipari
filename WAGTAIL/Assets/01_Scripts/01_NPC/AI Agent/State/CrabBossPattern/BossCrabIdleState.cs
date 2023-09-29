using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class BossCrabIdleState : AIIdleState
{
    //====================================
    //////          Fields            ////
    //====================================
    private float _waitTime = 0f;
    private float _currTime = 0f;



    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabIdleState(AIStateMachine stateMachine, float waitTime)
    : base(stateMachine)
    {
        _waitTime = waitTime;
    }

    public override void Enter()
    {
        base.Enter();
        _currTime = 0f;
    }

    public override void Update()
    {
        base.Update();
        _currTime += Time.deltaTime;

        /**크랩 보스의 죽음상태 전환...*/
        if(AISM.character.isDeath){

            AISM.ChangeState(AISM.character.AiDie);
            return;
        }

        /**크랩 보스의 히트 상태 전환...*/
        else if(AISM.character.IsHit){

            AISM.ChangeState(AISM.character.AiHit);
            return;
        }

        /**다음 패턴으로 넘어간다...*/
        if(_currTime>_waitTime){

            AISM.NextPattern();
        }
    }

}
