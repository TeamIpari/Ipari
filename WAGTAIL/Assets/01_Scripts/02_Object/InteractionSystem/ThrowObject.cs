using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;

//=================================================
// �÷��̾ ��� ���� �� �ִ� ������Ʈ�� ����� �Ǵ� Ŭ����.
//=================================================
[AddComponentMenu("InteractionSystem/ThrowObject")]
public class ThrowObject : MonoBehaviour, IInteractable
{
    //==========================================================
    //                  Properties And Fields                 //
    //==========================================================
    [Header("Throw Setting")]
    [Range(0, 5)]
    [SerializeField] float _value = 0.0f;
    [SerializeField] float _hight = 0.0f;
    [SerializeField] float _range = 0.0f;
    [SerializeField] private Transform autoTarget;
    [Range(0,0.25f)]
    [SerializeField] float _overheadSpeed = 0.0f;
    
    private Transform _transform;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private GameObject _center;
    private Vector3 _bounceDir;
    
    // Properties for Bezier Curve
    private Vector3 _startPos;
    private Vector3 _height;
    private Vector3 _endPos;
    
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
    [SerializeField] private bool isSmall;
    [SerializeField] private Vector3 Forward;
    [SerializeField] private bool PhysicsCheck = false;
    [SerializeField] private bool flight = false;
    private Animator _animator;
    // ���� Properties�� �׽�Ʈ������ ���߿� ���� ����
    
    
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
        
        
        _center = new GameObject();
        _center.transform.position = _transform.position + (_collider == null ? Vector3.zero : _collider.bounds.center);
        _center.transform.parent = _transform;
        _center.name = "Center";
    }

    private void FixedUpdate()
    {
        PhysicsChecking();
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
        if(_bounceDir == default)
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
      
        if (_player.currentInteractable == null)
        {
            StartCoroutine(PickUp(LerpTime, PickUpTime));
            _player.isCarry = true;
        }
        
        else
        {
            StartCoroutine(Throwing(interactor));
            _player.isCarry = false;
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
        // Object�� �տ� �پ �������� �ʵ��� ����
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.freezeRotation = true;
        _rigidbody.isKinematic = true;
        _collider.isTrigger = true;
        
        var pos = _transform.position;
        var currentTime = 0.0f;
        
        // �տ��� ���� �������ִ� Object�� ������ ������� ȿ���� �ֱ� ���� Lerp
        while (currentTime < lerpTime)
        {
            _transform.position = Vector3.Lerp(pos, _playerInteractionPos.position, currentTime / lerpTime);
            currentTime += 0.1f;
            yield return new WaitForSeconds(0.0025f);
        }
        yield return new WaitForSeconds(PickUpDelayTime);
        
        // BezierCurve�� ���� 3���� �� ���ϱ� StartPos, Height, EndPos
        _startPos = _transform.position;
        var lookVec = (_player.transform.position - _transform.position).normalized;
        _height = new Vector3(_startPos.x, _playerEquipPos.position.y - 0.5f, _startPos.z) + lookVec * 0.5f;
        _endPos = _playerEquipPos.position;
        _endPos.y -= 0.25f;
        
        // �Ӹ� ���� ��� ��� �׸��� �ڷ�ƾ
        currentTime = 0.0f;
        while (currentTime < pickUpTime)
        {
            _transform.position = BezierCurve(_startPos, _endPos, _height, currentTime / pickUpTime);
            currentTime += 0.1f;
            yield return new WaitForSeconds(0.0025f);
        }
        _transform.SetParent(_playerHead.transform);
    }

    private IEnumerator Throwing(GameObject interactor)
    {
        // Object ������ Ǯ����
        _transform.SetParent(null);
        
        if(autoTarget != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.LookAt(new Vector3(autoTarget.position.x, Player.Instance.transform.position.y, autoTarget.position.z));
            Player.Instance.GetComponent<CharacterController>().enabled = true;
        }
        yield return new WaitForSeconds(0.2f);
        if (_animator != null)
            _animator.SetTrigger("Flight");

        // �Ӹ� ������ �����̴°� �����ϱ� ���� �͵� ����
        _rigidbody.useGravity = true;
        _rigidbody.freezeRotation = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.isKinematic = false;
        _collider.isTrigger = false;

        if (_player.target == null)
            _rigidbody.velocity = IpariUtility.CaculateVelocity(_player.transform.position + _player.transform.forward * _range, this.transform.position, _hight);
        else if (_player.target != null)
        {
            Vector3 vel = IpariUtility.CaculateVelocity(_player.target.transform.position + _player.transform.forward * _range, _player.transform.position, _hight);
            Debug.Log(vel);
            _rigidbody.velocity = vel;
        } 
        Forward = transform.position;

        Forward = _playerInteractionPoint.transform.right;
        PhysicsCheck = true;
        if(_animator == null)
            flight = true;
    }
    
    private void PhysicsChecking()
    {
        if (PhysicsCheck)
        {
            _rigidbody.velocity += Physics.gravity * .05f;
        }
        
        if (flight)
        {
            transform.RotateAround(_center.transform.position, Forward, (1.0f * Time.deltaTime));
        }
    }
    
    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);
 
        return c;
    }
    
    private Vector3 RandomDirection()
    {
        // 8�������� �̵��� �����ϰ� �� ����.
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
