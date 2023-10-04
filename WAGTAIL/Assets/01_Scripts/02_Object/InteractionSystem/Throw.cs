using System;
using System.Collections;
using UnityEngine;
using IPariUtility;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Throw : MonoBehaviour, IInteractable
{
    [Header("Interactable Setting")]
    // 추가한 스크립트
    [SerializeField] private bool _isSmall;
    [SerializeField] private string _promt;
    private GameObject _playerEquipPoint;
    private Transform _playerEquipPos;
    private GameObject _playerInteractionPoint;
    private Transform _playerInteractionPos;
    private GameObject center;
    private Rigidbody _rigidbody;
    private GameObject _playerHead;
    private Transform _playerHeadPos;

    [Header("Throw Setting")]
    [Range(0, 5)]
    [SerializeField] float _value = 0.0f;
    [SerializeField] float _height = 0.0f;
    [SerializeField] float _range = 0.0f;
    [SerializeField] private Transform autoTarget;
    [Range(0,0.25f)]
    [SerializeField] float _overheadSpeed = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;
    Vector3 startPos;
    Vector3 height;
    Vector3 endPos;
    Vector3 spawnPoint;

    public bool PhysicsCheck = false;
    public string InteractionPrompt => _promt;
    private Animator _animator;
    // 일단 테스트 용 변수
    private float curTime;
    [SerializeField] private bool flight = false;
    [SerializeField] private Vector3 Forward;
    [SerializeField] private float speed = 1.0f;
    private Vector3 bounceDir;



    public bool AnimEvent()
    {
        return false;
    }

    //private void CheckRay()
    //{
    //    RaycastHit hit;
    //    bool bRangeHit = Physics.Raycast(transform.position, -transform.up, out hit, 0.2f);
    //    bool bTagHit = !hit.transform.gameObject.CompareTag("Player")
    //        && !hit.transform.gameObject.CompareTag("PassCollision");
    //    bool bLayerHit = hit.transform.gameObject.layer != 5;

    //    if (bRangeHit && bTagHit && bLayerHit)
    //    {
    //        _rigidbody.velocity = Vector3.zero;
    //        if (_animator != null)
    //            _animator.SetTrigger("Grounded");
    //    }
    //}

    //private void CheckVelocity()
    //{
    //    if (_rigidbody.velocity == Vector3.zero && _rigidbody.isKinematic == false )
    //    {
    //        //rigidbody.isKinematic = true;
    //        //rigidbody.drag = GetComponent<BombObject>() == null ? Mathf.Infinity : 0;
    //    }
    //}


    #region "완료"


    public bool Interact(GameObject interactor)
    {
        if (Player.Instance.currentInteractable == null)
        {
            StartCoroutine(PickUp(2f, 2.5f));
            Player.Instance.isCarry = true;
        }
        else
        {
            StartCoroutine(Throwing(interactor));
            Player.Instance.isCarry = false;
            return true;
        }
        return false;
    }

    private void PhyscisChecking()
    {
        if (PhysicsCheck)
        {
            _rigidbody.velocity += Physics.gravity * .05f;
        }
        //if (flight)
        //{
        //    this.transform.RotateAround(center.transform.position, Forward, (speed * Time.deltaTime));
        //}
    }

    private void FixedUpdate()
    {
        PhyscisChecking();
    }

    private void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();

        _playerInteractionPoint = Player.Instance.InteractionPoint.gameObject;
        _playerInteractionPos = _playerInteractionPoint.transform;
        _playerEquipPoint = Player.Instance.ThrowEquipPoint.gameObject;
        _playerEquipPos = _playerEquipPoint.transform;
        _playerHead = Player.Instance.Head;
        _playerHeadPos = _playerHead.transform;
    }

    IEnumerator PickUp(float lerpTime, float pickUpTime)
    {
        Player.Instance.isPickup = true;

        // Object가 Player의 머리 위에서 움직이는걸 방지
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.freezeRotation = true;
        _rigidbody.isKinematic = true;
        PhysicsCheck = false;
        flight = false;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;

        var pos = transform.position;
        var currentTime = 0.0f;

        while (currentTime < lerpTime)
        {
            transform.position = Vector3.Lerp(pos, _playerInteractionPos.position, currentTime / lerpTime);
            currentTime += 0.4f;
            yield return new WaitForSecondsRealtime(0.017f);
        }
        yield return new WaitForSecondsRealtime(0.3f);

        // BezierCurve를 위한 3개의 점 구하기 StartPos, Height, EndPos
        startPos = transform.position;
        var lookVec = (Player.Instance.transform.position - transform.position).normalized;
        height = new Vector3(startPos.x, _playerEquipPos.position.y - 0.5f, startPos.z) + lookVec * 0.5f;
        endPos = _playerEquipPos.position;
        endPos.y -= 0.25f;

        // 머리 위로 드는 곡선을 그리는 코루틴
        currentTime = 0.0f;
        while (currentTime < pickUpTime)
        {
            transform.position = BezierCurve(startPos, endPos, height, currentTime / pickUpTime);
            currentTime += 0.4f;
            yield return new WaitForSecondsRealtime(0.017f);
        }
        transform.SetParent(_playerHead.transform);

    }

    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);

        return c;
    }

    IEnumerator Throwing(GameObject interactor)
    {
        // Object 종속을 풀어줌
        transform.SetParent(null);
        if (Player.Instance.target != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.LookAt(
                new Vector3(Player.Instance.target.transform.position.x,
                Player.Instance.transform.position.y,
                Player.Instance.target.transform.position.z));
            Player.Instance.GetComponent<CharacterController>().enabled = true;
        }
        Debug.Log($"{Player.Instance.target}");
        yield return new WaitForSeconds(0.1f);
        if (_animator != null)
            _animator.SetTrigger("Flight");

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        _rigidbody.useGravity = true;
        _rigidbody.freezeRotation = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.isKinematic = false;
        if (Player.Instance.target == null)
        {
            Vector3 val = IpariUtility.CaculateVelocity(Player.Instance.transform.position + Player.Instance.transform.forward * _range, this.transform.position, _height);
            _rigidbody.velocity = val;
        }
        else if (Player.Instance.target != null)
        {
            Vector3 val = IpariUtility.CaculateVelocity(Player.Instance.target.transform.position + Player.Instance.transform.forward * _range, Player.Instance.transform.position, _height);
            _rigidbody.velocity = val;
        }
        Debug.Log($"{_rigidbody.velocity}");
        Forward = transform.position;

        Forward = _playerInteractionPoint.transform.right;
        PhysicsCheck = true;
        if (_animator == null)
            flight = true;

        yield return new WaitForSeconds(0.3f);
    }

    #endregion
    public void ResetPoint()
    {
        // 위치 초기화
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;
    }
    public void SetAutoTarget(Transform _transform = null)
    {
        autoTarget = _transform;
    }


    private void OnCollisionEnter(Collision collision)
    {
        bool bTagHit = !collision.gameObject.CompareTag("PassCollision") &&
            !collision.gameObject.CompareTag("Player");
        if (bTagHit)
        {
            flight = false;
            PhysicsCheck = false;
            _rigidbody.useGravity = true;
            if (_animator != null)
            {
                _animator.SetTrigger("Grounded");
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            }
            else
               _rigidbody.freezeRotation = false;
            _rigidbody.velocity += Physics.gravity * .05f;
        }
        if(bounceDir == default)
        {
            bounceDir = IpariUtility.RandomDirection();
            _rigidbody.velocity += bounceDir;
        }
    }

}
