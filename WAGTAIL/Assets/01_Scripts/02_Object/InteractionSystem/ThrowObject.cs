using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;
using UnityEngine.Serialization;
using UnityEngine.Events;

//=================================================
// 플레이어가 들고 던질 수 있는 오브젝트의 기반이 되는 클래스.
//=================================================
[AddComponentMenu("InteractionSystem/ThrowObject")]
public class ThrowObject : MonoBehaviour, IInteractable
{
    #region Define
    [System.Serializable]
    public sealed class ThrowObjectEvent : UnityEvent
    { 
    }
    #endregion

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

    public string InteractionPrompt    { get; set; } = "들기";
    public Vector3 InteractPopupOffset { get; set; } = (Vector3.up*1.5f);

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
    private float flightTime = 0;

    //=================================================================
    //                    Test Properties                    
    //=================================================================
    [Header("Test Properties")]
    [SerializeField] public bool isReady;
    [SerializeField] private bool isTarget;
    [SerializeField] private bool isSmall;
    [SerializeField] private Vector3 Forward;
    [SerializeField] private bool PhysicsCheck = false;
    [SerializeField] private bool flight = false;
    [SerializeField] private float Gravity = 7;
    private Animator _animator;
    [SerializeField] private float Rot = 12;
    [SerializeField] private float correctionHeight = 0f;
    [SerializeField] private const float correctionForward = 1.5f;
    [SerializeField] private float correctionGravity = 0f;
    // 위의 Properties는 테스트용으로 나중에 삭제 예정( 안할수도?)

    // Property
    public bool GetPhyscisCheck
    {
        get
        {
            return PhysicsCheck;
        }
    }

    [SerializeField] public ThrowObjectEvent OnPickUp;
    [SerializeField] public ThrowObjectEvent OnThrow;


    //=================================================================
    //                      Magic Methods          
    //=================================================================
    private void Start()
    {
        isReady = true;
        flight = true;
        // Caching
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // Player Caching
        _player = Player.Instance;
        _playerInteractionPoint = _player.InteractionPoint.gameObject;
        _playerInteractionPos = _playerInteractionPoint.transform;
        _playerEquipPoint = _player.ThrowEquipPoint.gameObject;
        _playerEquipPos = _playerEquipPoint.transform;
        _playerHead = _player.Head;
        _playerHeadPos = _playerHead.transform;
        spawnPoint = this.transform.position;

        _center = new GameObject();
        _center.transform.parent = this.transform;
        _center.transform.position = (gameObject.GetComponent<Collider>() == null ? Vector3.zero : gameObject.GetComponent<Collider>().bounds.center);
        _center.name = "Center";
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
            ResetPoint();
        if (!isTarget)
            PhysicsChecking();
        else if(isTarget )
            _rigidbody.velocity += -Vector3.up * (Gravity * 0.1f);

        FlightRotate();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"{collision.gameObject.layer}");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            Debug.Log("AA");
            ResetPoint();
            return;
        }
        bool bTagHit = !collision.gameObject.CompareTag("PassCollision") &&
                       !collision.gameObject.CompareTag("Player");
        if (bTagHit)
        {
            //Debug.Log($"hit name = {collision.gameObject.name}");
            flight = false;
            PhysicsCheck = false;
            isTarget = false;
            _rigidbody.useGravity = true;
            if (_animator != null)
            {
                _animator.SetTrigger("Grounded");
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            }
            else
                _rigidbody.freezeRotation = false;
            //_rigidbody.velocity += Physics.gravity * Gravity;
            //_rigidbody.velocity += -Vector3.up * (Gravity / 10f);
            flightTime = 1;
        }
        if (_bounceDir == default)
        {
            _bounceDir = RandomDirection().normalized;
            _rigidbody.velocity += _bounceDir;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Deathzone>() != null)
        {
            if (this.GetComponent<BombObject>() != null)
                this.GetComponent<BombObject>().Explosion();
            else
                ResetPoint();
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
            if (!isReady)
                return false;
            _rigidbody.useGravity = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.isKinematic = true;
            PhysicsCheck = false;
            flight = false;
            _collider.isTrigger = true;
            if(_animator != null)
                _animator.SetTrigger("Caught");
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _player.isPickup = true;
            isTarget = false;
            isReady = false;
            return true;
        }

        else
        {
            _transform.SetParent(null);
            isReady = false;
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
        #region Omit
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
        OnPickUp?.Invoke();
        #endregion
    }

    private IEnumerator Throwing(GameObject interactor)
    {
        // Object 종속을 풀어줌
        if (_player.target != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.LookAt(
                new Vector3(_player.target.transform.position.x,
                Player.Instance.transform.position.y,
                _player.target.transform.position.z));
            Player.Instance.GetComponent<CharacterController>().enabled = true;
        }
        //Debug.Log($"{_player.target}");
        yield return new WaitForSecondsRealtime(0.1f);
        if (_animator != null)
            _animator.SetTrigger("Flight");
        
        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        _rigidbody.useGravity = true;
        _rigidbody.freezeRotation = false;
        //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
            flightTime = height * (distance / (_player.throwRange));
            isTarget = true;
            Vector3 correction_value =
                new Vector3(_player.target.transform.position.x, _player.target.transform.position.y + correctionHeight, _player.target.transform.position.z)
                + (_player.transform.forward * correctionForward)* (Gravity * flightTime);

            Vector3 val = IpariUtility.CaculateVelocity(correction_value , _player.transform.position, flightTime);
            _rigidbody.velocity = val;
        }
        
        //_rigidbody.velocity += _player.movementSM.currentState.velocity * _player.playerSpeed * 0.3f;
        Forward = transform.position;

        Forward = _playerInteractionPoint.transform.right;
        PhysicsCheck = true;
        _player.isCarry = false;
        if (_animator == null)
            flight = true;

        OnThrow?.Invoke();
        yield return new WaitForSecondsRealtime(0.3f);
        _collider.isTrigger = false;
        isReady = true;
    }

    private void PhysicsChecking()
    {
        if (PhysicsCheck)
        {
            _rigidbody.velocity += Physics.gravity * .05f;
        }

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
        if (_player.currentInteractable != null)
        {
            _transform.SetParent(null);
            _rigidbody.useGravity = true;
            _rigidbody.freezeRotation = false;
            _rigidbody.isKinematic = false;
            _collider.isTrigger = false;
            _player.currentInteractable = null;
            isReady = true;
        }
        transform.position = spawnPoint + Vector3.up * 5f;
        transform.rotation = Quaternion.identity;
        _rigidbody.velocity = Vector3.zero;
        PhysicsCheck = false;
        flight = false;
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

    private void FlightRotate()
    {
        if (flight && _animator == null)
        {
            transform.RotateAround(_center.transform.position, Forward, (Rot / flightTime));
        }
        else if (!flight && _animator != null)
        {
            ;
        }
        else
        {
            _rigidbody.velocity += -Vector3.up * (correctionGravity * 1.4f);
        }
    }
}
