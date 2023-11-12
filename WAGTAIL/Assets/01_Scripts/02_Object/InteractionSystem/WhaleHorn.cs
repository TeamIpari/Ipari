using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
         *   ���� �и���Ű��, ���� �ۿ��� �����ϵ��� �Ѵ�....
         * *****/
        transform.parent  = null;
        _body.isKinematic = false;
        _body.AddTorque(Vector3.left * 50f);
        _body.AddForce((Vector3.left * 90f));

        float time = 3f;
        while ((time -= Time.deltaTime)>0f) yield return null;


        /************************************************
         *    ���� �������� ��������.....
         * *******/
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

            yield return waitTime;

            /**���� ����Ʈ�� ��ġ�� �����Ѵ�...*/
            if (_state==HornState.Fly_Idle && _ShineSFXIns!=null){

                _ShineSFXIns.position = collider.bounds.center;
            }

            /**��ȣ�ۿ��� �����ϰ� �ϰ�, ���� ����Ʈ�� �����Ѵ�....*/
            else if(_state==HornState.Fly && dst.sqrMagnitude<=1f && ShineSFXPrefab)
            {
                _state              = HornState.Fly_Idle;
                _ShineSFXIns = Instantiate(ShineSFXPrefab).transform;
                _ShineSFXIns.position = collider.bounds.center;

                /**��ȣ�ۿ��� �����ϵ��� �Ѵ�...*/
                InteractActionDispatcher dispatcher = InteractableGoalPos.GetComponent<InteractActionDispatcher>();
                if(dispatcher!=null){

                    dispatcher.gameObject.SetActive(true);
                    dispatcher.IsInteractable = true;
                    dispatcher.OnInteract.AddListener(() =>
                    {
                        /**��ȣ�ۿ��� �Ұ����ϰ� �ϰ�, ���̵带 �����Ѵ�....*/
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
