using System;
using UnityEngine;
using IPariUtility;
using UnityEngine.Serialization;
using UnityEditor.SceneManagement;

/*********************************************************************
 *    상호작용시, 특정 수집요소를 카운팅하는 기능이 구현된 컴포넌트입니다...
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

    private float     _currTime        = 0f;    //경과 시간....
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

        /**플레이어의 트랜스폼 캐싱...*/
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
         *   지연시간 만큼 대기후, 자석 효과를 적용한다...
         * ***/
        if(_currTime<MagnetMoveDelayTime){

            if ((_currTime+=Time.deltaTime) >= MagnetMoveDelayTime)
            {
                /**지연시간이 끝났을 경우...*/
                ApplyTimeout();
            }
            else return;
        }

        /**************************************
         *   자석의 움직임 로직을 적용한다....
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

                /**코인을 획득하였을 경우...*/
                case (ScoreType.Coin):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Bead
                    );

                    gm.Coin++;
                    break;
                }

                /**코코시를 획득하였을 경우...*/
                case (ScoreType.Cocosi):
                {

                    break;
                }

                /**꽃을 획득하였을 경우....*/
                case (ScoreType.Flower):
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.Get_Flower
                    );

                    gm.Flower++;
                    break;
                }
        }

        /**아이템 획득시 떠오르는 효과를 구현한다...*/
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

        /**아이템이 파괴되면서 이펙트를 생성한다...*/
        SpawnVFX();
        Destroy(gameObject);

        #endregion
    }

    private void ApplyTimeout()
    {
        #region Omit
        /**********************************************
         *   특정 지연시간이 지난후, 지정한 효과를 적용한다...
         * ***/

        /**획득되었을 경우...*/
        if(_collider!=null && _collider.enabled==false){

            SpawnVFX();
            Destroy(gameObject);
            return;
        }

        /**자석효과를 적용할 경우...*/
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
