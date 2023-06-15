using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity;
using UnityEngine;
//using UnityEditorInternal;

/// <summary>
/// 작성자 : 성지훈
/// 추가 작성
/// </summary>
public class DummyAttack1 : AIState
{
    private int curAnim = 0;


    private float curTimer = 0;
    private float changeTimer = 5f;

    private float curShowTimer = 0;
    private float showTimer = 0.5f;

    private bool on = false;
    private GameObject tentacle;
    private GameObject dangerousEffect;
    private int count = 0;


    public DummyAttack1(AIStateMachine stateMachine, GameObject tentacle, GameObject danger) : base(stateMachine)
    {
        //curTimer = 0;
        this.stateMachine = stateMachine;
        this.dangerousEffect = danger;
        this.tentacle = tentacle;
        curAnim = 0;
    }

    public override void Enter()
    {
        //dangerousEffect.SetActive(true);
        dangerousEffect.transform.position 
            = new Vector3
            (Player.Instance.transform.position.x, 
            Player.Instance.transform.position.y + 0.1f, 
            Player.Instance.transform.position.z);

        tentacle.transform.position = dangerousEffect.transform.position;

        Debug.Log("Start Attack1");
    }


    public override void Exit()
    {
        dangerousEffect.SetActive(false);
        Debug.Log("End Attack1");

    }

    public override void OntriggerEnter(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        curTimer += Time.deltaTime;
        switch(curAnim)
        {
            case 0:
                BlinkEffect();
                break;
            case 1:
                ShowTentacle();
                break;
            case 2:
                break;
            default:
                break;
        }
        //if(curTimer > changeTimer)
        //{
        //    // 공격 기능.
        //    if (children.Count > 0)
        //        stateMachine.ChangeState(children[current]);
        //    else if (parent != null)
        //        stateMachine.ChangeState(parent);
        //    else if (stateMachine.pattern.Count > 0)
        //        stateMachine.NextPattern();
        //    else
        //        Debug.Log("연결된 State가 없음.");

        //    curTimer = 0;
        //}
    }

    private void ShowTentacle()
    {
        tentacle.SetActive(true);
        tentacle.transform.LookAt(Player.Instance.transform.position);
        curAnim++;

    }

    // Effect가 점멸하는 기능.
    private void BlinkEffect()
    {
        curShowTimer += Time.deltaTime;
        if(curShowTimer >= showTimer)
        {
            if (on)
            {
                dangerousEffect.SetActive(false);
                on = false;
                count++;
            }
            else
            {
                dangerousEffect.SetActive(true);
                on = true;
            }
            curShowTimer = 0;
        }
        if(count > 3)
        {
            curAnim++;
        }
    }
}
