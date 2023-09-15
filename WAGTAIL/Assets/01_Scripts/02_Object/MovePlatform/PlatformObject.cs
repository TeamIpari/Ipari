using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


/********************************************************
 * 오브젝트들이 밟을 수 있는 플랫폼의 기반이 되는 클래스입니다.
 ****/
[AddComponentMenu("Platform/PlatformObject")]
public sealed class PlatformObject : MonoBehaviour, IEnviroment
{
    #region Editor_Extension
    /**************************************
     *   에디터 확장을 위한 private class.
     * ***/
#if UNITY_EDITOR
    [CustomEditor(typeof(PlatformObject))]
    private sealed class PlatformObjectEditor : Editor
    {
        //===================================
        //////        Property          /////
        //===================================
        SerializedProperty PlayerRideOnProperty;
        SerializedProperty OnObjectCountProperty;
        SerializedProperty BehaviorListProperty;
        SerializedProperty UsedCollisionProperty;



        //=======================================
        /////       Override methods         ////
        //=======================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*********************************
             *   모든 프로퍼티들을 표시한다...
             * ***/
            GUI_Initialized();

            GUI_ShowPlayerOnPlatform();

            GUI_ShowObjectOnPlatform();

            GUI_ShowUSedCollision();

            /**값이 변경되었다면 갱신한다...*/
            if (GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }



        //========================================
        //////         GUI methods            ////
        //========================================
        private void GUI_Initialized()
        {
            #region Omit
            /*********************************
             *   모든 프로퍼티들을 초기화한다...
             * ***/
            if(PlayerRideOnProperty==null){

                PlayerRideOnProperty = serializedObject.FindProperty("_PlayerOnPlatform");
            }

            if(OnObjectCountProperty==null){

                OnObjectCountProperty = serializedObject.FindProperty("_ObjectOnPlatformCount");
            }

            if(BehaviorListProperty==null){

                BehaviorListProperty = serializedObject.FindProperty("Behaviors");
            }

            if(UsedCollisionProperty==null){

                UsedCollisionProperty = serializedObject.FindProperty("UsedCollision");
            }
            #endregion
        }

        private void GUI_ShowPlayerOnPlatform()
        {
            #region Omit
            if (PlayerRideOnProperty == null) return;

            EditorGUILayout.Toggle("Player OnPlatform", PlayerRideOnProperty.boolValue);

            #endregion
        }

        private void GUI_ShowObjectOnPlatform()
        {
            #region Omit
            if (OnObjectCountProperty == null) return;

            bool isOverZero = (OnObjectCountProperty.intValue>0);
            EditorGUILayout.Toggle("Object OnPlatform", isOverZero);
            #endregion
        }

        private void GUI_ShowBehaviorList()
        {
            #region Omit
            if (BehaviorListProperty == null) return;

            EditorGUILayout.PropertyField(BehaviorListProperty, true);

            #endregion
        }

        private void GUI_ShowUSedCollision()
        {
            #region Omit
            if (UsedCollisionProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                bool value = EditorGUILayout.Toggle("Used Collision", UsedCollisionProperty.boolValue);

                /**값이 바뀌었다면 갱신...*/
                if(scope.changed){

                    UsedCollisionProperty.boolValue = value;    
                }
            }
            #endregion
        }
    }
#endif
    #endregion

    private enum PendingKillProgress
    {
        NONE,
        PENDINGKILL_READY,
        PENDINGKILL
    }

    //========================================
    //////          Property            /////
    //========================================
    public string   EnviromentPrompt { get; }      = string.Empty;
    public bool     IsHit            { get; set; } = false;
    public bool     PlayerOnPlatform { get { return _PlayerOnPlatform; } }
    public bool     ObjectOnPlatform { get { return (_ObjectOnPlatformCount>0);  } }
    public Collider Collider         { get { return _Collider; } set{ if (value != null) _Collider = value;  }  }

    [HideInInspector] public Quaternion UpdateQuat      = Quaternion.identity;
    [HideInInspector] public Quaternion OffsetQuat      = Quaternion.identity;
    [HideInInspector] public Vector3    UpdatePosition  = Vector3.zero;
    [HideInInspector] public Vector3    OffsetPosition  = Vector3.zero;
    [HideInInspector] public Vector3    PrevPosition    = Vector3.zero;
    [HideInInspector] public bool       PreventAddChild = false;

    [HideInInspector][SerializeField]  
    private bool _PlayerOnPlatform = false;

    [HideInInspector][SerializeField]  
    public bool UsedCollision = true;

    [HideInInspector] public float CheckGroundOffset    = 0f;
    [HideInInspector] public float CheckGroundDownOffset = 0f;



