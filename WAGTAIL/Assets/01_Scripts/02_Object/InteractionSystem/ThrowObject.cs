using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;
using UnityEngine.Serialization;
using FMODUnity;

//=================================================
// 플레이어가 들고 던질 수 있는 오브젝트의 기반이 되는 클래스.
//=================================================
[AddComponentMenu("InteractionSystem/ThrowObject")]
public class ThrowObject : MonoBehaviour, IInteractable
{
    //==========================================================
    //                  Properties And Fields                 //
    //==========================================================
    [FormerlySerializedAs("_value")]
    [Header("Throw Setting")]
    [Range(0, 5)]
    [SerializeField] private float value = 0.0f;
    [SerializeField] private float height = 0.0f;
    [SerializeField] private float range = 0.0f;
    [SerializeField] private Transform autoTarget;

    private Transform _transform;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private GameObject _center;
    private Vector3 _bounceDir;

    // Properties for Bezier Curve
    private Vector3 _startPos;
    private Vector3 _height;
    private Vector3 _endPos;
    Vector3 spawnPoint;

    public string InteractionPrompt { get; } = string.Empty;

    // Player Properties
    private Player _player;
    private GameObject _playerInteractionPoint;
    private Transform _playerInteractionPos;
    private GameObject _playerEquipPoint;
    private Transform _playerEquipPos;
    private GameObject _playerHead;
    private Transform _playerHeadPos;

    // DelayTime Properties
    private const float LerpTime = 2f;
    private const float PickUpDelayTime = 0.3f;
    private const float PickUpTime = 2.5f;

    //=================================================================
    //                    Test Properties                    
    //=================================================================
    [Header("Test Properties")]
    [SerializeField] private bool isTarget;
    [SerializeField] private bool isSmall;
    [SerializeField] private Vector3 Forward;
    [SerializeField] private bool PhysicsCheck = false;
    [SerializeField] private bool flight = false;
    private Animator _animator;
    // 위의 Properties는 테스트용으로 나중에 삭제 예정

    // Property
    public bool GetPhyscisCheck
    {
        get
        {
            return PhysicsCheck;
        }
    }

