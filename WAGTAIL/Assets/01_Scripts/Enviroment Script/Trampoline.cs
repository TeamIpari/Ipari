using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class Trampoline : MonoBehaviour, IEnviroment
{

    //public bool ishit;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public float JumpHeight;
    private float saveHeight;

    public Mushroom mushroom;

    public bool IsHit { get; set; }
    private bool move = false;

    public bool Interact()
    {
        // ����� �� Player�� �����ϰ� ��.
        Player.Instance.jumpHeight = JumpHeight;
        //Player.Instance.movementSM.ChangeState(Player.Instance.jump);
        if (Player.Instance.movementSM.currentState == Player.Instance.idle)
        {
            Player.Instance.idle.Jumping();
        }
        else if (Player.Instance.movementSM.currentState == Player.Instance.flight)
        {
            Player.Instance.flight.Jumping();
        }
        else if (Player.Instance.movementSM.currentState == Player.Instance.jump)
        {
            Player.Instance.jump.Jumping();
        }
        if (!move)
            move = true;
        StartCoroutine(BackJumpValue());
        
        return false;
    }

    /// <summary>
    /// n�� �� ������ ���� ���� �����ִ� ���.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackJumpValue()
    {
        yield return new WaitForSeconds(0.05f);
        Player.Instance.jumpHeight = saveHeight;

        if (mushroom != null)
        {
            mushroom.ChangeMushroom();
            mushroom.isMush = true;
        }

    }


    // Start is called before the first frame update
    private void Start()
    {
        saveHeight = Player.Instance.jumpHeight;
        //targetPos = heightPos.transform.position;
    }

    public float Curtime = 0;
    [SerializeField] private Transform lowPos;
    [SerializeField] private Transform heightPos;
    //Vector3 _localLow;
    //Vector3 _localHeight;
    [SerializeField] private Vector3 targetPos;
    public float MoveTime = 0;


    private void FixedUpdate()
    {
        //Moving();
        //if (move)
        //{
        //    Curtime += Time.deltaTime;
        //    // �ö���� �����.
        //    if (Curtime > MoveTime)
        //    {
        //        Moving();
        //        Curtime = 0;
        //    }
        //}
    }

    private void Moving()
    {
        if (move)
        {
            Curtime += Time.deltaTime;
            // �ö���� �����.
            if (Curtime > MoveTime)
            {     
                // �ʴ� n�� �ӵ��� ��ǥ�� ���� ������.
                if (Vector3.Distance(transform.position, targetPos) <= 1)
                {
                    targetPos = targetPos == heightPos.position ? lowPos.position : heightPos.position;
                    move = false;
                }
                else
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
                Curtime = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (lowPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(lowPos.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

        if (heightPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(heightPos.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }
    }

}
