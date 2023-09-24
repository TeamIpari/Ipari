using UnityEngine;

/************************************************
 *  ���� ��ü�� �Լ����� ���� ó���ϴ� ������Ʈ�Դϴ�.
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

    /**Effect pool ����*/
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
        Collider collider= GetComponent<Collider>();
        if(collider!=null)
        {
            Bounds bounds= collider.bounds;
            _waterHeight = bounds.center.y + (bounds.extents.y * bounds.size.y);
        }

        /**Particle �ʱ�ȭ..*/
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
        /**���� �Լ��� ����� �÷��̾��� ���...*/
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

        /**�Լ��� ������ ������Ʈ�鿡�� ���������� ����.*/
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Enter_Water);
        PlayWaterFX(other);
    }

    private void OnTriggerStay(Collider other)
    {
        /*PlayerMask�� üũ�Ͽ� �̵��� ��Ŵ.*/
        if(other.gameObject.CompareTag( "Player")){

            Debug.Log("AA");
            _player.movementSM.currentState.gravityVelocity += WaterDir * WaterForce;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")/* && _player.movementSM.currentState.Equals(_player.jump)*/){

            _player.jumpHeight = _playerDefaultJumPow;
            _player.movementSM.currentState.gravityVelocity.x = 0;
            _player.movementSM.currentState.gravityVelocity.z = 0;
        }
        else return;

        /**�Լ��� ������ ������Ʈ�鿡�� ���������� ����.*/
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

    private void PlayWaterFX(Collider collider)
    {
        #region Omit
        if(EnterWaterFX!=null)
        {
            ParticleSystem system = GetWaterFX();
            system.gameObject.SetActive(true);
            
            Vector3 pos = collider.transform.position;
            pos.y = _waterHeight;
            system.transform.position = pos;
            system.Play();

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