    //=================================================================
    //                      Magic Methods          
    //=================================================================
    private void Start()
    {
        // Caching
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();

        // Player Caching
        _player = Player.Instance;
        _playerInteractionPoint = _player.InteractionPoint.gameObject;
        _playerInteractionPos = _playerInteractionPoint.transform;
        _playerEquipPoint = _player.ThrowEquipPoint.gameObject;
        _playerEquipPos = _playerEquipPoint.transform;
        _playerHead = _player.Head;
        _playerHeadPos = _playerHead.transform;
        spawnPoint = this.transform.position;


        //_center = new GameObject
        //{
        //    transform =
        //    {
        //        position = _transform.position + (_collider == null ? Vector3.zero : _collider.bounds.center),
        //        parent = _transform
        //    },
        //    name = "Center"
        //};
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetPoint();
        if (!isTarget)
            PhysicsChecking();
        else 
            _rigidbody.velocity += -Vector3.up * .05f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool bTagHit = !collision.gameObject.CompareTag("PassCollision") &&
                       !collision.gameObject.CompareTag("Player");
        if (bTagHit)
        {
            //Debug.Log($"hit name = {collision.gameObject.name}");
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
        if (_bounceDir == default)
        {
            _bounceDir = RandomDirection().normalized;
            _rigidbody.velocity += _bounceDir;
        }
    }

    //=================================================================
    //                      Interface Methods
    //=================================================================
    public bool Interact(GameObject interactor)
    {
        // PickUp logic
        if (_player.currentInteractable == null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.isKinematic = true;
            PhysicsCheck = false;
            flight = false;
            _collider.isTrigger = true;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _player.isPickup = true;
        }

        else
        {
            StartCoroutine(Throwing(interactor));
            //_player.isCarry = false;
            return true;
        }

        return false;
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }

    //=================================================================
    //                      Core Methods
    //=================================================================
    private IEnumerator PickUp(float lerpTime, float pickUpTime)
    {
        _player.isPickup = true;
        // Object가 손에 붙어서 움직이지 않도록 설정
        _rigidbody.useGravity = false;
        _rigidbody.freezeRotation = true;
        _rigidbody.isKinematic = true;
        PhysicsCheck = false;
        flight = false;
        _collider.isTrigger = true;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;

        var pos = _transform.position;
        var currentTime = 0.0f;

        // 손에서 조금 떨어져있는 Object를 손으로 끌어당기는 효과를 주기 위한 Lerp
        while (currentTime < lerpTime)
        {
            _transform.position = Vector3.Lerp(pos, _playerInteractionPos.position, currentTime / lerpTime);
            currentTime += 0.4f;
            yield return new WaitForSecondsRealtime(0.017f);
        }
        yield return new WaitForSecondsRealtime(PickUpDelayTime);

        // BezierCurve를 위한 3개의 점 구하기 StartPos, Height, EndPos
        _startPos = _transform.position;
        var lookVec = (_player.transform.position - _transform.position).normalized;
        _height = new Vector3(_startPos.x, _playerEquipPos.position.y - 0.5f, _startPos.z) + lookVec * 0.5f;
        _endPos = _playerEquipPos.position;
        _endPos.y -= 0.25f;

        // 머리 위로 드는 곡선을 그리는 코루틴
        currentTime = 0.0f;
        while (currentTime < pickUpTime)
        {
            _transform.position = BezierCurve(_startPos, _endPos, _height, currentTime / pickUpTime);
            currentTime += 0.4f;
            yield return new WaitForSecondsRealtime(0.017f);
        }
        _transform.SetParent(_playerHead.transform);
    }

    private IEnumerator Throwing(GameObject interactor)
    {
        // Object 종속을 풀어줌
        _transform.SetParent(null);
        if (_player.target != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.LookAt(
                new Vector3(_player.target.transform.position.x,
                Player.Instance.transform.position.y,
                _player.target.transform.position.z));
            Player.Instance.GetComponent<CharacterController>().enabled = true;
        }
        Debug.Log($"{_player.target}");
        yield return new WaitForSecondsRealtime(0.1f);
        if (_animator != null)
            _animator.SetTrigger("Flight");

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        _rigidbody.useGravity = true;
        _rigidbody.freezeRotation = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.isKinematic = false;

        if (_player.target == null)
        {
            isTarget = false;
            Vector3 val = IpariUtility.CaculateVelocity(_player.transform.position + _player.transform.forward * range, _player.transform.position, height);
            _rigidbody.velocity = val;

        }
        else if (_player.target != null)
        {
            float distance = Vector3.Distance(_player.target.transform.position, this.transform.position);
            float flightTime = 1f * (distance / _player.throwRange);
            isTarget = true;
            Vector3 val = IpariUtility.CaculateVelocity(_player.target.transform.position, this.transform.position, flightTime);
            _rigidbody.velocity = val;
        }
        
        //_rigidbody.velocity += _player.movementSM.currentState.velocity * _player.playerSpeed * 0.3f;
        Forward = transform.position;

        Forward = _playerInteractionPoint.transform.right;
        PhysicsCheck = true;
        if (_animator == null)
            flight = true;

        yield return new WaitForSecondsRealtime(0.3f);
        _player.isCarry = false;
        _collider.isTrigger = false;
    }

    private void PhysicsChecking()
    {
        if (PhysicsCheck)
        {
            _rigidbody.velocity += Physics.gravity * .05f;
        }

        //if (flight)
        //{
        //    transform.RotateAround(_center.transform.position, Forward, (1.0f * Time.deltaTime));
        //}
    }

    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);

        return c;
    }

    public void ResetPoint()
    {
        // 위치 초기화
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;
    }

    private Vector3 RandomDirection()
    {
        // 8방향으로 이동이 가능하게 할 예정.
        switch (UnityEngine.Random.Range(0, 8))
        {
            case 0:
                return new Vector3(0, 0, 1);
            case 1:
                return new Vector3(1, 0, 1);
            case 2:
                return new Vector3(0, 0, 1);
            case 3:
                return new Vector3(1, 0, -1);
            case 4:
                return new Vector3(0, 0, -1);
            case 5:
                return new Vector3(-1, 0, -1);
            case 6:
                return new Vector3(-1, 0, 0);
            case 7:
                return new Vector3(-1, 0, 1);
        }
        return Vector3.zero;
    }
}
