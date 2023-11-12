using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Enter_CutScene
    }

    //========================================================
    //////            Property and fields               //////
    //========================================================
    public Rigidbody Body                { get { return _body; } }

    [SerializeField] public Transform      InteractableGoalPos;
    [SerializeField] public GameObject     ShineSFXPrefab;
    [SerializeField] private float         MoveDuration = 1f;
    [SerializeField] private float         MoveMaxHeight;


    private Rigidbody _body;
    private Transform _ShineSFXIns;
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
        _body = GetComponent<Rigidbody>();
        _body.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region 
        if (!collision.gameObject.CompareTag("Platform")) return;

        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Walk, 
            FModLocalParamType.PlayerWalkType, 
            FModParamLabel.PlayerWalkType.Sand,
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
    private IEnumerator HornDropProgress()
    {
        #region Omit

        /****************************************************
         *   뿔을 분리시키고, 물리 작용이 가능하도록 한다....
         * *****/
        transform.parent  = null;
        _body.isKinematic = false;
        _body.AddTorque(Vector3.left * 50f);
        _body.AddForce((Vector3.left * 90f));

        float time = 3f;
        while ((time -= Time.deltaTime)>0f) yield return null;


        /************************************************
         *    뿔이 공중으로 떠오른다.....
         * *******/
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

            yield return waitTime;

            /**샤인 이펙트의 위치를 갱신한다...*/
            if (_state==HornState.Fly_Idle && _ShineSFXIns!=null){

                _ShineSFXIns.position = collider.bounds.center;
            }

            /**상호작용을 가능하게 하고, 샤인 이펙트를 생성한다....*/
            else if(_state==HornState.Fly && dst.sqrMagnitude<=1f && ShineSFXPrefab)
            {
                _state              = HornState.Fly_Idle;
                _ShineSFXIns = Instantiate(ShineSFXPrefab).transform;
                _ShineSFXIns.position = collider.bounds.center;

                /**상호작용을 가능하도록 한다...*/
                InteractActionDispatcher dispatcher = InteractableGoalPos.GetComponent<InteractActionDispatcher>();
                if(dispatcher!=null){

                    dispatcher.gameObject.SetActive(true);
                    dispatcher.IsInteractable = true;
                    dispatcher.OnInteract.AddListener(() =>
                    {
                        /**상호작용을 불가능하게 하고, 페이드를 적용한다....*/
                        dispatcher.IsInteractable = false;

                        Player.Instance.stiffen.StiffenTime = -1f;
                        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);
                        FadeUI fade = UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>();
                        fade.FadeIn(FadeType.Normal);
                    });
                }
            }
        }
        while (true);


        #endregion
    }

}
