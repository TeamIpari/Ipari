using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
 *   모래가 침식되는 효과를 구현하는 클래스들의 기반 클래스입니다..
 * ***/
[RequireComponent(typeof(Collider))]
public abstract class SandScriptBase : MonoBehaviour, IEnviroment
{
    //=======================================
    /////          Property             /////
    //=======================================
    public string EnviromentPrompt  { get; set; } = string.Empty;
    public bool   PlayerOnSand      { get { return _PlayerOnSand; } private set { _PlayerOnSand = value; } }
    public bool   PlayerIsDead      { get; private set; } = false;
    public bool   IsHit             { get; set; } = false;
    public bool   IsIntake          { get; private set; } = false;
    public float  IntakeDuration{

        get { return _intakeDuration; }
        set
        {
            _intakeDuration = (value <= 0 ? 0f : value);
            _intakeDiv      = (1f / _intakeDuration);
        }
    }

    [SerializeField] 
    private bool   _PlayerOnSand = false;

    [SerializeField] 
    public bool    IntakeOnAwake = false;

    [SerializeField, Min(0f)] 
    public float   IntakePower            = 6f;

    [SerializeField, Min(0f)]
    public float   IntakeMatSpeedScale   = 1f;

    [SerializeField, Min(0f)]
    public float   IntakeStopDuration     = -1f;

    [SerializeField] 
    public Vector3 SandIdleCenterOffset   = Vector3.zero;

    [SerializeField] 
    public Vector3 SandIntakeCenterOffset = Vector3.zero;

    [SerializeField]
    public GameObject IntakeObjectFX;



    //=========================================
    /////             Fields               ////
    //=========================================

    /*****************************************
     *   참조에 대한 캐싱 관련 필드들...
     * ***/
    private Collider   _collider;
    protected Material _SandMat;
    private FModEventInstance sfxIns;


    /*****************************************
     *   침식 로직과 관련된 필드들...
     * ***/
    private float   _currStopTime       = 0f;
    private float   _intakeDuration     = 1f;
    private float   _intakeDiv          = 0f;
    private float   _currIntakeTime     = 0f;
    private Vector3 _currCenterOffset   = Vector3.zero;
    private Vector3 _startCenterOffset  = Vector3.zero;
    private Vector3 _goalCenterOffset   = Vector3.zero;

    private float _startPullSpeed = 0f;
    private float _currPullSpeed = 0f;
    private float _goalPullSpeed = 0f;
    private float _totalTime = 0f;
    private float _shakeTime = 0f;



