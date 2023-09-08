using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public AIAttackState AiAttack2;
    public AIAttackState AiAttack3;

    public override void SetAttackPattern()
    {
    }

    protected override void AddPattern(AIState curPattern)
    {
        AiSM.AddPattern(curPattern);
    }

    public override void SettingPattern(MonsterPattern.Pattern[] _pattern)
    {
        base.SettingPattern(_pattern);

        if (_pattern.Length <= 0)
        {
            Debug.LogWarning("������ ���� ������ �������� �ʾҽ��ϴ�.");
        }
        this.curPattern = _pattern;
        AiSM.ClearPattern();

        for (int i = 0; i < _pattern.Length; i++)
        {
            switch (_pattern[i])
            {
                case MonsterPattern.Pattern.IDLE:
                    AddPattern(AiIdle);
                    break;
                case MonsterPattern.Pattern.MOVE:
                    //AiSM.AddPatern(AiMove);
                    Debug.Log("Move�� �������� ����.");
                    break;
                case MonsterPattern.Pattern.WAIT:
                    AddPattern(AiWait);
                    Debug.Log("Wait�� �������� ����.");
                    break;
                case MonsterPattern.Pattern.SPECAIL1:
                    AddPattern(AiAttack);
                    break;
                case MonsterPattern.Pattern.SPECAIL2:
                    AddPattern(AiAttack2);
                    break;
                case MonsterPattern.Pattern.SPECAIL3:
                    AddPattern(AiAttack3);
                    break;
                case MonsterPattern.Pattern.DIE:
                    //AddPattern(Die);
                    Debug.Log("Die�� �������� ����.");
                    break;
                default:
                    break;
            }
        }
    }

    public override void CAttack()
    {
        base.CAttack();
    }

}
