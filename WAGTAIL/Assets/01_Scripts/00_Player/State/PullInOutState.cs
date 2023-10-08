using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/***************************************************
 *  �÷��̾ ���𰡸� ���� ���°� ������ Ŭ�����Դϴ�.
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


    /**���� ����...*/
    private Coroutine   _progressCoroutine;
    private const float _LookAtTime  = .1f;
    private bool        _applyIK     = false;

    /**IK ���� ����...*/
    private GameObject  _GrabPos;
    private Transform   _LArm, _RArm;
    private Transform   _LForearm, _RForearm;
    private Transform   _bip01Pelvis;



    //=================================================
    ////////         Override methods            //////
    //=================================================
    public PullInOutState(Player player, StateMachine stateMachine, GameObject grapPos=null)
    :base(player, stateMachine)
    {
        #region Omit
        /***********************************************************
         *  �÷��̾ ��� ��ġ �� �� �ȵ��� Ʈ�������� ������ ���Ѵ�...
         * ***/
        if(grapPos==null)
        {
            _GrabPos = player.transform.Find("HoldingPoint").gameObject;
        }
        else _GrabPos = grapPos;

        Transform playerTr      = player.transform;
        Transform modeling      = playerTr.Find("NewTavuti@Modeling");
        Transform con           = modeling.Find("NewTavuti_Con");
        Transform bip01         = con.Find("Bip001");
        Transform bip01Pelvis   = _bip01Pelvis = bip01.Find("Bip001 Pelvis");
        Transform bip01Spine    = bip01Pelvis.Find("Bip001 Spine").Find("Bip001 Spine1");
        Transform bipNeck       = bip01Spine.Find("Bip001 Neck");

        _LArm     = bipNeck.Find("Bip001 L Clavicle");
        _RArm     = bipNeck.Find("Bip001 R Clavicle");
        _LForearm = _LArm.transform.Find("Bip001 L UpperArm").Find("Bip001 L Forearm");
        _RForearm = _RArm.transform.Find("Bip001 R UpperArm").Find("Bip001 R Forearm");


        /*********************************************************************
         *   IK�� �����ϱ� ���� AnimationHelper�� LateUpdate�ݹ��� ����Ѵ�...
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

        /**�Է��� 4����Ű�� ������ ���Ѵ�...*/
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

        /**��� �� �ִ� ��ü��� ���� �۾��� �����Ѵ�..*/
        if(isPullable){

            player.movementSM.ChangeState(this);

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
         *   ��� ��ġ�� �ٶ󺸴µ� �ʿ��� ��� ��ҵ��� ���Ѵ�...
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
         *   ����� �ٶ󺻴�...
         * ***/
        animator.Play(PullAnimation.HOLD);

        do{

            float deltaTime = Time.deltaTime;
            progressRatio   = Mathf.Clamp(((currTime += deltaTime) * goalTimeDiv), 0f, 1f);

            Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(startDir, goalDir, progressRatio);
            Vector3    movVec   = movDir * (movDistance*progressRatio);
            movVec.y = 0f;

            /**��������*/
            playerTr.rotation  = (rotQuat * startQuat);
            playerTr.position  = (playerPos + movVec);

            yield return null;
        }
        while (currTime < _LookAtTime);


        /*******************************************************
         *   �ش� PullableObject�� ���, ������ ������ ����.....
         * ***/
        _applyIK = true;

        float fullLen    = PulledTarget.MaxLength;
        float fullLenDiv = (1f/fullLen);
        bool isMove      = false;

        /**����� ��´�...*/
        PulledTarget.HoldingPoint = _GrabPos;
        while(PulledTarget.IsBroken==false)
        {
            /******************************************
             *   ���� ���ۿ� �ʿ��� ������ ��� ���Ѵ�...
             * ***/
            Vector3 playerDir   = playerTr.forward;
            Vector3 lookDir     = (rootPoint - playerTr.position).normalized;
            Vector3 moveDir     = new Vector3(input.x, 0f, input.y);
            bool isPulling      = Vector3.Dot(input, lookDir) < 0;
            float deltaTime     = Time.deltaTime * 2f;
            float exLenRatio    = Mathf.Clamp(PulledTarget.NormalizedLength - .7f, 0f, .85f);
            float speed         = fullLen;

            /**����� ������ ���׷��� �����Ѵ�....*/
            if (isPulling)
                    speed -= (fullLen * exLenRatio);


            /**�ٶ� ������ ���ϴ� ���ʹϾ��� ���Ѵ�...*/
            lookDir.y = 0f;
            Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(playerDir, lookDir);

            /**��ȣ�ۿ�Ű�� �Է½� ���� ���´�...*/
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

            /**���� ����...*/
            playerCon.SimpleMove(moveDir * speed * 2f);
            playerTr.rotation = (rotQuat * playerTr.rotation);


            /**�̵��� ���� �ִϸ��̼� ����...*/
            SetMoveAnim(ref isMove, input.sqrMagnitude);

            /**�̵��ӵ�*/
            animator.SetFloat("speed", (speed * fullLenDiv));

            yield return null;
        }


        /**********************************************
         *   ���� �������� �÷��̾ �Ѿ�����.....
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

        /*******************************************
         *   �� ���� ���� ��ġ�� ���Ѵ�....
         * ***/
        int     boneIndex = -1;
        Vector3 forward   = player.transform.forward;
        for(int i=PulledTarget.BoneCount-2; i>=0; i--)
        {
            Vector3 currPos  = PulledTarget.GetBonePosition(i);
            Vector3 currDir  = (currPos - _bip01Pelvis.position).normalized;
            
            /**�ڿ� �ִٸ� �����Ѵ�...*/
            if(Vector3.Dot(forward, currDir)<0) 
                continue;

            boneIndex = i;
            break;
        }

        /**�������� ��� �⺻���� ����Ѵ�...*/
        if (boneIndex < 0) boneIndex = (PulledTarget.BoneCount - 2);

        float boneLen   = PulledTarget.GetBoneLength(boneIndex);
        Vector3 bonePos = PulledTarget.GetBonePosition(boneIndex);
        Vector3 boneDir = PulledTarget.GetBoneDir(boneIndex);

        Vector3 right   = Vector3.Cross(boneDir, Vector3.up);
        Vector3 LgrabPos = (bonePos + boneDir * boneLen*.6f);
        Vector3 RgrabPos = (bonePos + boneDir * boneLen * .8f) + (right* .04f);


        /*********************************************
         *   �� ���� ������ ��ġ�� ȸ����Ų��...
         * ******/
        Vector3 LArmDir    = (_LForearm.position - _LArm.position).normalized;
        Vector3 LArmTarget = (LgrabPos - _LArm.position).normalized;

        Vector3 RArmDir    = (_RForearm.position - _RArm.position).normalized;
        Vector3 RArmTarget = (RgrabPos - _RArm.position).normalized;

        Quaternion LRotQuat = IpariUtility.GetQuatBetweenVector(LArmDir, LArmTarget);
        Quaternion RRotQuat = IpariUtility.GetQuatBetweenVector(RArmDir, RArmTarget);

        /**��������*/
        //Debug.DrawLine(_LArm.position, _LArm.position + LArmTarget * 10f, Color.red);
        //Debug.DrawLine(_RArm.position, _RArm.position + RArmTarget * 10f, Color.red);
        _LArm.rotation = (LRotQuat * _LArm.rotation);
        _RArm.rotation = (RRotQuat * _RArm.rotation);
        #endregion
    }
}
