using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************
 *   �𷡰� ħ�ĵǴ� ȿ���� �����ϴ� Ŭ�������� ��� Ŭ�����Դϴ�..
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
     *   ������ ���� ĳ�� ���� �ʵ��...
     * ***/
    private Collider   _collider;
    protected Material _SandMat;
    private FModEventInstance sfxIns;


    /*****************************************
     *   ħ�� ������ ���õ� �ʵ��...
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
         *   ���� �İ���� ȿ�� ����.
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
         *   �� ħ�� uv �ִϸ��̼��� �ӵ���, ��鸲 ȿ���� �����Ѵ�.....
         * *****/
        if(_SandMat) _SandMat.SetFloat("_Speed", _totalTime += (deltaTime * _currPullSpeed * IntakeMatSpeedScale));

        if(_currPullSpeed>0f)
        {
            /**����ɴ� ȿ���� �ߴ��Ѵ�....*/
            if(_currStopTime>0f && (_currStopTime-=deltaTime)<=0f)
            {
                IpariUtility.StopGamePadVibration(10);
                IntakeSand(false);
            }
        }



        /***************************************************
         *   �÷��̾ �𷡿� �������� ���� ó���� �Ѵ�...
         * ********/
        if (PlayerOnSand == false) return;
        Player player = Player.Instance;

        /**�÷��̾ �������� ���...*/
        if (PlayerIsDead){

            if (player.isDead == false) PlayerIsDead = false;
            player.transform.position += (Vector3.down * .025f);
            return;
        }

        /**�÷��̾ �𷡿� ��� �ִ� ���ȿ� ���� ó���� �Ѵ�....*/
        RaycastHit hit;
        if (!(IpariUtility.GetPlayerFloorinfo(out hit, 1<<gameObject.layer)))
        {
            PlayerOnSand = false;
            return;
        }

        bool isGround       = (hit.normal.y > 0);
        bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

        /**���� ��� �ִ� ������ ó��...*/
        if (isGround && isSameCollider)
        {
            if (_currPullSpeed <= 0f) return;

            /**�𷡰� ����ɴ� ������ ó��...*/
            Vector3 playerPos       = player.transform.position;
            Vector3 centerPos       = GetWorldCenterPosition(_currCenterOffset);
            Vector3 target2Center   = (centerPos - playerPos).normalized;
            Vector3 right           = Vector3.Cross(target2Center, hit.normal);
            Vector3 forward         = -Vector3.Cross(right, hit.normal);

            float pullPow = (IntakePower * _currPullSpeed);
            player.controller.Move((forward * pullPow * deltaTime));


            /**�÷��̾ �� ����� ���Դٸ� ��������...*/
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

        /**�÷��̾ ���� ���� �ʾ��� ����� ó��...*/
        PlayerOnSand = false;
        PlayerIsDead = false;

        #endregion
    }

    private void OnCollisionStay(Collision collision)
    {
        #region Omit
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Boss") || _currPullSpeed <= 0f) return;

        /****************************************
         *   �浹�� ��ü�� �𷡸� ���� ������ üũ�Ѵ�...
         * ****/
        int  Count         = collision.contactCount;
        bool isGround      = false;
        ContactPoint point = new ContactPoint();

        for (int i = 0; i < Count; i++)
        {
            point = collision.GetContact(i);

            /**���� ������� Ȯ���ϰ� �񱳸� �ߴ��Ѵ�..*/
            if (point.normal.y < 0){

                isGround = true;
                break;
            }
        }


        /*****************************************
         *   �𷡸� ��� �ִ� ������ ó��...
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

            /**���� �߽����� �������� ���*/
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
         *   �� ħ�� ���带 �����Ų��.....
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
         *   �÷��̾ ���� ����� ����� ó��..
         * ***/
        if (PlayerOnSand == false)
        {
            RaycastHit hit;
            if (IpariUtility.GetPlayerFloorinfo(out hit, 1<<gameObject.layer)){

                bool isGround       = (hit.normal.y > 0);
                bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

                /**���� ���� ������ �����Ѵ�...*/
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
