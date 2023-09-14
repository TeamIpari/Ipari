using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using IPariUtility;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

[RequireComponent(typeof(SoundHandler))]
public class Player : MonoBehaviour
{
    static Player instance;
    public static Player Instance { get { return instance; } }  

    [Header("Controls")]
    public float playerSpeed = 15.0f;
    // 점프 사용할지 말지 몰라서 일단 기입만 해둠
    // =======================================
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2;
    // =======================================
    public float rotationSpeed = 5f;
    public float climbingSpeed = 0;
    public float pullingSpeed = 5.0f;
    // =======================================
    public float slopeSpeed = 0f;
    public float respawnTime;
    //========================================
    //              지훈 추가               //
    //========================================
    [Header("HoldSearch")]
    public float rotateAngle;
    [Range(0, 360f)]
    [SerializeField] private float m_horizontalViewAngle = 0f;
    [Range(-180f, 180f)]
    [SerializeField] private float m_viewRotateZ = 0f;
    public LayerMask holdTargetMask;
    public LayerMask ThrowTargetMask;
    private float m_horizontalViewHalfAngle = 0f;
    public float throwRange = 6f;
    [Header("AutoTarget")]
    public GameObject Target;
    //========================================


    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 1)]
    public float velocityDampTime = 0.1f;
    [Range(0, 1)]
    public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    [Header("Interaction")]
    private readonly Collider[] _colliders = new Collider[3];
    private int _numFound;
    private readonly float _interactionPointRadius = 0.5f;
    public LayerMask interactableMask;
    public Transform InteractionPoint;
    public Transform EquipPoint;
    public Transform ThrowEquipPoint;
    public Transform LeftHand;
    public Transform RightHandPoint;
    public Transform RightHand;
    
    [Header("State Check")]
    public bool isIdle = true;
    public bool isClimbing = false;
    public bool isPush = false;
    public bool isCarry = false;
    public bool isFlight = false;
    public bool isDead = false;
    public bool isSmallThrow = false;
    // 당기는데, 원점으로부터 멀어지면 멀어질 수록 최대 도달점과 비교하여
    // 퍼센테이지로 이동속도를 줄임.
    public bool isPull = false;
    //public bool isSlide = true;
    
    [Header("Sound")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip deathClip;
    
    //============================================//
    // State
    public StateMachine movementSM;
    public IdleState idle;
    public JumpingState jump;
    public FlightState flight;
    public LandingState landing;
    public PushState push;
    public ClimbingState climbing;
    public CarryState carry;
    public PickUpState pickup;
    public DropState drop;
    public PullingState pull;
    public PullOutState pullOut;
    public DeathState death;

    //============================================//
    // Move
    [HideInInspector]
    public float gravityValue = -9.81f;
    [HideInInspector]
    public float normalColliderHeight;
    [HideInInspector]
    public Vector3 normalColliderCenter;
    [HideInInspector]
    public float normalColliderRadius;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Transform cameraTransform;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Vector3 playerVelocity;
    //[HideInInspector]
    public GameObject currentInteractable;

    // ============================================//
    // FX
    // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
    [Header("FX")]
    public GameObject MoveFX;

    //============================================//
    // Manager
    [HideInInspector] public UIManager UIManager;
    [HideInInspector] public GameManager GameManager;
    [HideInInspector] public CameraManager CameraManager;
    
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(InteractionPoint.position, _interactionPointRadius);

        //======================================
        //지훈추가
        //======================================
        if (!isCarry)
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, 3);

            Vector3 horizontalRightDir = IpariUtility.AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = IpariUtility.AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = IpariUtility.AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * 3, Color.cyan);
            Debug.DrawRay(originPos, lookDir * 3, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * 3, Color.cyan);
        }
        else
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, throwRange);

            Vector3 horizontalRightDir = IpariUtility.AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = IpariUtility.AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = IpariUtility.AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * throwRange, Color.cyan);
            Debug.DrawRay(originPos, lookDir * throwRange, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * throwRange, Color.cyan);
        }
    }

    private void Awake()
    {
        // FindObjectOfType<Player>() 사용 비권장

        // 유니티 최신 버전에서 추가된 API
        // FindAnyObjectByType<Player>(); 
        // FindFirstObjectByType<Player>();
        

        // player singleton 고민.
        if(instance == null)
            instance = this;
        //transform.parent = null;
    }

    private void Start()
    {
        // Manager
        GameManager = GameManager.GetInstance();
        UIManager = UIManager.GetInstance();
        CameraManager = CameraManager.GetInstance();
        
        // GetComponents
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        // State
        movementSM = new StateMachine();
        idle = new IdleState(this, movementSM);
        jump = new JumpingState(this, movementSM);
        flight = new FlightState(this, movementSM);
        landing = new LandingState(this, movementSM);
        climbing = new ClimbingState(this, movementSM);
        push = new PushState(this, movementSM);
        carry = new CarryState(this, movementSM);
        pickup = new PickUpState(this, movementSM);
        drop = new DropState(this, movementSM);
        pull = new PullingState(this, movementSM);
        pullOut = new PullOutState(this, movementSM);
        death = new DeathState(this, movementSM);

        // 시작할때 Init 해줄 State 지정
        movementSM.Initialize(idle);

        // 초기 Collider 저장
        normalColliderHeight = controller.height;
        normalColliderCenter = controller.center;
        normalColliderRadius = controller.radius;
        gravityValue *= gravityMultiplier;
        
        interactableMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    private void Update()
    {
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();
        if(isCarry)
        {
            EnemySearching();
        }
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
    }
    
    // (구현해야함) 가장 가까운 collider를 읽어내서 IInteractable을 상속받은 클래스가 있다면 상호작용을 한다.
    public void Interaction()
    {
        //if (currentInteractable == null)
        //{
        //    _numFound = Physics.OverlapSphereNonAlloc(InteractionPoint.position, _interactionPointRadius, _colliders,
        //        interactableMask);

        //    if (_numFound <= 0) return;
        //    var interactable = _colliders[0].GetComponent<IInteractable>();

        //    if (interactable == null) return;
        //    interactable.Interact(gameObject);
        //    currentInteractable = _colliders[0].gameObject;
        //}
        if(currentInteractable == null)
        {
            GameObject obj = FindViewTarget(transform, 3,holdTargetMask);
            if (obj == null)  return;
            var interactable = obj.GetComponent<IInteractable>();
            if (interactable == null)  return;
            interactable.Interact(gameObject);
            currentInteractable = obj;

        }

        else
        {
            currentInteractable.GetComponent<IInteractable>().Interact(gameObject);
            currentInteractable = null;
        }
    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        try
        {
            if (hit.gameObject.CompareTag( "Platform" )&&
                !hit.gameObject.GetComponent<IEnviroment>().IsHit)
            {
                hit.gameObject.GetComponent<IEnviroment>().Interact();
            }
            else
            {
                transform.SetParent(null);
            }
        }
        catch
        {

        }
    }

    //==================================================
    //                      지훈 추가                   //
    //          용서가 빠르다는 이야기를 들었습니다.       // 
    //==================================================
    private GameObject FindViewTarget(Transform transform, float SearchRange, LayerMask targetMask)
    {
        Vector3 targetPos, dir, lookdir;
        Vector3 originPos = transform.position;
        Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, targetMask);

        float dot, angle;

        foreach(var hitedTarget in hitedTargets)
        {
            targetPos = hitedTarget.transform.position;
            dir = (targetPos - originPos).normalized;
            lookdir = IpariUtility.AngleToDirY(this.transform.eulerAngles, rotateAngle);

            dot = Vector3.Dot(lookdir, dir);
            angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if(angle <= m_horizontalViewAngle)
            {
                // 타겟이 걸리면 반환.
                return hitedTarget.gameObject;
            }
        }
        // 걸리는게 없나요? 정상입니다.
        return null;
    }

    private void EnemySearching()
    {
        Target = FindViewTarget(transform, throwRange, ThrowTargetMask );
        
    }
}
