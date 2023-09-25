using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IPariUtility;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    static Player instance;
    public static Player Instance { get { return instance; } } 

    [Header("Controls")]
    //==================================
    // 1. Player에 있는 수치값들은 특수한 이유가 없으면 상수로 선언해야함.
    //==================================
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

    private const float HorizontalViewAngle = 120f;
    [Range(-180f, 180f)]
    [SerializeField] private float m_viewRotateZ = 0f;
    [HideInInspector] public LayerMask holdTargetMask;
    [HideInInspector] public LayerMask throwTargetMask;
    private float m_horizontalViewHalfAngle = 0f;
    public float throwRange = 6f;
    [Header("AutoTarget")]
    public GameObject target;
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
    
    private int _numFound;
    [Header("Interaction")]
    public GameObject Head;
    public Transform InteractionPoint;
    public Transform EquipPoint;
    public Transform ThrowEquipPoint;
    
    [Header("State Check")]
    public bool isIdle = true;
    public bool isJump = false;
    public bool isPickup = false;
    public bool isCarry = false;
    public bool isThrow = false;
    public bool isFlight = false;
    public bool isDead = false;
    public bool isPull = false;
    //public bool isSlide = true;
    
    //============================================//
    // State
    public StateMachine movementSM;
    public IdleState idle;
    public JumpingState jump;
    public FlightState flight;
    public LandingState landing;
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

    #region DrawGizmos
    /*
    private void OnDrawGizmos()
    {
        if (!isCarry)
        {
            m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

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
            m_horizontalViewHalfAngle = HorizontalViewAngle * 0.5f;

            Vector3 originPos = transform.position;

            Gizmos.DrawWireSphere(originPos, throwRange);

            Vector3 horizontalRightDir = IpariUtility.AngleToDirY(transform, -m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 horizontalLeftDir = IpariUtility.AngleToDirY(transform, m_horizontalViewHalfAngle + m_viewRotateZ);
            Vector3 lookDir = IpariUtility.AngleToDirY(transform, m_viewRotateZ);

            Debug.DrawRay(originPos, horizontalLeftDir * throwRange, Color.cyan);
            Debug.DrawRay(originPos, lookDir * throwRange, Color.green);
            Debug.DrawRay(originPos, horizontalRightDir * throwRange, Color.cyan);
        }
    }*/
    #endregion

    private void Awake()
    {
        if(instance == null)
            instance = this;
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

        holdTargetMask = LayerMask.GetMask("Interactable");
        throwTargetMask = LayerMask.GetMask("Enemies");
    }

    // Update is called once per frame
    private void Update()
    {
        movementSM.currentState.HandleInput();

        movementSM.currentState.LogicUpdate();

        //Debug.Log(isCarry);
        // 이거 그냥 Carry State or Throw State 에 넣으면 되는거 아닌가?
        if(isPickup)
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
        #region LegacyCode
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
        #endregion
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

    private GameObject FindViewTarget(Transform transform, float SearchRange, LayerMask targetMask)
    {
        Vector3 targetPos, dir, lookDir;
        Vector3 originPos = transform.position;
        Collider[] hitedTargets = Physics.OverlapSphere(originPos, SearchRange, targetMask);

        float dot, angle;

        foreach(var hitedTarget in hitedTargets)
        {
            targetPos = hitedTarget.transform.position;
            dir = (targetPos - originPos).normalized;
            lookDir = IpariUtility.AngleToDirY(this.transform.eulerAngles, rotateAngle);

            dot = Vector3.Dot(lookDir, dir);
            angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle <= HorizontalViewAngle)
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
        target = FindViewTarget(transform, throwRange, throwTargetMask );
    }
}
