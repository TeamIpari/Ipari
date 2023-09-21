using System;
using System.Collections;
using UnityEngine;
using IPariUtility;

public class Throw : MonoBehaviour, IInteractable
{
    [Header("Interactable Setting")]
    // 추가한 스크립트
    [SerializeField] private bool _isSmall;
    [SerializeField] private string _promt;
    private GameObject _playerEquipPoint;
    private Vector3 _playerEquipPos;
    private GameObject _playerInteractionPoint;
    private GameObject center;
    private Rigidbody rigidbody;

    [Header("Throw Setting")]
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

    private void Start()
    {
        _playerEquipPoint = Player.Instance.ThrowEquipPoint.gameObject;
        _playerEquipPos = _playerEquipPoint.transform.localPosition;
        _playerInteractionPoint = Player.Instance.InteractionPoint.gameObject;
         startPos = this.transform.position;
        _animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        BoxCollider box = GetComponent<BoxCollider>();
        rigidbody.useGravity = true;
        spawnPoint = this.transform.position;

        center = new GameObject();
        center.transform.position = this.transform.position + (box == null ? Vector3.zero : box.center);
        center.transform.parent = this.transform;
        center.name = "Center";
        //if (_animator != null)
        //    rayRange = 1f;
    }


    public bool AnimEvent()
    {
        return false;
    }

    public bool Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<Player>();
        if(_playerEquipPoint.transform.childCount == 0 &&
            player.movementSM.currentState == player.idle)
        {
            StartCoroutine(Pickup());
            // isCarry를 isThrow로 바꿔줘야함
            player.isPickup = true;
            
            return true;
        }

        else if (player.currentInteractable != null)
        {
            StartCoroutine(Throwing(interactor));
            player.isCarry = false;
            return true;
        }

        return false;
    }
    
    private void CheckRay()
    {
        RaycastHit hit;
        bool bRangeHit = Physics.Raycast(transform.position, -transform.up, out hit, 0.2f);
        bool bTagHit = !hit.transform.gameObject.CompareTag("Player")
            && !hit.transform.gameObject.CompareTag("PassCollision");
        bool bLayerHit = hit.transform.gameObject.layer != 5;

        if (bRangeHit && bTagHit && bLayerHit)
        {
            rigidbody.velocity = Vector3.zero;
            if (_animator != null)
                _animator.SetTrigger("Grounded");
        }
    }

    private void CheckVelocity()
    {
        if (rigidbody.velocity == Vector3.zero && rigidbody.isKinematic == false )
        {
            //rigidbody.isKinematic = true;
            ;
        }
    }

    private void PhyscisChecking()
    {
        if (PhysicsCheck)
        {
            rigidbody.velocity += Physics.gravity * .05f;
        }
        if (flight)
        {
            this.transform.RotateAround(center.transform.position, Forward, (speed * Time.deltaTime));
        }
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.K))
            ResetPoint();
        PhyscisChecking();
        CheckVelocity();
    }

    IEnumerator Pickup()
    {
        // 추가된 스크립트 2023-08-22 강명호
        //if (_isSmall)
        //{
        //    _playerEquipPoint.transform.localPosition = new Vector3(_playerEquipPoint.transform.localPosition.x, _playerEquipPoint.transform.localPosition.y - 0.338f, _playerEquipPoint.transform.localPosition.z + 0.865f);
        //}
        // ================================

        transform.SetParent(_playerEquipPoint.transform);
        transform.position = new Vector3(_playerInteractionPoint.transform.position.x, Player.Instance.transform.position.y, _playerInteractionPoint.transform.position.z) ;
        transform.rotation = Quaternion.identity;
        startPos = transform.position;
        _value = 0;

        // 방향벡터를 구함.
        Vector3 lookvec = Player.Instance.transform.position - transform.position;
        lookvec = lookvec.normalized;
        
        // 꺾이는 지점.
        height = new Vector3(startPos.x, _playerEquipPoint.transform.position.y -.5f, startPos.z) + lookvec * 0.5f;
        // 머리 위
        endPos = new Vector3(_playerEquipPoint.transform.position.x, _playerEquipPoint.transform.position.y - 0.5f, _playerEquipPoint.transform.position.z);
        // Object가 Player의 머리 위에서 움직이는걸 방지
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.freezeRotation = true;
        rigidbody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        // 추가된 스크립트 2023-08-22 강명호
        if (_isSmall)
            yield return new WaitForSeconds(0.4f);
        else
            yield return new WaitForSeconds(0.75f);
        // ==============================

        if (_animator != null)
            _animator.SetTrigger("Caught");

        while (_value <= 1)
        {
            yield return new WaitForSeconds(0.0025f);
            transform.transform.position = (BeziurCurve(_value));
            _value += 0.05f;
        }
        // Object를 Player의 머리 위로 옮김.
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    Vector3 BeziurCurve( float _value)
    {
        Vector3 A = Vector3.Lerp(startPos, height, _value);

        Vector3 B = Vector3.Lerp(height, endPos, _value);

        Vector3 C = Vector3.Lerp(A, B, _value);

        return C;
    }

    IEnumerator Throwing(GameObject interactor)
    {
        Player player = interactor.GetComponent<Player>();
        if(player.target != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.LookAt(new Vector3(player.target.transform.position.x, Player.Instance.transform.position.y, player.target.transform.position.z));
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
        rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rigidbody.isKinematic = false;
        GetComponent<Collider>().isTrigger = false;

        //rigidbody.velocity = CaculateVelocity(interactor.player.transform.position + interactor.player.transform.forward * _range, this.transform.position, _hight);
        if (player.target == null)
            rigidbody.velocity = IpariUtility.CaculateVelocity(player.transform.position + player.transform.forward * _range, this.transform.position, _hight);
        else if (player.target != null)
        {
            Vector3 vel = IpariUtility.CaculateVelocity(player.target.transform.position + player.transform.forward * _range, player.transform.position, _hight);
            Debug.Log(vel);
            rigidbody.velocity = vel;
        } 
        Forward = this.transform.position;

        Forward = _playerInteractionPoint.transform.right;
        PhysicsCheck = true;
        if(_animator == null)
            flight = true;
    }

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

    private void OnCollisionEnter(Collision collision)
    {
        bool bTagHit = !collision.gameObject.CompareTag("PassCollision") &&
            !collision.gameObject.CompareTag("Player");
        if (bTagHit)
        {
            flight = false;
            PhysicsCheck = false;
            rigidbody.useGravity = true;
            if (_animator != null)
            {
                _animator.SetTrigger("Grounded");
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            }
            else
               rigidbody.freezeRotation = false;
            rigidbody.velocity += Physics.gravity * .05f;
        }
        if(bounceDir == default)
        {
            bounceDir = RandomDirection().normalized;
            rigidbody.velocity += bounceDir;
        }
    }

}
