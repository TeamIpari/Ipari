using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

/***************************************************
 *  플레이어가 무언가를 당기는 상태가 구현된 클래스입니다.
 * **/
public sealed class PullInOutState : State
{
    private sealed class PullAnimation
    {
        public const string PLAYER_IDLE = "Idle";

        public const string IDLE = "PulloutPull_idle";
        public const string HOLD = "PulloutHold";
        public const string MOVE = "PulloutPullMove";
        public const string PUT  = "PulloutPut";
    }


    //=================================================
    ////////        Property and Fields           //////
    //=================================================
    public PullableObject PulledTarget { get; private set; }
    public float          PullPower    { get; set; }


    /**로직 관련...*/
    private Coroutine   _progressCoroutine;
    private const float _LookAtTime  = .1f;
    private bool        _applyIK     = false;

    /**IK 참조 관련...*/
    private GameObject  _GrabPos;
    private Transform   _LArm, _RArm;
    private Transform   _LForearm, _RForearm;
    private Transform   _LHand, _RHand;
    private float       _arm2ForearmLen, _forearm2HandLen;



    //=================================================
    ////////         Override methods            //////
    //=================================================
    public PullInOutState(Player player, StateMachine stateMachine, GameObject grapPos=null)
    :base(player, stateMachine)
    {
        #region Omit
        /***********************************************************
         *  플레이어가 잡는 위치 및 양 팔들의 트랜스폼의 참조를 구한다...
         * ***/
        if (grapPos == null){

            Transform bone = player.transform.Find("HoldingPoint");
            if (bone) _GrabPos = bone.gameObject;
        }
        else _GrabPos = grapPos;

        Transform playerTr      = player.transform;
        Transform modeling      = playerTr.Find("NewTavuti@Modeling");
        Transform con           = modeling.Find("NewTavuti_Con");
        Transform bip01         = con.Find("Bip001");
        Transform bip01Pelvis   = bip01.Find("Bip001 Pelvis");
        Transform bip01Spine    = bip01Pelvis.Find("Bip001 Spine").Find("Bip001 Spine1");
        Transform bipNeck       = bip01Spine.Find("Bip001 Neck");

        _LArm = bipNeck.Find("Bip001 L Clavicle");
        _RArm = bipNeck.Find("Bip001 R Clavicle");

        _LForearm    = _LArm.Find("Bip001 L UpperArm").Find("Bip001 L Forearm");
        _LHand       = _LForearm.Find("Bip001 L Hand");

        _RForearm   = _RArm.Find("Bip001 R UpperArm").Find("Bip001 R Forearm");
        _RHand      = _RForearm.Find("Bip001 R Hand");

        _LForearm = _LForearm.Find("Bip001 L ForeTwist");
        _RForearm = _RForearm.Find("Bip001 R ForeTwist");

        _arm2ForearmLen  = (_LHand.position - _LArm.position).magnitude;
        _forearm2HandLen = (_LHand.position - _LForearm.position).magnitude;


        /*********************************************************************
         *   IK를 적용하기 위한 AnimationHelper의 LateUpdate콜백을 등록한다...
         * ****/
        AnimatorHelper dispatcher = modeling.GetComponent<AnimatorHelper>();
        if(dispatcher){

            dispatcher.AnimatorLateUpdate += () =>{ SetArmRotate(); };
        }

        #endregion
    }

    public override void Enter()
    {
        base.Enter();
        player.isPull = true;
    }

    public override void Exit()
    {
        #region Omit
        base.Exit();
        player.isPull = false;

        if (_progressCoroutine != null){

            player.StopCoroutine(_progressCoroutine);
            _progressCoroutine = null;
        }
        #endregion
    }

    public override void HandleInput()
    {
        #region Omit
        base.HandleInput();

        /**입력한 4방향키의 방향을 구한다...*/
        input = pushZAxisAction.ReadValue<Vector2>();
        input += pushXAxisAction.ReadValue<Vector2>();
        #endregion
    }

    

    //=================================================
    ////////            Core methods            //////
    //=================================================
    public void HoldTarget(GameObject target)
    {
        #region Omit
        Vector3 targetDistance = (target.transform.position - player.transform.position);

        bool isSameDir  = Vector3.Dot(player.transform.forward, targetDistance) >= 0f;
        bool isPullable = (PulledTarget = target.GetComponent<PullableObject>());

        /**당길 수 있는 객체라면 당기는 작업을 실행한다..*/
        if(isPullable){

            if(_progressCoroutine!=null) player.StopCoroutine(_progressCoroutine);
            _progressCoroutine = player.StartCoroutine(PullingProgress());
        }
        #endregion
    }