    //==========================================
    //////             Fields               ////
    //==========================================
    [SerializeField] 
    private List<PlatformBehaviorBase> Behaviors = new List<PlatformBehaviorBase>();

    [HideInInspector][SerializeField] 
    private int _ObjectOnPlatformCount = 0;

    private List<GameObject> _ignoreExitList;
    private List<GameObject> _ignoreEnterList;
    private List<GameObject> _onPlatformObjects = new List<GameObject>();

    /**플레이어 관련 클래스 캐싱*/
    private static CharacterController _Controller;
    private static StateMachine  _PlayerSM;

    private int layer = 0;

    /**Interactions 관련*/
    private Transform _Tr;
    private Rigidbody _body;
    private Collider  _Collider;
    private PendingKillProgress    _PkProgress = PendingKillProgress.NONE;
    private int                    _CopyCount  = 0;
    private PlatformBehaviorBase[] _InteractionsCopy;



    //=======================================
    //////        Magic Methods          ////
    //=======================================
    private void Start()
    {
        #region Omit
        if (_Controller == null)
        {
            _Controller = Player.Instance?.GetComponent<CharacterController>();
            _PlayerSM = Player.Instance.movementSM;
        }

        if(UsedCollision)
        {
            _body = GetComponent<Rigidbody>();  
        }

        if(Collider==null) Collider = GetComponent<Collider>();
        _Tr = transform;
        gameObject.tag   = "Platform";

        /**적절한 레이어를 세팅한다..*/
        if ((layer = LayerMask.NameToLayer("Platform")) >= 0){

            gameObject.layer = layer;
            layer = (1 << layer);
        }
        else layer = 0;

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
        PrevPosition = _Tr.position;
        _Tr.rotation = (UpdateQuat * OffsetQuat);
        _Tr.position = (UpdatePosition + OffsetPosition);
        OffsetQuat      = Quaternion.identity;
        OffsetPosition  = Vector3.zero;

        //플레이어 적용
        if (PlayerOnPlatform)
        {
            if (_PlayerSM == null) _PlayerSM = Player.Instance.movementSM;

            RaycastHit hit;
            bool rayCastResult   = GetPlayerFloorinfo(out hit);
            bool isSameCollider  = (hit.collider == Collider);
            bool playerIsJumping = (_PlayerSM.currentState == Player.Instance.jump && _Controller.velocity.y > 0f);

            //최졍 적용
            if (rayCastResult && isSameCollider && !playerIsJumping && hit.normal.y>0f)
            {
                #region Call_OnObjectPlatformStay
                /**만약 해당 객체가 부모가 없을 때만 자식으로 넣는다.*/
                if (Player.Instance.transform.parent == null && !PreventAddChild) {

                    Player.Instance.transform.parent = this.transform;
                }

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

                _onPlatformObjects.Remove(_Controller.gameObject);
                _Controller.transform.parent = null;
                _PlayerOnPlatform = false;
                _ObjectOnPlatformCount--;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region Omit
        if (UsedCollision == false) return;

        /***********************************
         *   enter의 처리를 안하는 경우의 처리.
         * ***/
        bool NoAddParent = false;
        if (_ignoreEnterList != null){

            int EnterCount = _ignoreEnterList.Count;
            for (int i = 0; i < EnterCount; i++)
            {
                /**exit 목록에 존재할 경우, 순서보장을 위해 탈출...*/
                if (collision.gameObject.Equals(_ignoreEnterList[i])){

                    _ignoreEnterList.RemoveAt(i);
                    NoAddParent = true;
                    break;
                }
            }
        }


        /**************************************
         *  해당 발판위를 밟고 있는지 확인한다...
         * **/
        Vector3 normal  = Vector3.zero;
        Vector3 point   = Vector3.zero;
        int     Count   = collision.contactCount;
        bool    result  = false;
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
            _ObjectOnPlatformCount++;
            _onPlatformObjects.Add(collision.gameObject);
            if(!NoAddParent && !PreventAddChild) collision.transform.parent = transform;
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
        else if(collision.gameObject != Player.Instance.gameObject)
        {
            /**부모가 해당 객체일 경우에만 부모에서 해제시킨다.*/
            if (collision.transform.parent == transform){

                _ObjectOnPlatformCount--;
                collision.transform.parent = null;
            }
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

    private void OnCollisionExit(Collision collision)
    {
        #region Omit
        if (UsedCollision == false) return;

        /***********************************
         *   exit의 처리를 안하는 경우의 처리.
         * ***/
        if(_ignoreExitList != null){

            int Count = _ignoreExitList.Count;
            for(int i=0; i<Count; i++)
            {
                /**exit 목록에 존재할 경우, 순서보장을 위해 탈출...*/
                if (collision.gameObject.Equals(_ignoreExitList[i])){

                    _onPlatformObjects.Remove(collision.gameObject);
                    _ignoreExitList.RemoveAt(i);
                    return;
                }
            }
        }

        if (collision.gameObject != Player.Instance.gameObject)
        {
            /**부모가 해당 객체일 경우에만 부모에서 해제시킨다.*/
            if (collision.transform.parent == transform){

                collision.transform.parent = null;
                _onPlatformObjects.Remove(collision.gameObject);
                _ObjectOnPlatformCount--;
            }
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

    private void OnDestroy()
    {
        #region Omit

        /********************************************
         *   해당 발판을 밟은 오브젝트들을 자식에서 제거한다...
         * ***/
        int Count = _onPlatformObjects.Count;
        for(int i=0; i<Count; i++)
        {
            Transform objTransform = _onPlatformObjects[i].transform;

            if (objTransform.parent==transform){

                if (gameObject.activeInHierarchy)
                    objTransform.transform.SetParent(null);
            }
        }
        #endregion
    }



    //=======================================
    //////        Core Methods           ////
    //=======================================
    public void IgnoreCollisionExit(GameObject ignoreObj)
    {
        #region Omit
        if (_ignoreExitList == null){

            _ignoreExitList = new List<GameObject>();
        }
        _ignoreExitList.Add(ignoreObj);
        #endregion
    }

    public void IgnoreCollisionEnter(GameObject ignoreObj)
    {
        #region Omit
        if (_ignoreEnterList == null){

            _ignoreEnterList = new List<GameObject>();
        }
        _ignoreEnterList.Add(ignoreObj);
        #endregion
    }

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

    public bool GetPlayerFloorinfo(out RaycastHit hit)
    {
        #region Ommision
        float heightHalf       = _Controller.height;
        float radius           = _Controller.radius;
        float heightHalfOffset = (heightHalf * .5f) - radius;
        Vector3 playerPos      = _Controller.transform.position;
        Vector3 center         = ( playerPos + _Controller.center ) + (Vector3.up*CheckGroundOffset);

        return Physics.SphereCast(
            center,
            radius,
            Vector3.down,
            out hit,
            heightHalf+.1f+ CheckGroundDownOffset+ CheckGroundOffset,
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
        #region Omit
        //플레이어가 낙하하여 해당 발판 위쪽에 착지했을 경우
        if (PlayerOnPlatform == false && _Controller.velocity.y <= 0f)
        {
            RaycastHit hit;
            GetPlayerFloorinfo(out hit);

            bool isSameObject   = (hit.transform.gameObject.Equals(gameObject));
            bool isLanded       = (hit.normal.y > 0);

            if (isSameObject && isLanded)
            {
                /**만약 해당 객체가 부모가 없을 때만 자식으로 넣는다.*/
                _PlayerOnPlatform = true;
                _ObjectOnPlatformCount++;
                _onPlatformObjects.Add(_Controller.gameObject);

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
        #endregion
    }

    public void ExecutionFunction(float time)
    {
        // 무빙 플랫폼 전용인데... 일단 임시
        #region Call_OnObjectPlatformEnter
        _PkProgress = PendingKillProgress.PENDINGKILL_READY;
        for (int i = 0; i < _CopyCount; i++)
        {
            _InteractionsCopy[i].OnObjectPlatformEnter(this, null, null, Vector3.zero, Vector3.zero);
        }
        RefreshInteractionCopy(true);
        _PkProgress = PendingKillProgress.NONE;
        #endregion
    }

    public void AddPlatformBehavior(PlatformBehaviorBase newReaction)
    {
        #region Omit
        if (Behaviors.Contains(newReaction)) return;
        Behaviors.Add(newReaction);
        newReaction.BehaviorStart(this);

        RefreshInteractionCopy(true);
        #endregion
    }

    public void RemovePlatformBehavior(PlatformBehaviorBase removeReaction)
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

        if (isChanged) RefreshInteractionCopy(true);

        #endregion
    }

    public PlatformBehaviorBase GetPlatformBehavior<T>()
    {
        #region Omit
        System.Type type = typeof(T);

        for (int i = 0; i < _CopyCount; i++)
        {
            if (_InteractionsCopy[i].GetType().Equals(type))
                return _InteractionsCopy[i];
        }

        return null;
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

        for (int i = 0; i < _CopyCount; i++)
        {
            if (_InteractionsCopy[i].GetType().Equals(type)) return true;
        }

        return false;
        #endregion
    }
}
