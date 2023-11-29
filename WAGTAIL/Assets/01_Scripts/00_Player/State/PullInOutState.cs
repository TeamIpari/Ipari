using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

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
    private GameObject     _GrabPos;
    private PullableObject _PulledTarget;
    private Transform      _LArm, _RArm;
    private Transform      _LForearm, _RForearm;
    private Transform      _LHand, _RHand;
    private float          _arm2ForearmLen, _forearm2HandLen;

    private bool _padIsVibration = false;



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
        #region  Omit
        base.Enter();
        
        /***********************************************************
         *   ���� �÷��̾ ��ȣ�ۿ��� ��ü�� ������ ���¸� �����Ѵ�...
         ********/
        GameObject target;
        if ((target = player.currentInteractable)==null) return;
        if ((PulledTarget = target.GetComponent<PullableObject>()) == null) return;
        
        /**���⸦ �����Ѵ�....*/
        if(_progressCoroutine!=null) player.StopCoroutine(_progressCoroutine);
        _progressCoroutine = player.StartCoroutine(PullingProgress());
        
        player.isPull = true;
        #endregion
    }

    public override void Exit()
    {
        #region Omit
        base.Exit();
        player.isPull = false;
        player.currentInteractable = null;

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
    
    public override void LogicUpdate()
    {
        #region Omit
        base.LogicUpdate();
        
        if (player.isDead && player.movementSM.currentState != player.death)
        {
            stateMachine.ChangeState(player.death);
            return;
        }
        #endregion
    }

    

    //=================================================
    ////////            Core methods            //////
    //=================================================
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

        float movDistance   = (grabPoint - playerPos).magnitude * 1.2f;
        float progressRatio = 0f;
        float currTime      = 0f;
        float goalTimeDiv   = (1f / _LookAtTime);

        goalDir.y = 0f;
        movDir.y = 0f;


        /*********************************************
         *   ����� �ٶ󺻴�...
         * ***/
        animator.CrossFade(PullAnimation.HOLD, .2f, 0);

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
        PulledTarget.HoldingPoint = _GrabPos;

        float fullLen       = PulledTarget.MaxLength;
        float fullLenDiv    = (1f / fullLen);
        float limitLenDiv   = (1f / PulledTarget.MaxScale);
        bool isMove         = false;

        animator.CrossFade(PullAnimation.IDLE, .1f);
        _applyIK            = true;

        while (PulledTarget.IsBroken==false)
        {
            /******************************************
             *   ���� ���ۿ� �ʿ��� ������ ��� ���Ѵ�...
             * ***/
            Vector3 playerDir    = playerTr.forward;
            Vector3 lookDir      = (rootPoint - playerTr.position).normalized;
            Vector3 moveDir      = new Vector3(input.x, 0f, input.y).normalized;
            bool isPulling       = Vector3.Dot(moveDir, lookDir) <= 0;
            float normalizedLen  = PulledTarget.NormalizedLength;
            float nearLimitRatio = (normalizedLen * limitLenDiv);
            float deltaTime      = Time.deltaTime * 2f;
            float speed          = fullLen;


            /**********************************************
             *    Ű�Է��� ���� ���� �ִٸ�, ���׷¿� ���� 
             *    �ӵ����� �� �е������� �����Ѵ�...
             * *****/
            if (isPulling && moveDir.sqrMagnitude>0f && normalizedLen >= .95f)
            {
                speed -= (fullLen * Mathf.Clamp(nearLimitRatio, 0f, .9f));

                /**�����е带 ������Ų��...*/
                if(!_padIsVibration && normalizedLen>=1f){

                    _padIsVibration = true;
                    Gamepad.current?.SetMotorSpeeds(0.0123f, 0.0134f);
                }
            }

            /**������ ������ ���´�...*/
            else if (_padIsVibration){

                _padIsVibration = false;
                Gamepad.current?.SetMotorSpeeds(0f, 0f);
            }
            speed = Mathf.Clamp(speed, 0f, 4.1f);


            /****************************************************
             *   �÷��̾ �ٶ� ������ ���ϴ� ���ʹϾ��� ���Ѵ�...
             * ***/
            lookDir.y = 0f;
            Quaternion rotQuat  = IpariUtility.GetQuatBetweenVector(playerDir, lookDir);

            

            /**********************************************
             *   ��ȣ�ۿ�Ű�� �ԷµǸ� ���⸦ �׸��д�...
             * ****/
            if(interactAction.triggered)
            {
                GamePadShake(.1f, .1f, .1f);

                _applyIK                  = false;
                player.isPull             = false;
                PulledTarget.HoldingPoint = null;

                animator.SetFloat("speed", 0f);
                animator.CrossFade(PullAnimation.PLAYER_IDLE, .3f);
                player.movementSM.ChangeState(player.idle);
                yield break;
            }

            
            /*********************************************
             *   ����� �̵��� �� �ִϸ��̼��� ���������Ѵ�...
             * ***/
            playerCon.SimpleMove(moveDir * speed * 2f);
            playerTr.rotation = (rotQuat * playerTr.rotation);

            SetMoveAnim(ref isMove, input.sqrMagnitude);
            animator.SetFloat("speed", speed * fullLenDiv);

            yield return null;
        }


        /**********************************************
         *   ���� �������� �÷��̾ �Ѿ�����.....
         * ****/
        GamePadShake(1f, 1f, .1f);

        _applyIK = false;
        animator.SetFloat("speed", 0f);
        animator.Play(PullAnimation.PUT);

        float leftTime = 2.4f;
        while((leftTime-=Time.deltaTime)>0) yield return null;

        player.isPull = false;
        player.movementSM.ChangeState(player.idle);
        #endregion
    }

    private void GamePadShake(float leftMoter, float rightMoter, float time)
    {
        #region Omit
        Gamepad current;
        if((current= Gamepad.current)!=null)
        {
            current.SetMotorSpeeds(leftMoter, rightMoter);
            player.StartCoroutine(GamePadShakeProgress(leftMoter, rightMoter, time));
        }
        #endregion
    }

    private IEnumerator GamePadShakeProgress(float leftMoter, float rightMoter, float time)
    {
        #region Omit

        while((time-=Time.deltaTime)>0f)
        {
            yield return null;
        }

        Gamepad.current.SetMotorSpeeds(0f, 0f);
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
         *   ��Ե� ������, ���� ���� ������ ���Ѵ�....
         * ******/

        /**���� ��Ե� ��ġ�� ���´�...*/
        Vector3 grabPos, grabDir;
        Transform playerTr = player.transform;

        PulledTarget.GetNearBonePositionAndDir(
            playerTr.position + (playerTr.forward*.1f), 
            out grabPos, 
            out grabDir
        );

        /**����� ���ϰԵ� �������� ���Ѵ�....*/
        Vector3 grabRight = -Vector3.Cross(Vector3.up, grabDir);
        grabPos += (Vector3.down * .1f) + (grabRight*.03f);

        /******************************************************
         *   �� ������ ��ġ�ؾ��� ��ġ�� ȸ������ ������ ���Ѵ�...
         * ******/

        /**Hands...*/
        Vector3 LHandGoalPos = grabPos;
        Vector3 LHandDir = _LHand.transform.up;
        Vector3 RHandGoalPos = grabPos + (grabRight * .1f);
        Vector3 RHandDir = _RHand.transform.up;

        /**ForeArms....*/
        Vector3 LForeArmDir = (_LHand.position - _LForearm.position).normalized;
        Vector3 LForeArmGoalDir = (LHandGoalPos - _LForearm.position).normalized;
        Vector3 LForeArmPos = LHandGoalPos + (_LHand.right * _forearm2HandLen);
        Vector3 RForeArmDir = (_RHand.position - _RForearm.position).normalized;
        Vector3 RForeArmGoalDir = (RHandGoalPos - _RForearm.position).normalized;
        Vector3 RForeArmPos = RHandGoalPos + (_RHand.right * _forearm2HandLen);

        /**Arms...*/
        Vector3 LArmDir = (_LForearm.position - _LArm.position).normalized;
        Vector3 LArmGoalDir = (LHandGoalPos - _LArm.position).normalized;
        Vector3 LArmPos = LForeArmPos - (LArmGoalDir * _arm2ForearmLen);
        Vector3 RArmDir = (_RForearm.position - _RArm.position).normalized;
        Vector3 RArmGoalDir = (RHandGoalPos - _RArm.position).normalized;
        Vector3 RArmPos = RForeArmPos - (RArmGoalDir * _arm2ForearmLen);


        /************************************************
         *    �� ���鿡�� ���� ������ �Ѵ�.....
         * ****/
        Debug.DrawLine(grabPos, grabPos + (Vector3.up * 10f), Color.red);

        _LArm.rotation = (IpariUtility.GetQuatBetweenVector(LArmDir, LArmGoalDir) * _LArm.rotation);
        _RArm.rotation = (IpariUtility.GetQuatBetweenVector(RArmDir, RArmGoalDir) * _RArm.rotation);

        _LHand.rotation = (IpariUtility.GetQuatBetweenVector(LHandDir, grabRight) * _LHand.transform.rotation);
        _RHand.rotation = (IpariUtility.GetQuatBetweenVector(RHandDir, -grabRight) * _RHand.transform.rotation);

        #endregion
    }
}
