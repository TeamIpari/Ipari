using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


/********************************************************
 * 오브젝트들이 밟을 수 있는 플랫폼의 기반이 되는 클래스입니다.
 ****/
[AddComponentMenu("Platform/PlatformObject")]
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

    [HideInInspector] public Quaternion UpdateQuat      = Quaternion.identity;
    [HideInInspector] public Quaternion OffsetQuat      = Quaternion.identity;
    [HideInInspector] public Vector3    UpdatePosition  = Vector3.zero;
    [HideInInspector] public Vector3    OffsetPosition  = Vector3.zero;
    [SerializeField]  public bool PlayerOnPlatform      = false;
    [SerializeField]  public bool UsedCollision         = false;
    [HideInInspector] public float CheckGroundOffset    = 0f;


    [SerializeField] private List<PlatformBehaviorBase> Behaviors = new List<PlatformBehaviorBase>();

    /**플레이어 관련 클래스 캐싱*/
    private static CharacterController _Controller;
    private StateMachine  _PlayerSM;

    private int layer = 0;

    /**Interactions 관련*/
    private Transform _Tr;
    private Collider  _Collider;
    private PendingKillProgress    _PkProgress = PendingKillProgress.NONE;
    private int                    _CopyCount  = 0;
    private int                    _ObjectOnPlatformCount = 0;
    private PlatformBehaviorBase[] _InteractionsCopy;



    //=======================================
    //////       Public Methods          ////
    //=======================================
    public void AddPlatformBehavior( PlatformBehaviorBase newReaction)
    {
        #region Omit
        if (Behaviors.Contains(newReaction)) return;
        Behaviors.Add(newReaction);
        newReaction.BehaviorStart(this);

        RefreshInteractionCopy(true);
        #endregion
    }

    public void RemovePlatformBehavior( PlatformBehaviorBase removeReaction)
    {
        #region Omit
        if (!Behaviors.Contains(removeReaction)) return;
        Behaviors.Remove(removeReaction);
        removeReaction.BehaviorEnd(this);

        RefreshInteractionCopy(true);
        #endregion
    }

    public void RemovePlatformBehaviorByType<T>()
    {
        #region Omit
        System.Type type = typeof(T);
        bool isChanged = false;

        for (int i = 0; i < _CopyCount; i++)
        {
            if (_InteractionsCopy[i].GetType().Equals(type))
            {
                Behaviors.RemoveAt(i);
                isChanged = true;
            }
        }

        if(isChanged) RefreshInteractionCopy(true);

        #endregion
    }

    public bool IsContainsPlatformBehavior(PlatformBehaviorBase newReaction)
    {
        return Behaviors.Contains(newReaction);
    }

    public bool IsContainsPlatformBehavior<T>()
    {
        #region Omit
        System.Type type = typeof(T);

        for(int i=0; i<_CopyCount; i++)
        {
            if (_InteractionsCopy[i].GetType().Equals(type)) return true;
        }

        return false;
        #endregion
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

        if(Collider==null) Collider = GetComponent<Collider>();
        _Tr = transform;
        gameObject.tag = "Platform";
        layer          = ( 1<<gameObject.layer );
        UpdateQuat      = transform.rotation;
        UpdatePosition  = transform.position;

        //Platform Behavior 초기화 작업...
        RefreshInteractionCopy(true);

        #region Call_BehaviorStart
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++) 
        {
            /************************************
             *   배열의 원소가 유효하지 않다면 제거...
             * ***/
            if (_InteractionsCopy[i]==null){

                _PkProgress= PendingKillProgress.PENDINGKILL;
                Behaviors.Remove(_InteractionsCopy[i]);
                continue;
            }

            _InteractionsCopy[i].BehaviorStart(this);
        } 
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
        #endregion 
    }

    private void FixedUpdate()
    {
        #region Call_PhysicsUpdate
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++)
        {
            _InteractionsCopy[i].PhysicsUpdate(this);
        }
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
        #endregion

        /**공통 변경사항 적용...*/
        _Tr.rotation    = (UpdateQuat * OffsetQuat);
        _Tr.position    = (UpdatePosition + OffsetPosition);
        OffsetQuat      = Quaternion.identity;
        OffsetPosition  = Vector3.zero;

        //플레이어 적용
        if (PlayerOnPlatform)
        {
            if (_PlayerSM == null) _PlayerSM = Player.Instance.movementSM;

            RaycastHit hit;
            bool rayCastResult = GetPlayerFloorinfo(out hit);
            bool isSameCollider = (hit.collider == Collider);
            bool playerIsJumping = (_PlayerSM.currentState == Player.Instance.jump && _Controller.velocity.y > 0f);

            //최졍 적용
            if (rayCastResult && isSameCollider && !playerIsJumping && hit.normal.y>0f)
            {
                #region Call_OnObjectPlatformStay
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformStay(this, Player.Instance.gameObject, null, hit.point, hit.normal );
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
                #endregion
            }
            else
            {
                #region Call_OnObjectPlatformExit
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformExit(this, Player.Instance.gameObject, null);
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
                #endregion

                PlayerOnPlatform = false;
                _ObjectOnPlatformCount--;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region Omit
        if (UsedCollision == false) return;

        /**************************************
         *  해당 발판위를 밟고 있는지 확인한다...
         * **/
        Vector3 normal = Vector3.zero;
        Vector3 point = Vector3.zero;
        int Count = collision.contactCount;
        bool result = false;
        for(int i=0; i<Count; i++)
        {
            ContactPoint p = collision.GetContact(i);
            normal = p.normal;

            if (normal.y<0f){

                point = p.point;
                result = true;
                break;
            }
        }

        /**해당 오브젝트가 발판을 밟았을 경우의 처리*/
        if (result && collision.gameObject != Player.Instance.gameObject)
        {
            collision.transform.SetParent(transform, true);
            #region Call_OnObjectPlatformEnter
            _PkProgress = PendingKillProgress.PENDINGKILL_READY;
            for (int i = 0; i < _CopyCount; i++)
            {
                _InteractionsCopy[i].OnObjectPlatformEnter(this, collision.gameObject, collision.rigidbody, point, normal);
            }
            RefreshInteractionCopy(true);
            _PkProgress = PendingKillProgress.NONE;
            #endregion
        }

        #endregion
    }

    private void OnCollisionStay(Collision collision)
    {
        #region Omit
        if (UsedCollision == false) return;

        /**************************************
         *  해당 발판위를 밟고 있는지 확인한다...
         * **/
        Vector3 normal = Vector3.zero;
        Vector3 point  = Vector3.zero;
        int Count = collision.contactCount;
        bool result = false;
        for (int i = 0; i < Count; i++)
        {
            ContactPoint p = collision.GetContact(i);
            normal = p.normal;

            if (normal.y < 0f){

                point = p.point;
                result = true;
                break;
            }
        }

        /**해당 오브젝트가 발판을 밟았을 경우의 처리*/
        if (result && collision.gameObject != Player.Instance.gameObject)
        {
            #region Call_OnObjectPlatformStay
            _PkProgress = PendingKillProgress.PENDINGKILL_READY;
            for (int i = 0; i < _CopyCount; i++)
            {
                _InteractionsCopy[i].OnObjectPlatformStay(this, collision.gameObject, collision.rigidbody, point, normal);
            }
            RefreshInteractionCopy(true);
            _PkProgress = PendingKillProgress.NONE;
            #endregion
        }
        else
        {
            /**부모-자식 관계를 해제한다.*/
            Transform exitTr = collision.transform;
            if (exitTr.parent == transform) {

                exitTr.parent = null;
            }
        }

        #endregion
    }

    private void OnCollisionExit(Collision collision)
    {
        #region Omit
        if (UsedCollision == false) return;

        /**부모-자식 관계를 해제한다.*/
        Transform exitTr = collision.transform;
        if(exitTr.parent == transform){

            exitTr.parent = null;
        }

        if (collision.gameObject != Player.Instance.gameObject)
        {
            #region Call_OnObjectPlatformExit
            _PkProgress = PendingKillProgress.PENDINGKILL_READY;
            for (int i = 0; i < _CopyCount; i++)
            {
                _InteractionsCopy[i].OnObjectPlatformExit(this, Player.Instance.gameObject, collision.rigidbody);
            }
            RefreshInteractionCopy(true);
            _PkProgress = PendingKillProgress.NONE;
            #endregion
        }
        #endregion
    }



    //=======================================
    //////        Core Methods           ////
    //=======================================
    public void ExecPlatformEnter(Vector3 point, Vector3 normal)
    {
        #region Call_OnObjectPlatformEnter
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++)
        {
            _InteractionsCopy[i].OnObjectPlatformEnter(this, Player.Instance.gameObject, null, point, normal);
        }
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
        #endregion
    }

    public bool GetPlayerFloorinfo(out RaycastHit hit, float downMoveSpeed=0f)
    {
        #region Ommision
        float heightHalf       = _Controller.height;
        float radius           = _Controller.radius;
        float heightHalfOffset = (heightHalf * .5f) - radius;
        Vector3 playerPos      = _Controller.transform.position;
        Vector3 center         = ( playerPos + _Controller.center );

        return Physics.SphereCast(
            center,
            radius,
            Vector3.down,
            out hit,
            heightHalf+.1f,
            layer
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
        if (PlayerOnPlatform == false && _Controller.velocity.y <= 0f)
        {
            RaycastHit hit;
            GetPlayerFloorinfo(out hit);

            bool isSameObject   = (hit.transform.gameObject.Equals(gameObject));
            bool isLanded       = (hit.normal.y > 0);

            if (isSameObject&& isLanded)
            {
                PlayerOnPlatform = true;
                _ObjectOnPlatformCount++;

                #region Call_OnObjectPlatformEnter
                _PkProgress = PendingKillProgress.PENDINGKILL_READY;
                for (int i = 0; i < _CopyCount; i++)
                {
                    _InteractionsCopy[i].OnObjectPlatformEnter(this, Player.Instance.gameObject, null, hit.point, hit.normal);
                }
                RefreshInteractionCopy(true);
                _PkProgress = PendingKillProgress.NONE;
                #endregion

                return true;
            }
        }

        return false;
    }

    public void ExecutionFunction(float time)
    {
        Debug.Log($"Not Have Function");
    }
}
