using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightState : State
{
    
    public FlightState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
    }
}