    private IEnumerator PullingProgress()
    {
        #region Omit
        if (_GrabPos == null) yield break;

        /****************************************************
         *   당길 위치를 바라보는데 필요한 모든 요소들을 구한다...
         * **/
        Animator            animator  = player.animator;
        Transform           playerTr  = player.transform;
        CharacterController playerCon = player.controller;

        Vector3     grabPoint       = PulledTarget.EndPosition;
        Vector3     rootPoint       = PulledTarget.StartPosition;
        Vector3     playerPos       = player.transform.position;
        Vector3     playerGrabPos   = _GrabPos.transform.position;
        Vector3     startDir        = playerTr.forward;
        Vector3     goalDir         = (rootPoint - playerPos).normalized;
        Vector3     movDir          = (grabPoint - playerGrabPos).normalized;
        Quaternion  startQuat       = playerTr.rotation;

        float movDistance   = (grabPoint - playerPos).magnitude;
        float progressRatio = 0f;
        float currTime      = 0f;
        float goalTimeDiv   = (1f / _LookAtTime);

        goalDir.y = 0f;
        movDir.y = 0f;


        /*********************************************
         *   대상을 바라본다...
         * ***/
        animator.Play(PullAnimation.HOLD);

        do{

            float deltaTime = Time.deltaTime;
            progressRatio   = Mathf.Clamp(((currTime += deltaTime) * goalTimeDiv), 0f, 1f);

            Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(startDir, goalDir, progressRatio);
            Vector3    movVec   = movDir * (movDistance*progressRatio);
            movVec.y = 0f;

            /**최종적용*/
            playerTr.rotation  = (rotQuat * startQuat);
            playerTr.position  = (playerPos + movVec);

            yield return null;
        }
        while (currTime < _LookAtTime);


        /*******************************************************
         *   해당 PullableObject를 잡고, 끊어질 때까지 당긴다.....
         * ***/
        _applyIK = true;

        float fullLen    = PulledTarget.MaxLength;
        float fullLenDiv = (1f/fullLen);
        bool isMove      = false;

        /**대상을 잡는다...*/
        PulledTarget.HoldingPoint = _GrabPos;
        while(PulledTarget.IsBroken==false)
        {
            /******************************************
             *   당기는 동작에 필요한 계산들을 모두 구한다...
             * ***/
            Vector3 playerDir   = playerTr.forward;
            Vector3 lookDir     = (rootPoint - playerTr.position).normalized;
            Vector3 moveDir     = new Vector3(input.x, 0f, input.y);
            bool isPulling      = Vector3.Dot(moveDir, lookDir) <= 0;
            float deltaTime     = Time.deltaTime * 2f;
            float exLenRatio    = Mathf.Clamp(PulledTarget.NormalizedLength - .7f, 0f, .85f);
            float speed         = fullLen;

            /**당겨질 때에만 저항력을 적용한다....*/
            if (isPulling){

                speed -= (fullLen * exLenRatio);
            }

            speed = Mathf.Clamp(speed, 0f, 4f);

            /**바라볼 방향을 향하는 쿼터니언을 구한다...*/
            lookDir.y = 0f;
            Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(playerDir, lookDir);

            /**상호작용키를 입력시 손을 놓는다...*/
            if(Input.GetKeyDown(KeyCode.F))
            {
                _applyIK = false;
                animator.SetFloat("speed", 0f);

                animator.Play(PullAnimation.PLAYER_IDLE);
                PulledTarget.HoldingPoint = null;
                player.movementSM.ChangeState(player.idle);
                player.isPull = false;
                yield break;
            }

            /**최종 적용...*/
            playerCon.SimpleMove(moveDir * speed * 2f);
            playerTr.rotation = (rotQuat * playerTr.rotation);


            /**이동에 따른 애니메이션 적용...*/
            SetMoveAnim(ref isMove, input.sqrMagnitude);

            /**이동속도*/
            animator.SetFloat("speed", (speed * fullLenDiv));

            yield return null;
        }


        /**********************************************
         *   줄이 끊어지고 플레이어가 넘어진다.....
         * ****/
        _applyIK = false;
        animator.SetFloat("speed", 0f);
        animator.Play(PullAnimation.PUT);

        float leftTime = 2.4f;
        while((leftTime-=Time.deltaTime)>0) yield return null;

        player.isPull = false;
        player.movementSM.ChangeState(player.idle);
        #endregion
    }

