using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using IPariUtility;

/**********************************************************
 *    ������ �԰� ��ȣ�ۿ��ϴ� ����� ������ ������Ʈ�Դϴ�...
 * ******/
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class WhaleHorn : MonoBehaviour
{
    private enum HornState
    {
        None,
        Fall_Down,
        Fly,
        Fly_Idle,
        Wait_CutScene,
        Enter_CutScene,
        Give_Horn,
        Get_Horn,
        GotoEnding
    }

    //========================================================
    //////            Property and fields               //////
    //========================================================
    public Rigidbody Body                { get { return _body; } }

    [SerializeField] public Transform      InteractableGoalPos;
    [SerializeField] public GameObject     ShineSFXPrefab;
    [SerializeField] public GameObject     PutSFXPrefab;
    [SerializeField] public GameObject     WhaleSpawnSFXPrefab;
    [SerializeField] public SaySpeaker     TalkableWhale;
    [SerializeField] private float         MoveDuration = 1f;
    [SerializeField] private float         MoveMaxHeight;
    [SerializeField] public  string        MoveScene = "Credit";


    private Rigidbody _body;
    private Transform _ShineSFXIns;
    private Image     _fadeUI;
    private HornState _state           = HornState.None;
    private float     _currTime        = 0f;
    private float     _moveDurationDiv = 1f;

#if UNITY_EDITOR
    private Collider _collider;
#endif



    //======================================================
    ////////              Magic methods             ////////
    //======================================================
    private void Start()
    {
        #region Omit
        _body = GetComponent<Rigidbody>();

        Transform bossCanvas = GameObject.Find("Boss_Canvas").transform;
        _fadeUI = bossCanvas.GetChild(1).GetComponent<Image>();
        DontDestroyOnLoad(bossCanvas);

        _body.isKinematic = true;
        #endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region 
        if (!collision.gameObject.CompareTag("Platform")) return;

        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Walk, 
            FModLocalParamType.EnvironmentType, 
            FModParamLabel.EnvironmentType.Sand,
            transform.position,
            10f
        );

        #endregion
    }



    //==================================================
    //////             Public methods              //////
    //==================================================
    public void DropItem()
    {
        #region Omit
        StopAllCoroutines();
        StartCoroutine(HornDropProgress());
        #endregion
    }



    //===================================================
    ///////             Core methods               //////
    //===================================================
    private void FadeComplete( bool isDark, int id )
    {
        #region Omit
        switch(id){

                /**���� ����� ���� ���̵尡 �Ϸ�Ǿ��� ���....*/
                case (11):
                {
                    #region Omit
                    if (TalkableWhale == null || isDark == false) return;
                    Player.Instance.movementSM.ChangeState(Player.Instance.idle);

                    /**ȭ�鿡�� ������ �ʵ��� �Ѵ�....*/
                    _body.useGravity = false;
                    transform.position += (Vector3.right * 99999999f);
                    if(_ShineSFXIns!=null) 
                        _ShineSFXIns.position += (Vector3.right*99999999f);

                    GameObject.Find("---Camera").transform.Find("BossRoomCM").gameObject.SetActive(false);
                    GameObject.Find("---Camera").transform.Find("CM vcam1").gameObject.SetActive(true);

                    TalkableWhale.gameObject.SetActive(true);
                    _state = HornState.Enter_CutScene;
                    break;
                    #endregion
                }

                /**���� ��������!!!!*/
                case (12):
                {
                    UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>().FadeOut(FadeType.LetterBox);
                    SceneLoader.GetInstance().LoadScene("EndCredits");
                    IpariUtility.OnFadeChange -= FadeComplete;
                    break;
                }

                /**���ſ��� ���� ���� �� ���̵尡 �Ϸ�Ǿ��� ���...*/
                case (13):
                {
                    _state = HornState.Get_Horn;
                    break;
                }
        }
        #endregion
    }

    private IEnumerator WhaleWaveProgress()
    {
        #region Omit
        float time    = 0f;
        float timeDiv = (1f / 3f);
        float twoPI   = (Mathf.PI * 2f);

        Transform whaleTr = TalkableWhale.transform.GetChild(0);
        Vector3 startPos = whaleTr.position;

        while (true)
        {
            float progressRatio = Mathf.Clamp01((time += Time.deltaTime) * timeDiv);
            float sin = Mathf.Sin(progressRatio * twoPI);
            whaleTr.position = startPos + (Vector3.down * .13f * sin);

            if (progressRatio >= 1f) time = 0f;
            yield return null;
        }
        #endregion
    }

    private IEnumerator HornDropProgress()
    {
        #region Omit

        /****************************************************
         *   ���� �и���Ű��, ���� �ۿ��� �����ϵ��� �Ѵ�....
         * *****/
        transform.parent  = null;
        _body.isKinematic = false;
        _body.AddTorque(Vector3.left * 50f);
        _body.AddForce((Vector3.left * 90f)+(Vector3.forward*30f));

        float time = 3f;
        while ((time -= Time.deltaTime)>0f) yield return null;



        /************************************************
         *    ���� �������� ��������.....
         * *******/
        #region Fly_Horn

        if (InteractableGoalPos == null) yield break;
        _state = HornState.Fly;

        /**���ư��µ� �ʿ��� ����� �Ѵ�...*/
        Collider collider = transform.GetComponent<Collider>(); 
        Vector3 goalPos   = InteractableGoalPos.position;

        /**������ �������� ���ư����� �Ѵ�....*/
        Vector3 velocity = Vector3.zero;
        float   scale    = 0f;
        float  rotDelay  = 1.2f;
        WaitForFixedUpdate waitTime = new WaitForFixedUpdate();

        _body.AddTorque((goalPos - collider.bounds.center).normalized * 200f, ForceMode.Acceleration);
        do 
        {
            #region Fly_Progress
            Vector3 dst     = (goalPos - collider.bounds.center);
            Vector3 dir     = dst.normalized;
            float deltaTime = Time.fixedDeltaTime;

            scale    = Mathf.Clamp(scale += deltaTime*30f, 0, 35f);
            velocity = (deltaTime*scale) * (goalPos - collider.bounds.center);
            _body.velocity = velocity;
            

            /**ȸ������ �������� ȸ���� ���Ѵ�....*/
            if((rotDelay-=deltaTime)<=0f){

                _body.AddTorque(dir * 50f, ForceMode.Acceleration);
                rotDelay = 13f;
            }

            /**���� ����Ʈ�� ��ġ�� �����Ѵ�...*/
            if (_state==HornState.Fly_Idle && _ShineSFXIns!=null){

                _ShineSFXIns.position = collider.bounds.center;
            }

            /**��ȣ�ۿ��� �����ϰ� �ϰ�, ���� ����Ʈ�� �����Ѵ�....*/
            else if(_state==HornState.Fly && dst.sqrMagnitude<=1f && ShineSFXPrefab){

                _state              = HornState.Fly_Idle;
                _ShineSFXIns = Instantiate(ShineSFXPrefab).transform;
                _ShineSFXIns.position = collider.bounds.center;

                /**��ȣ�ۿ��� �����ϵ��� �Ѵ�...*/
                InteractActionDispatcher dispatcher = InteractableGoalPos.GetComponent<InteractActionDispatcher>();
                if(dispatcher!=null)
                {
                    dispatcher.gameObject.SetActive(true);
                    dispatcher.IsInteractable = true;
                    dispatcher.OnInteract.AddListener(() =>
                    {
                        /**��ȣ�ۿ��� �Ұ����ϰ� �ϰ�, ���̵带 �����Ѵ�....*/
                        dispatcher.IsInteractable = false;

                        Player.Instance.stiffen.StiffenTime = -1f;
                        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);
                        _state = HornState.Wait_CutScene;
                        dispatcher.OnInteract.RemoveAllListeners();

                        /**���̵带 �����Ѵ�....*/
                        IpariUtility.ApplyImageFade(
                            IpariUtility.FadeOutType.WHITE_TO_DARK_TO_WHITE,
                            _fadeUI,
                            3f,
                            .6f,
                            11
                        );

                        IpariUtility.OnFadeChange += FadeComplete;
                        TalkableWhale.OnTalkComplete = null;
                    });
                }
            }

            yield return waitTime;
            #endregion
        }
        while (_state!=HornState.Wait_CutScene);

        #endregion



        /*********************************************
         *    �ƾ��� ���۵� ������ ����Ѵ�....
         * ******/
        _body.useGravity = false;
        while (_state != HornState.Enter_CutScene)
        {
            if(_ShineSFXIns!=null) _ShineSFXIns.position = collider.bounds.center;
            yield return null;
        }


        /*********************************************
         *    ��ȭ�Ÿ��� ����� ������� ������ �̵��Ѵ�...
         * ******/

        #region Player_Move
        /**������ �����ϱ� ���� ���۹��� �� ���͹ڽ��� Ȱ��ȭ�Ѵ�....*/
        try { UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>().FadeIn(FadeType.LetterBox); } catch { }
        Player.Instance.playerInput.enabled = false;

        Transform talkTr     = TalkableWhale.transform;
        Transform playerTr   = Player.Instance.transform;
        Animator  playerAnim = Player.Instance.animator; 
        float     playerSpd  = Player.Instance.playerSpeed;
        float     playerDmp  = Player.Instance.speedDampTime;
        CharacterController con = Player.Instance.controller;

        do
        {
            #region Move_Progress
            /**�÷��̾ Ư�� ��ǥ������ �ٶ󺸵��� �Ѵ�....*/
            Vector3 distance = (talkTr.position - playerTr.position);
            Vector3 lookDir  = distance.normalized;
            float   floatdst = distance.magnitude;
            lookDir.y = 0f;
            playerTr.rotation = (IpariUtility.GetQuatBetweenVector(playerTr.forward, lookDir) * playerTr.rotation);

            /**��ǥ�������� �̵���Ų��....*/
            Vector3 ArrivalPoint = (talkTr.position - lookDir * 1.5f);
            con.Move(lookDir * Time.deltaTime * (playerSpd * 0.5f));
            playerAnim.SetFloat("speed", Vector3.Distance(ArrivalPoint, playerTr.position) - 0.2f, playerDmp, Time.deltaTime);

            /**������ ��ġ�� �����ϸ� �̵��� ���������´�...*/
            if (floatdst < 13.5f){

                playerAnim.SetFloat("speed", 0f, playerDmp, Time.deltaTime);
                break;
            }

            yield return null;
            #endregion
        }
        while (true);

        GameObject.Find("---Camera").transform.Find("CM vcam1").gameObject.SetActive(false);
        #endregion



        /*******************************************
         *    ������ ������ ������ �����Ѵ�...
         * ****/

        #region Appear_Whale
        time = .7f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**������ ���������� ����� ������ �߻��Ѵ�....*/
        CameraManager cm = CameraManager.GetInstance();   
        cm.MainCamBrain.ActiveVirtualCamera.Follow = null;
        cm.CameraShake(.15f, CameraManager.ShakeDir.ROTATE, 8f, .032f);
        IpariUtility.PlayGamePadVibration(.01f, .01f, 8f);

        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**������ �����Ѵ�....*/
        Transform whaleTr  = TalkableWhale.transform.GetChild(0);
        Vector3  startPos  = whaleTr.position; 
        whaleTr.gameObject.SetActive(true);

        time          = 5f;
        float timeDiv = (1f / time);

        do
        {
            float progressRatio = Mathf.Clamp01((time -= Time.deltaTime)*timeDiv);
            whaleTr.position = startPos + (Vector3.down * 10f * progressRatio);
            yield return null;
        }
        while (time>0f);

        /**�� ��ȯ ����Ʈ�� �����Ѵ�...*/
        if(WhaleSpawnSFXPrefab)
        {
            GameObject newIns = Instantiate(WhaleSpawnSFXPrefab);
            newIns.transform.position = TalkableWhale.transform.GetChild(0).position;

        }

        /**������ ������ �εս� ���ٴϴ� ȿ���� �����Ѵ�....*/
        StartCoroutine(WhaleWaveProgress());
        #endregion



        /******************************************************
         *    ���Ű��� ù��° ��ȭ�� �������� ������ ����Ѵ�...
         * *****/

        #region Talk_Whale
        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**���Ű� ��ȭ�� �����Ѵ�.....*/
        TalkableWhale.UseLetterBox = false;
        TalkableWhale.SayType      = 5;
        TalkableWhale.PlaySay();
        TalkableWhale.OnTalkComplete += (SaySpeaker speaker) =>
        {
            if (speaker.SayType == 5)
            {
                _state  = HornState.Give_Horn;
                TalkableWhale.OnTalkComplete = null;
            }
        };

        /**���Ű� 1�� ��ȭ�� �������� ������ ����Ѵ�...*/
        Player.Instance.playerInput.enabled = true;
        while (_state != HornState.Give_Horn) yield return null;
        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);

        #endregion


        /*******************************************
         *   ���� ���ſ��� ����� �ȴ�!!
         * *******/

        #region Move_Horn2
        /**��꿡 �ʿ��� �͵��� ������ ���ϰ�, ������ ��� �׸���....*/
        Transform whaleHornTr    = TalkableWhale.transform.GetChild(0).Find("Jewelry");
        Transform whaleHornDirTr = whaleHornTr.GetChild(0);
        Transform hornDirTr      = transform.GetChild(0);

        Vector3 whaleHornDir = (whaleHornDirTr.position - whaleHornTr.position).normalized;
        Vector3 hornDir      = (hornDirTr.position - transform.position).normalized;
        Vector3 hornGoalPos  = whaleHornTr.position;
        Vector3 hornStartPos = playerTr.position + (playerTr.forward * 1.5f);

        Vector3 hornS2G       = (hornGoalPos - hornStartPos);
        Vector3 hornS2GDir    = hornS2G.normalized;
        Vector3 right         = Vector3.Cross(Vector3.up, hornS2GDir);
        Vector3 up            = Vector3.Cross(right, hornS2GDir);
        Vector3 hornCenterPos = hornStartPos + (hornS2G*.5f) + (up*-8f);
        Vector3 startScale    = (transform.localScale*1.3f);
        Quaternion startQuat  = transform.rotation;
        _body.isKinematic  = true;

        time    = 1.6f;
        timeDiv = (1f / time);

        float timeDiv2 = (1f / .2f);

        ParticleSystem system = _ShineSFXIns.GetComponent<ParticleSystem>();
        system.Stop(true);
        system.Play(true);
        transform.localScale = (transform.localScale * 1.5f);
        transform.rotation   = whaleHornTr.rotation;

        do
        {
            time -= Time.deltaTime;
            float progressRatio  = (1f - Mathf.Clamp01(time * timeDiv));
            float progressRatio2 = Mathf.Clamp01(progressRatio*timeDiv2);

            Vector3 pos = IpariUtility.GetBezier(  ref hornStartPos,
                                                    ref hornCenterPos,
                                                    ref hornGoalPos,
                                                    progressRatio);

            transform.position   = pos;
            transform.localScale = ( startScale * progressRatio2 );

            /**���� ����Ʈ�� ��ġ�� �����Ѵ�...*/
            if (_ShineSFXIns != null)
                _ShineSFXIns.position = collider.bounds.center;

            yield return null;
        }
        while (time > 0f);

        /**ȭ���� ������.....*/
        if(PutSFXPrefab!=null)
        {
            GameObject newIns = Instantiate(PutSFXPrefab);
            newIns.transform.position = transform.position;
            Destroy(newIns, 1.5f);

        }

        transform.position += (Vector3.up * 99999f);
        if (_ShineSFXIns != null) Destroy(_ShineSFXIns.gameObject);
        whaleHornTr.gameObject.SetActive(true);

        #endregion


        /**************************************************
         *    ������ ��ȭ�� ����ģ �� ���̵带 �����Ѵ�.....
         * *****/
        #region Ending_Fade
        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**��ȭ ������ ���̵��ξƿ�....*/
        IpariUtility.ApplyImageFade(

            IpariUtility.FadeOutType.WHITE_TO_DARK_TO_WHITE,
            _fadeUI,
            4f,
            1.5f,
            12
        );

        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);
        #endregion

        #endregion
    }

}
