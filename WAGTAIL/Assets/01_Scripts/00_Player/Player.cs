using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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
    [HideInInspector]
    public GameObject currentInteractable;
    
    [Header("Points")]
    public Transform InteractionPoint;
    public Transform EquipPoint;
    public Transform ThrowEquipPoint;
    public Transform LeftHand;
    public Transform RightHandPoint;
    public Transform RightHand;

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
    [HideInInspector] public SoundHandler SoundHandler;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

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
    }

    private void Start()
    {
        // Manager
        GameManager = GameManager.GetInstance();
        UIManager = UIManager.GetInstance();
        CameraManager = CameraManager.GetInstance();
        SoundHandler = GetComponent<SoundHandler>();
        
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
        
        // SoundBind
        SoundHandler.RegisterBool("isWalk");
        SoundHandler.RegisterTrigger("isJump");
        SoundHandler.RegisterTrigger("isLanding");
        SoundHandler.RegisterTrigger("isDeath");
        SoundHandler.Bind("isWalk", walkClip);
        SoundHandler.Bind("isJump", jumpClip);
        SoundHandler.Bind("isLanding", landingClip);
        SoundHandler.Bind("isDeath", deathClip);
    }

    // Update is called once per frame
    private void Update()
    {
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
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
}
