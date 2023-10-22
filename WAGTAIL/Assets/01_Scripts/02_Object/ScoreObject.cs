using System;
using UnityEngine;
using IPariUtility;
using UnityEngine.Serialization;
using UnityEditor.SceneManagement;

/*********************************************************************
 *    ��ȣ�ۿ��, Ư�� ������Ҹ� ī�����ϴ� ����� ������ ������Ʈ�Դϴ�...
 * *****/
public sealed class ScoreObject : MonoBehaviour, IEnviroment
{
    public enum GetKind
    {
        None,
        RaiseUp
    }

    //=================================================
    /////                Property                 /////
    //=================================================
    public Rigidbody Body                 { get { return _body; } }
    public Collider  Collider             { get { return _collider;  }}
    public float     MagnetMoveSpeed      { get { return _MagnetSpeed; } set { _MagnetSpeed = (value < 0f ? 0f : value); } }
    public float     MagnetMoveDelayTime  { get { return _MagnetMoveDelayTime; } set { _MagnetMoveDelayTime = (value < 0f ? 0.01f : value); } }
    public float     GetRaiseUpTime       { get { return _GetRaiseUpTime; } set { _GetRaiseUpTime = (value < 0f ? 0f : value); } }
    public string    EnviromentPrompt     { get; }=String.Empty;
    public bool      IsHit                { get; set; } = false;

    [SerializeField] 
    public bool     UseMagnetMovement = false;

    [SerializeField] 
    public GetKind  ItemGetType       = GetKind.RaiseUp;

    [SerializeField, FormerlySerializedAs("scoreType")]
    public ScoreType ScoreType        = ScoreType.Coin;

    [SerializeField,Min(0f)] 
    private float       _MagnetSpeed          = 1.1f;

    [SerializeField,Min(0f)]
    private float       _MagnetMoveDelayTime  = 0f;

    [SerializeField, Min(0f)]
    private float       _GetRaiseUpTime         = .25f;

    [SerializeField,FormerlySerializedAs("_interactionVFX")] 
    public GameObject   InteractionVFX;

    

    //=================================================
    /////                 Fields                  /////
    //=================================================
    private const float Height    = 1.7f;
    private const float FlightDiv = 4.0f;

    private float     _currTime        = 0f;    //��� �ð�....
    private bool      _isValid         = false;

    private Rigidbody _body;
    private Collider  _collider;
    private Transform _playerTr;



    //==================================================
    //////             Magic methods               /////
    //=================================================
    private void Awake()
    {
        #region Omit
        _body     = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        gameObject.tag = "Platform";

        /**�÷��̾��� Ʈ������ ĳ��...*/
        Player player = Player.Instance;
        if (player != null){
            _playerTr = player.transform;
        }

        _isValid = (_body != null || _collider != null || _playerTr != null);
        #endregion
    }

    private void Update()
    {
        #region Omit

        /**********************************************
         *   �����ð� ��ŭ �����, �ڼ� ȿ���� �����Ѵ�...
         * ***/
        if(_currTime<MagnetMoveDelayTime){

            if ((_currTime+=Time.deltaTime) >= MagnetMoveDelayTime)
            {
                /**�����ð��� ������ ���...*/
                ApplyTimeout();
            }
            else return;
        }

        /**************************************
         *   �ڼ��� ������ ������ �����Ѵ�....
         * ****/
        if (UseMagnetMovement == false || _body==null) return;

        Vector3 center2Player = (_playerTr.position - _collider.bounds.center);
        float   distance      = center2Player.magnitude;
        float  magentDistance = (10 / distance) * 1.25f;
        _body.velocity        = center2Player * magentDistance;

        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GetItem();
    }


    //================================================
    ///////           Core methods              //////
    //================================================
    private void GetItem()
    {
        #region Omit
        GameManager gm = GameManager.GetInstance();
        if (gm == null || _collider==null) return;

        _collider.enabled = false;
        UseMagnetMovement = false;

        switch (ScoreType){

                /**������ ȹ���Ͽ��� ���...*/
                case (ScoreType.Coin):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Bead
                    );

                    gm.Coin++;
                    break;
                }

                /**���ڽø� ȹ���Ͽ��� ���...*/
                case (ScoreType.Cocosi):
                {

                    break;
                }

                /**���� ȹ���Ͽ��� ���....*/
                case (ScoreType.Flower):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Flower
                    );

                    gm.Flower++;
                    break;
                }
        }

        /**������ ȹ��� �������� ȿ���� �����Ѵ�...*/
        if (ItemGetType == GetKind.RaiseUp)
        {
            UseRigidbody();
            _body.useGravity = false;
            _body.velocity   = IpariUtility.CaculateVelocity(
                transform.position + (Vector3.up*Height),
                transform.position,
                _GetRaiseUpTime
            );

            SetTime(_GetRaiseUpTime);
            return;
        }

        /**�������� �ı��Ǹ鼭 ����Ʈ�� �����Ѵ�...*/
        SpawnVFX();
        Destroy(gameObject);

        #endregion
    }

    private void ApplyTimeout()
    {
        #region Omit
        /**********************************************
         *   Ư�� �����ð��� ������, ������ ȿ���� �����Ѵ�...
         * ***/

        /**ȹ��Ǿ��� ���...*/
        if(_collider!=null && _collider.enabled==false){

            SpawnVFX();
            Destroy(gameObject);
            return;
        }

        /**�ڼ�ȿ���� ������ ���...*/
        if(UseMagnetMovement && _body!=null && _collider!=null)
        {
            _body.velocity      = Vector3.zero;
            _body.useGravity    = false;
            _collider.isTrigger = true;
            return;
        }
        #endregion
    }

    private void SpawnVFX()
    {
        #region Omit
        if (InteractionVFX != null)
        {
            GameObject exploVFX = Instantiate(
                InteractionVFX,
                gameObject.transform.position,
                gameObject.transform.rotation
            );

            Destroy(exploVFX, 2);
            return;
        }

        Debug.LogWarning("InteractionVFX was missing!");
        #endregion
    }

    public void UseRigidbody(bool isTrigger=true, bool useGravity=false)
    {
        #region Omit
        if (_body != null || _collider==null) return;

        _body = gameObject.AddComponent<Rigidbody>();
        _body.useGravity    = useGravity;
        _collider.isTrigger = isTrigger;
        #endregion
    }

    public void SetTime(float time, float div=1f)
    {
        _MagnetMoveDelayTime = (time / div);
        _currTime = 0f;
    }

    public bool Interact()
    {
        GetItem();
        return true;
    }

    public void ExecutionFunction(float time)
    {
        /**NoUsed...*/
    }
}