    private void SetMoveAnim(ref bool isMove, float speed)
    {
        #region Omit
        if (!isMove && speed > 0f)
        {
            isMove = true;
            Player.Instance.animator.Play(PullAnimation.MOVE);
        }
        else if (isMove && input.sqrMagnitude <= 0f)
        {

            isMove = false;
            Player.Instance.animator.Play(PullAnimation.IDLE);
        }
        #endregion
    }

    private void SetArmRotate()
    {
        #region Omit
        if (_applyIK == false) return;

        /*********************************************
         *   잡게될 지점과, 잡은 본의 방향을 구한다....
         * ******/

        /**손이 잡게될 위치를 얻어온다...*/
        Vector3 grabPos, grabDir;
        PulledTarget.GetBonePositionAndDirFromLength(
            (PulledTarget.MaxLength - .1f), 
            out grabPos, 
            out grabDir
        );

        /**양손이 향하게될 오프셋을 구한다....*/
        Vector3 grabRight = -Vector3.Cross(Vector3.up, grabDir);


        /******************************************************
         *   각 본들이 위치해야할 위치와 회전값과 방향을 구한다...
         * ******/

        /**Hands...*/
        Vector3 LHandGoalPos  = grabPos + (grabRight * -.02f);
        Vector3 LHandDir      = _LHand.transform.up;
        Vector3 RHandGoalPos  = grabPos + (grabRight * .1f);
        Vector3 RHandDir      = _RHand.transform.up;

        /**ForeArms....*/
        Vector3 LForeArmDir     = (_LHand.position - _LForearm.position).normalized;
        Vector3 LForeArmGoalDir = (LHandGoalPos - _LForearm.position).normalized;
        Vector3 LForeArmPos     = LHandGoalPos + (_LHand.right*_forearm2HandLen);
        Vector3 RForeArmDir       = (_RHand.position - _RForearm.position).normalized;
        Vector3 RForeArmGoalDir   = (RHandGoalPos - _RForearm.position).normalized;
        Vector3 RForeArmPos       = RHandGoalPos + (_RHand.right * _forearm2HandLen);

        /**Arms...*/
        Vector3 LArmDir     = (_LForearm.position - _LArm.position).normalized;
        Vector3 LArmGoalDir = (LHandGoalPos - _LArm.position).normalized;
        Vector3 LArmPos     = LForeArmPos - (LArmGoalDir * _arm2ForearmLen);
        Vector3 RArmDir        = (_RForearm.position - _RArm.position).normalized;
        Vector3 RArmGoalDir    = (RHandGoalPos - _RArm.position).normalized;
        Vector3 RArmPos        = RForeArmPos - (RArmGoalDir * _arm2ForearmLen);


        /************************************************
         *    각 본들에게 최종 적용을 한다.....
         * ****/
        _LArm.rotation = (IpariUtility.GetQuatBetweenVector(LArmDir, LArmGoalDir) * _LArm.rotation);
        _RArm.rotation = (IpariUtility.GetQuatBetweenVector(RArmDir, RArmGoalDir) * _RArm.rotation);

        //_LForearm.position = LForeArmPos;
        //_LForearm.rotation = (IpariUtility.GetQuatBetweenVector(LForeArmDir, LForeArmGoalDir) * _LForearm.rotation);
        //_RForearm.position = RForeArmPos;
        //_RForearm.rotation = (IpariUtility.GetQuatBetweenVector(RForeArmDir, RForeArmGoalDir) * _RForearm.rotation);

        _LHand.rotation = (IpariUtility.GetQuatBetweenVector(LHandDir, grabRight) * _LHand.transform.rotation);
        //_LHand.position = LHandGoalPos;
        _RHand.rotation = (IpariUtility.GetQuatBetweenVector(RHandDir, -grabRight) * _RHand.transform.rotation);
       // _RHand.position = RHandGoalPos;

        #endregion
    }
}
