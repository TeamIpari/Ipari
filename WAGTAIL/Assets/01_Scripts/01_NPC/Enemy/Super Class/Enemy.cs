using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float AttackRange;
    public float AttackRate;
    protected float AttackTimer;

    // States 
    public AIIdleState AiIdle;

    public AIAttackState AiAttack;
    public AIMoveState AiMove;
    
    
}
