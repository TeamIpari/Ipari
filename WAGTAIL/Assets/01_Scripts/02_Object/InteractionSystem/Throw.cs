using System;
using System.Collections;
using System.Net;
using Unity.VisualScripting;
//using UnityEditorInternal;
using UnityEngine;

public class Throw : MonoBehaviour, IInteractable
{
    [Header("Interactable Setting")]
    [SerializeField] private string _promt;
    private GameObject _playerEquipPoint;
    private GameObject _playerInteractionPoint;

    [Header("Throw Setting")]
    [SerializeField] float _force = 1.0f;
    [SerializeField] float _yForce = 1.0f;
    [Range(0, 1)]
    [SerializeField] float _yAngle = 0.45f;
    [Range(0, 5)]
    [SerializeField] float _value = 0.0f;
    [SerializeField] float _hight = 0.0f;
    [SerializeField] float _range = 0.0f;
    [SerializeField] private Transform autoTarget;
    [Range(0,0.25f)]
    [SerializeField] float _overheadSpeed = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;
    Vector3 startPos;
    Vector3 height;
    Vector3 endPos;

    bool physicsCheck = false;
    public string InteractionPrompt => _promt;
    private Animator _animator;

    private void Start()
    {
        _playerEquipPoint = Player.Instance.ThrowEquipPoint.gameObject;
        _playerInteractionPoint = Player.Instance.InteractionPoint.gameObject;
         startPos = this.transform.position;
        _animator = GetComponent<Animator>();
    }

    public bool AnimEvent()
    {
        return false;
    }

    public bool Interact(Interactor interactor)
    {
        if(_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle)
        {
            StartCoroutine(Pickup());
            // isCarry를 isThrow로 바꿔줘야함
            interactor.player.isCarry = true;
            
            return true;
        }

        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            //Throwing(interactor);
            StartCoroutine(Throwing(interactor));
            interactor.player.isCarry = false;
            return true;
        }

        return false;
    }
    private void OnDrawGizmos()
    {

    }

    private void Update()
    {
        Debug.DrawRay(transform.position, _playerInteractionPoint.transform.forward * 10, Color.red);
        if (physicsCheck)
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, -transform.up, Color.red);

            if (Physics.Raycast(transform.position, -transform.up, out hit, .3f)
                && (!hit.transform.gameObject.CompareTag("Player")
                    && !hit.transform.gameObject.CompareTag("PassCollision")
                    && hit.transform.gameObject.layer != 5))
            {
                Debug.Log("IsGround");
                Debug.Log(hit.transform.name);
                Debug.Log(hit.transform.gameObject.tag);
                Debug.Log(hit.transform.gameObject.layer);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                _animator.SetTrigger("Grounded");
                physicsCheck = false;
            }
            else if(!hit.transform.gameObject.CompareTag("Player"))
            {

            }
        }
        else
        { 
            //try
            //{
            //    GetComponent<Rigidbody>().velocity = Vector3.zero;

            //}
            //catch
            //{

            //}
        }
    }

    IEnumerator Pickup()
    {
        transform.position = new Vector3(_playerInteractionPoint.transform.position.x, transform.position.y, _playerInteractionPoint.transform.position.z) ;
        startPos = transform.position;
        _value = 0;

        // 방향벡터를 구함.
        Vector3 lookvec = Player.Instance.transform.position - transform.position;
        lookvec = lookvec.normalized;
        
        // 꺾이는 지점.
        height = new Vector3(startPos.x, _playerEquipPoint.transform.position.y -.5f, startPos.z) + lookvec * 0.5f;
        // 머리 위
        endPos = new Vector3(_playerEquipPoint.transform.position.x, _playerEquipPoint.transform.position.y - 0.5f, _playerEquipPoint.transform.position.z);
        Debug.Log(lookvec);
        //GameObject obj = new GameObject("방향벡터");
        //obj.transform.position = height;

        // Object가 Player의 머리 위에서 움직이는걸 방지
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(0.75f);

        if (_animator != null)
            _animator.SetTrigger("Caught");

        while (_value <= 1)
        {
            yield return new WaitForSeconds(_overheadSpeed / 1000f);
            transform.transform.position = (BeziurCurve(_value));
            _value += 0.02f;
        }

        // Object를 Player의 머리 위로 옮김
        transform.SetParent(_playerEquipPoint.transform);
        GetComponent<Rigidbody>().isKinematic = true;

        // interactionPoint를 머리 위로 옮김
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;

    }

    Vector3 BeziurCurve( float _value)
    {
        Vector3 A = Vector3.Lerp(startPos, height, _value);

        Vector3 B = Vector3.Lerp(height, endPos, _value);

        Vector3 C = Vector3.Lerp(A, B, _value);

        return C;
    }

    IEnumerator Throwing(Interactor interactor)
    {
        //_animator.enabled = false;
        if(autoTarget != null)
        {

            Player.Instance.GetComponent<CharacterController>().enabled = false;

            Player.Instance.transform.LookAt(autoTarget);
            Player.Instance.GetComponent<CharacterController>().enabled = true;

        }
        yield return new WaitForSeconds(0.2f);
        if (_animator != null)
            _animator.SetTrigger("Flight");
        // interactionPoint의 Position을 초기상태로 되돌림
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;

        // Object 종속을 풀어줌
        _playerEquipPoint.transform.DetachChildren();

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //GetComponent<Rigidbody>().constraints = ;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        // 정한 방식대로 날라감
        //_playerForwardTransform = interactor.player.transform.forward;
        //_playerForwardTransform.x *= _force;
        //_playerForwardTransform.y = _yForce * _yAngle;
        //_playerForwardTransform.z *= _force;
        if (autoTarget == null)
            GetComponent<Rigidbody>().velocity = CaculateVelocity(interactor.player.transform.position + interactor.player.transform.forward * _range, this.transform.position, _hight);
        else if (autoTarget != null)
            GetComponent<Rigidbody>().velocity = CaculateVelocity(autoTarget.position, this.transform.position, _hight);

        if (_animator != null)
            physicsCheck = true;
    }

    public void SetAutoTarget(Transform _transform = null)
    {
        autoTarget = _transform;
    }

    private Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        // define the distance x and y first;
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
        distanceXZ.y = 0f; // y는 0으로 설정.

        // Create a float the represent our distance
        float Sy = distance.y;      // 세로 높이의 거리를 지정.
        float Sxz = distanceXZ.magnitude;

        // 속도 추가
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }

}
