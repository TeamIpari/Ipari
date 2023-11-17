using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using IPariUtility;

/**********************************************************
 *    고래신의 뿔과 상호작용하는 기능이 구현된 컴포넌트입니다...
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

                /**뿔을 얻었을 때의 페이드가 완료되었을 경우....*/
                case (11):
                {
                    #region Omit
                    if (TalkableWhale == null || isDark == false) return;
                    Player.Instance.movementSM.ChangeState(Player.Instance.idle);

                    /**화면에서 보이지 않도록 한다....*/
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

                /**가자 엔딩으로!!!!*/
                case (12):
                {
                    UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>().FadeOut(FadeType.LetterBox);
                    SceneLoader.GetInstance().LoadScene("EndCredits");
                    IpariUtility.OnFadeChange -= FadeComplete;
                    break;
                }

                /**고래신에게 뿔이 박힌 후 페이드가 완료되었을 경우...*/
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
         *   뿔을 분리시키고, 물리 작용이 가능하도록 한다....
         * *****/
        transform.parent  = null;
        _body.isKinematic = false;
        _body.AddTorque(Vector3.left * 50f);
        _body.AddForce((Vector3.left * 90f)+(Vector3.forward*30f));

        float time = 3f;
        while ((time -= Time.deltaTime)>0f) yield return null;



        /************************************************
         *    뿔이 공중으로 떠오른다.....
         * *******/
        #region Fly_Horn

        if (InteractableGoalPos == null) yield break;
        _state = HornState.Fly;

        /**날아가는데 필요한 계산을 한다...*/
        Collider collider = transform.GetComponent<Collider>(); 
        Vector3 goalPos   = InteractableGoalPos.position;

        /**지정한 방향으로 날아가도록 한다....*/
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
            

            /**회전력이 약해지면 회전을 가한다....*/
            if((rotDelay-=deltaTime)<=0f){

                _body.AddTorque(dir * 50f, ForceMode.Acceleration);
                rotDelay = 13f;
            }

            /**샤인 이펙트의 위치를 갱신한다...*/
            if (_state==HornState.Fly_Idle && _ShineSFXIns!=null){

                _ShineSFXIns.position = collider.bounds.center;
            }

            /**상호작용을 가능하게 하고, 샤인 이펙트를 생성한다....*/
            else if(_state==HornState.Fly && dst.sqrMagnitude<=1f && ShineSFXPrefab){

                _state              = HornState.Fly_Idle;
                _ShineSFXIns = Instantiate(ShineSFXPrefab).transform;
                _ShineSFXIns.position = collider.bounds.center;

                /**상호작용을 가능하도록 한다...*/
                InteractActionDispatcher dispatcher = InteractableGoalPos.GetComponent<InteractActionDispatcher>();
                if(dispatcher!=null)
                {
                    dispatcher.gameObject.SetActive(true);
                    dispatcher.IsInteractable = true;
                    dispatcher.OnInteract.AddListener(() =>
                    {
                        /**상호작용을 불가능하게 하고, 페이드를 적용한다....*/
                        dispatcher.IsInteractable = false;

                        Player.Instance.stiffen.StiffenTime = -1f;
                        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);
                        _state = HornState.Wait_CutScene;
                        dispatcher.OnInteract.RemoveAllListeners();

                        /**페이드를 적용한다....*/
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
         *    컷씬이 시작될 때까지 대기한다....
         * ******/
        _body.useGravity = false;
        while (_state != HornState.Enter_CutScene)
        {
            if(_ShineSFXIns!=null) _ShineSFXIns.position = collider.bounds.center;
            yield return null;
        }


        /*********************************************
         *    대화거리와 충분히 가까워질 때까지 이동한다...
         * ******/

        #region Player_Move
        /**연출을 적용하기 위해 조작방지 및 레터박스를 활성화한다....*/
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
            /**플레이어가 특정 목표지점을 바라보도록 한다....*/
            Vector3 distance = (talkTr.position - playerTr.position);
            Vector3 lookDir  = distance.normalized;
            float   floatdst = distance.magnitude;
            lookDir.y = 0f;
            playerTr.rotation = (IpariUtility.GetQuatBetweenVector(playerTr.forward, lookDir) * playerTr.rotation);

            /**목표지점으로 이동시킨다....*/
            Vector3 ArrivalPoint = (talkTr.position - lookDir * 1.5f);
            con.Move(lookDir * Time.deltaTime * (playerSpd * 0.5f));
            playerAnim.SetFloat("speed", Vector3.Distance(ArrivalPoint, playerTr.position) - 0.2f, playerDmp, Time.deltaTime);

            /**적절한 위치에 진입하면 이동을 마무리짓는다...*/
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
         *    고래신이 나오는 연출을 적용한다...
         * ****/

        #region Appear_Whale
        time = .7f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**고래신이 나오기전에 잠깐의 진동이 발생한다....*/
        CameraManager cm = CameraManager.GetInstance();   
        cm.MainCamBrain.ActiveVirtualCamera.Follow = null;
        cm.CameraShake(.15f, CameraManager.ShakeDir.ROTATE, 8f, .032f);
        IpariUtility.PlayGamePadVibration(.01f, .01f, 8f);

        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**고래신이 등장한다....*/
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

        /**고래 소환 이펙트를 적용한다...*/
        if(WhaleSpawnSFXPrefab)
        {
            GameObject newIns = Instantiate(WhaleSpawnSFXPrefab);
            newIns.transform.position = TalkableWhale.transform.GetChild(0).position;

        }

        /**고래신이 물에서 두둥실 떠다니는 효과를 적용한다....*/
        StartCoroutine(WhaleWaveProgress());
        #endregion



        /******************************************************
         *    고래신과의 첫번째 대화가 마무리될 때까지 대기한다...
         * *****/

        #region Talk_Whale
        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**고래신과 대화를 시전한다.....*/
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

        /**고래신과 1차 대화가 마무리될 때까지 대기한다...*/
        Player.Instance.playerInput.enabled = true;
        while (_state != HornState.Give_Horn) yield return null;
        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);

        #endregion


        /*******************************************
         *   뿔이 고래신에게 장☆착 된다!!
         * *******/

        #region Move_Horn2
        /**계산에 필요한 것들을 모조리 구하고, 배지어 곡선을 그린다....*/
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

            /**샤인 이펙트의 위치도 갱신한다...*/
            if (_ShineSFXIns != null)
                _ShineSFXIns.position = collider.bounds.center;

            yield return null;
        }
        while (time > 0f);

        /**화면이 빛난다.....*/
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
         *    마지막 대화를 끝마친 후 페이드를 적용한다.....
         * *****/
        #region Ending_Fade
        time = 1f;
        while ((time -= Time.deltaTime) > 0f) yield return null;

        /**대화 마무리 페이드인아웃....*/
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
