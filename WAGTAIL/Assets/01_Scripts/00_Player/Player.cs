using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
    public bool isClimbing = false;
    public bool isPush = false;
    public bool isCarry = false;
    
    //============================================//
    // State
    public StateMachine movementSM;
    public IdleState idle;
    public JumpingState jump;
    public PushState push;
    public ClimbingState climbing;
    public CarryState carry;
    public PickUpState pickup;
    public DropState drop;

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

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        movementSM = new StateMachine();
        idle = new IdleState(this, movementSM);
        jump = new JumpingState(this, movementSM);
        climbing = new ClimbingState(this, movementSM);
        push = new PushState(this, movementSM);
        carry = new CarryState(this, movementSM);
        pickup = new PickUpState(this, movementSM);
        drop = new DropState(this, movementSM);


        movementSM.Initialize(idle);

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
    }

    private void FixedUpdate()
    {
        movementSM.currentState.PhysicsUpdate();
    }
}
