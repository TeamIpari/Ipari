using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


/********************************************************
 * 플레이어가 밟을 수 있는 플랫폼의 기반이 되는 클래스입니다.
 ****/
public sealed class PlatformObject : MonoBehaviour, IEnviroment
{
    private enum PendingKillProgress
    {
        NONE,
        PENDINGKILL_READY,
        PENDINGKILL
    }

    //========================================
    //////      Property And Fields      /////
    //========================================
    public string   EnviromentPrompt { get; }      = string.Empty;
    public bool     IsHit            { get; set; } = false;
    public bool     ObjectOnPlatform { get { return (_ObjectOnPlatformCount>0);  } }
    public Collider Collider         { get { return _Collider; } set{ if (value != null) _Collider = value;  }  }

    [SerializeField] public bool PlayerRideOn = false;
    [HideInInspector] public float CheckGroundOffset = 0f;
    [SerializeField] private List<PlatformBehaviorBase> Behaviors = new List<PlatformBehaviorBase>();


    /**플레이어 관련 클래스 캐싱*/
    private static CharacterController _Controller;
    private StateMachine  _PlayerSM;
    private Collider      _Collider;

    /**Interactions 관련*/
    private PendingKillProgress   _PkProgress = PendingKillProgress.NONE;
    private int                   _CopyCount  = 0;
    private int                   _ObjectOnPlatformCount = 0;
    private PlatformBehaviorBase[] _InteractionsCopy;



    //=======================================
    //////       Public Methods          ////
    //=======================================
    public void AddPlatformBehavior( PlatformBehaviorBase newReaction)
    {
        if (Behaviors.Contains(newReaction)) return;
        Behaviors.Add(newReaction);
        newReaction.BehaviorStart(this);

        RefreshInteractionCopy(true);
    }

    public void RemovePlatformBehavior( PlatformBehaviorBase removeReaction)
    {
        if (!Behaviors.Contains(removeReaction)) return;
        Behaviors.Remove(removeReaction);
        removeReaction.BehaviorEnd(this);

        RefreshInteractionCopy(true);
    }

    public bool IsContainsPlatformBehavior(PlatformBehaviorBase newReaction)
    {
        return Behaviors.Contains(newReaction);
    }

    public bool IsContainsPlatformBehavior<T>()
    {
        System.Type type = typeof(T);

        for(int i=0; i<_CopyCount; i++)
        {
            if (_InteractionsCopy[i].GetType().Equals(type)) return true;
        }

        return false;
    }


    //=======================================
    //////        Magic Methods          ////
    //=======================================
    private void Start()
    {
        if (_Controller == null)
        {
            _Controller = Player.Instance?.GetComponent<CharacterController>();
            _PlayerSM = Player.Instance.movementSM;
        }

        Collider = GetComponent<Collider>();
        gameObject.tag = "Platform";

        //Platform Behavior 초기화 작업...
        RefreshInteractionCopy(true);

        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++) 
        {
            _InteractionsCopy[i].BehaviorStart(this);
        } 
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
    }

    private void FixedUpdate()
    {
        //플레이어 적용
        if (PlayerRideOn)
        {
            if (_PlayerSM == null) _PlayerSM = Player.Instance.movementSM;

            RaycastHit hit;
            bool rayCastResult = GetPlayerFloorinfo(out hit);
            bool isSameCollider = (hit.collider == Collider);
            bool playerIsJumping = (_PlayerSM.currentState == Player.Instance.jump && _Controller.velocity.y > 0f);

            //최졍 적용
            if (rayCastResult && isSameCollider && !playerIsJumping)
            {
                //Call OnObjectPlatformStay
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformStay(this, Player.Instance.gameObject, hit.point, hit.normal );
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
            }
            else
            {
                //Call OnObjectPlatformExit
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformExit(this, Player.Instance.gameObject);
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
                PlayerRideOn = false;
                _ObjectOnPlatformCount--;
            }
        }

        //Call PhysicsUpdate
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++)
        {
            _InteractionsCopy[i].PhysicsUpdate(this);
        }
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint p = collision.GetContact(0);

        if (p.normal.y < 0 && collision.gameObject != Player.Instance.gameObject)
        {
            //OnObjectPlatformEnter
            _PkProgress = PendingKillProgress.PENDINGKILL_READY;
            for (int i = 0; i < _CopyCount; i++)
            {
                _InteractionsCopy[i].OnObjectPlatformEnter(this, Player.Instance.gameObject, p.point, p.normal);
            }
            RefreshInteractionCopy(true);
            _PkProgress = PendingKillProgress.NONE;
        }
    }



    //=======================================
    //////        Core Methods           ////
    //=======================================
    public void ExecPlatformEnter(Vector3 point, Vector3 normal)
    {
        //OnObjectPlatformEnter
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++)
        {
            _InteractionsCopy[i].OnObjectPlatformEnter(this, Player.Instance.gameObject, point, normal);
        }
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
    }

    public bool GetPlayerFloorinfo(out RaycastHit hit, float downMoveSpeed=0f)
    {
        #region Ommision
        Vector3 playerPos = _Controller.transform.position;
        float heightHalf = _Controller.height;
        float radius = _Controller.radius;
        float heightHalfOffset = (heightHalf * .5f) - radius;
        Vector3 center = _Controller.center;
        Vector3 center1 = playerPos + center + (Vector3.down * heightHalfOffset) + (Vector3.up * (1f+ CheckGroundOffset));
        Vector3 center2 = playerPos + center + (Vector3.up * heightHalfOffset) + (Vector3.up * (1f+ CheckGroundOffset));

        return Physics.CapsuleCast(
            center1,
            center2,
            radius,
            Vector3.down,
            out hit,
            heightHalf * 3f + downMoveSpeed+ CheckGroundOffset,
            1 << gameObject.layer
        );
        #endregion
    }

    private void RefreshInteractionCopy(bool copyRefresh=false)
    {
        bool isRefresh = (_PkProgress == PendingKillProgress.NONE || _PkProgress == PendingKillProgress.PENDINGKILL);
        if (copyRefresh && isRefresh)
        {
            _InteractionsCopy = Behaviors.ToArray();
            _CopyCount= Behaviors.Count;
        }
        else if( _PkProgress==PendingKillProgress.PENDINGKILL_READY )
        {
            _PkProgress= PendingKillProgress.PENDINGKILL;
        }
    }

    public bool Interact()
    {
        //플레이어가 낙하하여 해당 발판 위쪽에 착지했을 경우
        if (PlayerRideOn == false && _Controller.velocity.y <= 0f)
        {
            RaycastHit hit;
            GetPlayerFloorinfo(out hit);

            bool isSameCollider = (hit.collider == Collider);
            bool isLanded = (hit.normal.y > 0);

            if (isSameCollider && isLanded)
            {
                PlayerRideOn = true;
                _ObjectOnPlatformCount++;

                //OnObjectPlatformEnter
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformEnter(this, Player.Instance.gameObject, hit.point, hit.normal);
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
                return true;
            }
        }

        return false;
    }
}
