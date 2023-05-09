using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
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
        //ishit = true;
        // 밟았을 때 Player를 점프하게 함.
        saveHeight = Player.Instance.jumpHeight;
        Player.Instance.jumpHeight = jumpHeight;
        Player.Instance.idle.Jump();
        if (!move)
            move = true;
        StartCoroutine(rbJump());

        return false;
    }

    IEnumerator rbJump()
    {
        yield return new WaitForSeconds(0.001f);
        Player.Instance.jumpHeight = saveHeight;

    }


    // Start is called before the first frame update
    void Start()
    {
        _targetPos = _localHeight;
    }

    public float _curtime = 0;
    [SerializeField] Vector3 _lowPos;
    [SerializeField] Vector3 _heightPos;
    Vector3 _localLow;
    Vector3 _localHeight;
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
            _targetPos = _targetPos == _localHeight ? _localLow : _localHeight;
            move = false;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, 5f * Time.deltaTime);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        _localHeight = transform.position + _heightPos;
        _localLow = transform.position + _lowPos;
        Gizmos.DrawWireCube(_localHeight + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        Gizmos.DrawWireCube(_localLow + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