    //===============================================
    //////          Magic methods               /////
    //===============================================
    private void OnDrawGizmosSelected()
    {
        #region Omit
        OnDrawSandGizmos();

        Vector3 pos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos + SandIdleCenterOffset, new Vector3(.2f, 10f, .2f));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos + SandIntakeCenterOffset, new Vector3(.2f, 10f, .2f));
        #endregion
    }

    private void Awake()
    {
        OnSandAwake();
    }

    private void Start()
    {
        #region Omit
        OnSandStart();

        _currCenterOffset   = SandIdleCenterOffset;
        gameObject.layer    = LayerMask.NameToLayer("Platform");
        gameObject.tag      = "Platform";

        _collider = GetComponent<Collider>();
        _SandMat?.SetFloat("_Speed", 0f);

        if (IntakeOnAwake) IntakeSand(true);
        #endregion
    }

    private void Update()
    {
        #region Omit
        /********************************************
         *   점점 파고들어가는 효과 적용.
         * ***/
        float deltaTime = Time.deltaTime;
        if (_currIntakeTime > 0f)
        {
            _currIntakeTime -= deltaTime;
            float progressRatio = (1f - Mathf.Clamp(_currIntakeTime * _intakeDiv, 0f, 1f));

            _currCenterOffset = _startCenterOffset + (_goalCenterOffset - _startCenterOffset) * progressRatio;
            _currPullSpeed = _startPullSpeed + (_goalPullSpeed - _startPullSpeed) * progressRatio;
            UpdateSandMesh( _currCenterOffset );
        }


        /***************************************************************
         *   모래 침식 uv 애니메이션의 속도와, 흔들림 효과를 적용한다.....
         * *****/
        if(_SandMat) _SandMat.SetFloat("_Speed", _totalTime += (deltaTime * _currPullSpeed * IntakeMatSpeedScale));

        if(_currPullSpeed>0f)
        {
            /**가라앉는 효과를 중단한다....*/
            if(_currStopTime>0f && (_currStopTime-=deltaTime)<=0f)
            {
                IpariUtility.StopGamePadVibration(10);
                IntakeSand(false);
            }
        }



        /***************************************************
         *   플레이어가 모래에 빨려들어갔을 때의 처리를 한다...
         * ********/
        if (PlayerOnSand == false) return;
        Player player = Player.Instance;

        /**플레이어가 빨려들어갔을 경우...*/
        if (PlayerIsDead){

            if (player.isDead == false) PlayerIsDead = false;
            player.transform.position += (Vector3.down * .025f);
            return;
        }

        /**플레이어가 모래에 밟고 있는 동안에 대한 처리를 한다....*/
        RaycastHit hit;
        if (!(IpariUtility.GetPlayerFloorinfo(out hit, 1<<gameObject.layer)))
        {
            PlayerOnSand = false;
            return;
        }

        bool isGround       = (hit.normal.y > 0);
        bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

        /**땅을 밟고 있는 동안의 처리...*/
        if (isGround && isSameCollider)
        {
            if (_currPullSpeed <= 0f) return;

            /**모래가 가라앉는 동안의 처리...*/
            Vector3 playerPos       = player.transform.position;
            Vector3 centerPos       = GetWorldCenterPosition(_currCenterOffset);
            Vector3 target2Center   = (centerPos - playerPos).normalized;
            Vector3 right           = Vector3.Cross(target2Center, hit.normal);
            Vector3 forward         = -Vector3.Cross(right, hit.normal);

            float pullPow = (IntakePower * _currPullSpeed);
            player.controller.Move((forward * pullPow * deltaTime));


            /**플레이어가 모래 가운데로 들어왔다면 죽음판정...*/
            float target2CenterLen = (centerPos - playerPos).magnitude;
            if (!player.isDead && target2CenterLen < 1f && _currPullSpeed>=.8f)
            {
                if (IntakeObjectFX != null)
                {
                    GameObject newFX = GameObject.Instantiate(IntakeObjectFX);
                    newFX.transform.position = centerPos + (Vector3.down * .5f);
                    newFX.transform.localScale = (Vector3.one * 1f);
                    newFX.SetActive(true);
                    Destroy(newFX, 1f);
                }

                player.controller.enabled = true;
                PlayerIsDead = true;
                player.isDead = true;
            }
            return;
        }

        /**플레이어가 땅을 밟지 않았을 경우의 처리...*/
        PlayerOnSand = false;
        PlayerIsDead = false;

        #endregion
    }

    private void OnCollisionStay(Collision collision)
    {
        #region Omit
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Boss") || _currPullSpeed <= 0f) return;

        /****************************************
         *   충돌한 객체가 모래를 밟은 것인지 체크한다...
         * ****/
        int  Count         = collision.contactCount;
        bool isGround      = false;
        ContactPoint point = new ContactPoint();

        for (int i = 0; i < Count; i++)
        {
            point = collision.GetContact(i);

            /**땅을 밟았음을 확인하고 비교를 중단한다..*/
            if (point.normal.y < 0){

                isGround = true;
                break;
            }
        }


        /*****************************************
         *   모래를 밟고 있는 동안의 처리...
         * ***/
        if (isGround == false || _currPullSpeed <= 0f) return;

        Rigidbody body          = collision.rigidbody;
        Vector3 centerPos       = GetWorldCenterPosition(_currCenterOffset);
        Vector3 target2Center   = (centerPos - body.position).normalized;
        Vector3 right           = Vector3.Cross(point.normal, target2Center);
        Vector3 forward         = Vector3.Cross(right, point.normal);

        float target2CenterLen = (centerPos - body.position).magnitude;

        if (body != null){

            float pullPow = (IntakePower * _currPullSpeed);
            body.MovePosition(body.position + (forward * pullPow * Time.fixedDeltaTime));

            /**모래의 중심으로 빨려들어갔을 경우*/
            if (target2CenterLen < 1f)
            {
                if(IntakeObjectFX!=null)
                {
                    GameObject newFX = GameObject.Instantiate(IntakeObjectFX);
                    newFX.transform.position   = centerPos + (Vector3.down*.5f);
                    newFX.transform.localScale = (Vector3.one * 1f);
                    newFX.SetActive(true);  
                    Destroy(newFX, 1f);
                }

                Physics.IgnoreCollision(collision.collider, _collider);
                body.velocity = (Vector3.down*10f*body.mass);
                GameObject.Destroy(body.gameObject, 2f);
            }
        }

        #endregion
    }

    private void OnDestroy()
    {
        if(sfxIns.IsValid) sfxIns.Destroy();
    }




    //===============================================
    //////          Virtual methods             /////
    //===============================================
    protected virtual void OnSandAwake() { }
    protected virtual void OnSandStart() { }
    protected virtual void OnDrawSandGizmos() { }
    protected virtual float SampleHeight( Vector3 worldPosition ){ return 0f; }
    protected abstract void UpdateSandMesh( Vector3 currCenterOffset);
    protected virtual Vector3 GetWorldCenterPosition(Vector3 currCenter) { return (currCenter + transform.position); }



    //=============================================
    /////           Core methods              ////
    //=============================================
    public void IntakeSand(bool apply)
    {
        #region Omit
        IsIntake            = apply;
        _goalCenterOffset   = (apply ? SandIntakeCenterOffset : SandIdleCenterOffset);
        _goalPullSpeed      = (apply ? 1f : 0f);

        _currStopTime       = IntakeStopDuration;
        _currIntakeTime     = _intakeDuration;
        _intakeDiv          = (1f / _intakeDuration);
        _startCenterOffset  = _currCenterOffset;
        _startPullSpeed     = _currPullSpeed;

        /****************************************************
         *   모래 침식 사운드를 재생시킨다.....
         * ****/
        if (apply)
        {
            sfxIns = FModAudioManager.CreateInstance(FModSFXEventType.SandFall3D);
            sfxIns.Volume = 0f;
            sfxIns.Position = (transform.position + SandIdleCenterOffset);
            sfxIns.Set3DDistance(10f, 30f);
            sfxIns.Play();

            FModAudioManager.ApplyInstanceFade(sfxIns, 1.4f, 3f);
        }
        else if (sfxIns.IsValid){

            FModAudioManager.ApplyInstanceFade(sfxIns, 0f, 3f, -999, true);
        }
        #endregion
    }

    public void ExecutionFunction(float time)
    {
       //TODO:
    }

    public bool Interact()
    {
        #region Omit

        /**************************************
         *   플레이어가 땅을 밟았을 경우의 처리..
         * ***/
        if (PlayerOnSand == false)
        {
            RaycastHit hit;
            if (IpariUtility.GetPlayerFloorinfo(out hit, 1<<gameObject.layer)){

                bool isGround       = (hit.normal.y > 0);
                bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

                /**땅을 밟음 판정을 적용한다...*/
                if (isGround && isSameCollider)
                {
                    PlayerOnSand = true;
                    //PlayerDead = false;
                    return true;
                }
            }
        }

        return false;
        #endregion
    }
}
