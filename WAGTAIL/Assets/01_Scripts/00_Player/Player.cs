using System.Collections;
using System.Collections.Generic;
//using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

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
    // 추가 스크립트
    //public bool isSmallThrow = false;
    //
    // 당기는데, 원점으로부터 멀어지면 멀어질 수록 최대 도달점과 비교하여
    // 퍼센테이지로 이동속도를 줄임.
    public bool isPull = false;
    
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

    public Transform InteractionPoint;
    public Transform EquipPoint;
    public Transform ThrowEquipPoint;
    public Transform LeftHand;
    public Transform RightHandPoint;
    public Transform RightHand;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
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

        // 시작할때 Init 해줄 State 지정
        movementSM.Initialize(idle);

        // 초기 Collider 저장
        normalColliderHeight = controller.height;
        normalColliderCenter = controller.center;
        normalColliderRadius = controller.radius;
        gravityValue *= gravityMultiplier;
    }

    // Update is called once per frame
    private void Update()
    {
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();
        Debug.Log(controller.isGrounded);
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        try
        {
            if (hit.gameObject.tag == "Platform" &&
                !hit.gameObject.GetComponent<IEnviroment>()._hit)
            {
                hit.gameObject.GetComponent<IEnviroment>().Interact();
            }
        }
        catch
        {

        }
    }
}
