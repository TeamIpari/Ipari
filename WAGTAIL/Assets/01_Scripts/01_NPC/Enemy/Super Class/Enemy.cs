using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public override void SetAttackPattern()
    {
        //if (AttackPattern.Length <= 0)
        //{
        //    AttackPattern = new Pattern[1];
        //    AttackPattern[0] = Pattern.NORMAL;
        //}

        //AiAttack = new AIAttackState[AttackPattern.Length];
        //base.SetAttackPattern();
    }

    public override void CAttack()
    {
        base.CAttack();
    }

}
