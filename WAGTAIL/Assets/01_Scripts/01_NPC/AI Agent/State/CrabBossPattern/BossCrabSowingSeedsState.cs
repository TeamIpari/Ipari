using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BossCrabSowingSeedsState : BossNepenthesSmallShotGun
{
    //==================================================
    //////        Public and Override methods       ////
    //==================================================
    public BossCrabSowingSeedsState( AIStateMachine       stateMachine,
                                     BossNepenthesProfile profile,
                                     float  flightTime,
                                     int    count,
                                     float  rad )

    :base( stateMachine, profile,flightTime,count,rad)
    {
    }

}
