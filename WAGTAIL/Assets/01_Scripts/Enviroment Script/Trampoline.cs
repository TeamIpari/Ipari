using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Trampoline : MonoBehaviour, IEnviroment
{

    public bool ishit;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public float jumpHeight;
    float saveHeight;
    

    public bool _hit { get { return ishit; } set { ishit = value; } }
    private bool move = false;

    public bool Interact()
    {
        //cot++;
        //ishit = true;
        // 밟았을 때 Player를 점프하게 함.
        Player.Instance.jumpHeight = jumpHeight;
        Player.Instance.movementSM.ChangeState(Player.Instance.jump);
        //if(Player.Instance.movementSM.currentState == Player.Instance.idle)
        //{
        //    Player.Instance.idle.Jump();
        //}
        //else if(Player.Instance.movementSM.currentState == Player.Instance.flight)
        //{
        //    Player.Instance.flight.Jump();
        //}
        if (!move)
            move = true;
        StartCoroutine(rbJump());

        return false;
    }

    IEnumerator rbJump()
    {
        yield return new WaitForSeconds(0.05f);
        Player.Instance.jumpHeight = saveHeight;

    }


    // Start is called before the first frame update
    void Start()
    {

        saveHeight = Player.Instance.jumpHeight;
        _targetPos = _heightPos.transform.position;
    }

    public float _curtime = 0;
    [SerializeField] Transform _lowPos;
    [SerializeField] Transform _heightPos;
    //Vector3 _localLow;
    //Vector3 _localHeight;
    [SerializeField] Vector3 _targetPos;
    public float _moveTime = 0;
    public float _rbTime = 0;


    private void FixedUpdate()
    {
        if(move)
        {
            _curtime += Time.deltaTime;
            // 올라오게 만들기.
            if (_curtime > _moveTime)
            {
                Moving();
                _curtime = 0;
            }
        }

    }

    void Moving()
    {
        // 초당 n의 속도로 목표를 향해 움직임.
        if (Vector3.Distance(transform.position, _targetPos) <= 1)
        {
            _targetPos = _targetPos == _heightPos.position ? _lowPos.position : _heightPos.position;
            move = false;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, 5f * Time.deltaTime);
    }
    
    private void OnDrawGizmos()
    {
        if (_lowPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_lowPos.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

        if (_heightPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_heightPos.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
