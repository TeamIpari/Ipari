using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class Trampoline : MonoBehaviour, IEnviroment
{

    //public bool ishit;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public float JumpHeight;
    private float saveHeight;
    

    public bool IsHit { get; set; }
    private bool move = false;

    public bool Interact()
    {
        // 밟았을 때 Player를 점프하게 함.
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
        StartCoroutine(rbJump());

        return false;
    }

    private IEnumerator rbJump()
    {
        yield return new WaitForSeconds(0.05f);
        Player.Instance.jumpHeight = saveHeight;

    }


    // Start is called before the first frame update
    private void Start()
    {

        saveHeight = Player.Instance.jumpHeight;
        targetPos = heightPos.transform.position;
    }

    public float _curtime = 0;
    [SerializeField] private Transform lowPos;
    [SerializeField] private Transform heightPos;
    //Vector3 _localLow;
    //Vector3 _localHeight;
    [SerializeField] private Vector3 targetPos;
    public float MoveTime = 0;
    public float RbTime = 0;


    private void FixedUpdate()
    {
        if(move)
        {
            _curtime += Time.deltaTime;
            // 올라오게 만들기.
            if (_curtime > MoveTime)
            {
                Moving();
                _curtime = 0;
            }
        }

    }

    private void Moving()
    {
        // 초당 n의 속도로 목표를 향해 움직임.
        if (Vector3.Distance(transform.position, targetPos) <= 1)
        {
            targetPos = targetPos == heightPos.position ? lowPos.position : heightPos.position;
            move = false;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
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

    // Update is called once per frame
    private void Update()
    {
        
    }
}
