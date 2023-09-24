using UnityEngine;
using static UnityEditor.PlayerSettings;

/************************************************
 *  물에 물체가 입수했을 때를 처리하는 컴포넌트입니다.
 * ***/
public sealed class WaterScript : MonoBehaviour
{
    //========================================
    /////       Property and Fields      ////
    //=======================================
    [SerializeField][Tooltip("swept away dir.")]
    Vector3     WaterDir = Vector3.zero;

    [SerializeField][Tooltip("swept away force.")][ Range(0f, 1f) ]
    float       WaterForce = .1f;

    [SerializeField][Tooltip("Enter Water FX.")]
    GameObject  EnterWaterFX;

    [SerializeField]
    bool        JumpPowRedution = false;


    private float                 _playerDefaultJumPow = 0f;
    private float                 _waterHeight = 0f;

    /**Effect pool 관련*/
    private static ParticleSystem[] _FXInsList;
    private static int _AliveIndex = 0;
    private const int _FXCount = 10;

    private Player _player;



    //=======================================
    /////        Magic methods          /////
    //=======================================
    private void Start()
    {
        #region Omit
        _player = Player.Instance;
        _playerDefaultJumPow = _player.jumpHeight;
        gameObject.layer = LayerMask.NameToLayer("Water");
        Collider collider= GetComponent<Collider>();
        if(collider!=null)
        {
            collider.isTrigger = true;   
            Bounds bounds= collider.bounds;
            _waterHeight = bounds.center.y + (bounds.extents.y * bounds.size.y);
        }

        /**Particle 초기화..*/
        if(_FXInsList==null && EnterWaterFX!=null)
        {
            _FXInsList = new ParticleSystem[_FXCount];
            _FXInsList[0] = CreateWaterFX();

            for(int i=1; i<_FXCount; i++)
            {
                _FXInsList[i] = GameObject.Instantiate(_FXInsList[0]);
                GameObject.DontDestroyOnLoad(_FXInsList[i]);    
            }
            _AliveIndex = (_FXCount-1);
        }
        #endregion
    }

    private void OnDestroy()
    {
        _FXInsList = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        /**물에 입수한 대상이 플레이어일 경우...*/
        if (other.gameObject.CompareTag("Player")/* && _player.movementSM.currentState.Equals(_player.jump)*/)
        {
            if(JumpPowRedution) Player.Instance.jumpHeight = 0.2f;
        }
        else if (other.gameObject.CompareTag("Platform"))
        {

        }
        else if (LayerMask.NameToLayer("Interactable") == other.gameObject.layer)
        {

        }
        else return;

        /**입수가 가능한 오브젝트들에게 공통적으로 실행.*/
        PlayWaterFX(other);
    }

    private void OnTriggerStay(Collider other)
    {
        /*PlayerMask만 체크하여 이동을 시킴.*/
        if(other.gameObject.CompareTag( "Player")){

            _player.controller.Move(WaterDir * WaterForce);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")/* && _player.movementSM.currentState.Equals(_player.jump)*/){

            _player.jumpHeight = _playerDefaultJumPow;
        }
        else return;

        /**입수가 가능한 오브젝트들에게 공통적으로 실행.*/
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Enter_Water);
        PlayWaterFX(other);
    }



    //=======================================
    /////         Core methods          /////
    //=======================================
    private ParticleSystem GetWaterFX()
    {
        #region Omit
        ParticleSystem system = _FXInsList[_AliveIndex--];
        if(_AliveIndex<=0)
        {
            _AliveIndex = _FXInsList.Length - 1;
        }

        return system;
        #endregion
    }

    private void PlayWaterFX(Collider other)
    {
        #region Omit
        RaycastHit result;
        if (Physics.Raycast((other.transform.position + Vector3.up * 10f),
                            Vector3.down,
                            out result,
                            15f,
                            1 << LayerMask.NameToLayer("Water")))

        {
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Enter_Water);

            if (EnterWaterFX != null)
            {
                ParticleSystem system = GetWaterFX();
                system.gameObject.SetActive(true);

                Vector3 pos = result.point;
                system.transform.position = pos;
                system.Play();

            }
        }
        #endregion
    }

    private ParticleSystem CreateWaterFX()
    {
        #region Omit
        ParticleSystem newWaterFX;

        if (EnterWaterFX != null)
        {
            newWaterFX = Instantiate(EnterWaterFX).GetComponent<ParticleSystem>();
            GameObject.DontDestroyOnLoad(newWaterFX);
            newWaterFX.transform.localScale = Vector3.one * 5f;

            ParticleSystem.MainModule module = newWaterFX.transform.Find("Splash").GetComponent<ParticleSystem>().main;
            module.loop = false;
            module.stopAction = ParticleSystemStopAction.Callback;

            module = newWaterFX.transform.Find("FXprefab_J_WaterMove").Find("FXprefab_J_WaterMove").GetComponent<ParticleSystem>().main;
            module.loop = false;

            module = newWaterFX.transform.Find("FXprefab_J_WaterMove").GetComponent<ParticleSystem>().main;
            module.loop = false;

            module = newWaterFX.transform.Find("WaterJump 01").GetComponent<ParticleSystem>().main;
            module.loop = false;

            module = newWaterFX.transform.Find("WaterJump 02").GetComponent<ParticleSystem>().main;
            module.loop = false;

            module = newWaterFX.transform.Find("WaterJump 03").GetComponent<ParticleSystem>().main;
            module.loop = false;
            

            newWaterFX.gameObject.SetActive(false);
            return newWaterFX;
        }

        return null;
        #endregion
    }
    

}
