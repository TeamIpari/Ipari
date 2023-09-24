using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;

//=================================================
// 플레이어가 들고 던질 수 있는 오브젝트의 기반이 되는 클래스.
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
    // 위의 Properties는 테스트용으로 나중에 삭제 예정
    
    
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
        // Object가 손에 붙어서 움직이지 않도록 설정
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.freezeRotation = true;
        _rigidbody.isKinematic = true;
        _collider.isTrigger = true;
        
        var pos = _transform.position;
        var currentTime = 0.0f;
        
        // 손에서 조금 떨어져있는 Object를 손으로 끌어당기는 효과를 주기 위한 Lerp
        while (currentTime < lerpTime)
        {
            _transform.position = Vector3.Lerp(pos, _playerInteractionPos.position, currentTime / lerpTime);
            currentTime += 0.1f;
            yield return new WaitForSeconds(0.0025f);
        }
        yield return new WaitForSeconds(PickUpDelayTime);
        
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
            currentTime += 0.1f;
            yield return new WaitForSeconds(0.0025f);
        }
        _transform.SetParent(_playerHead.transform);
    }

    private IEnumerator Throwing(GameObject interactor)
    {
        // Object 종속을 풀어줌
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

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        //_rigidbody.useGravity = true;
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
    
    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);
 
        return c;
    }
    
    private Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        // define the distance x and y first;
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
        distanceXZ.y = 0f; // y는 0으로 설정.
        Forward = origin;
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
    
    private void PhyscisChecking()
    {
        if (PhysicsCheck)
        {
            _rigidbody.velocity += Physics.gravity * .05f;
        }
    }
    
    public void SetAutoTarget(Transform _transform = null)
    {
        autoTarget = _transform;
    }
}
