using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**********************************************
 *   다수의 플랫폼에게 충격을 가하는 컴포넌트입니다.
 * ****/
public sealed class ChildPlatformsShaker : MonoBehaviour
{
    //======================================================
    //////            Property and Fields              /////
    //======================================================
    public float WaveMaxDistance { get { return _maxDst; } set { _maxDst=(value<0f?0f:value); } }
    public float WaveMaxDelay    { get { return _delay; } set { _delay = (value < 0f ? 0f : value); } }

    [SerializeField] private GameObject[] TestWaveStarters;
    [SerializeField] public float  _maxDst = 11f;
    [SerializeField] public float  yspeed  = -.1f;
    [SerializeField] public float  _delay  = .5f;
    [SerializeField] public float  rotPow  = .1f;


    /**충격을 적용할 WavePlatformBehaviors 관련...*/
    private WavePlatformBehavior[] behaviors;
    private int   behaviorsNum = 0;
    private float center2EdgeDstDiv = 0f;

    private Collider[] colliders = new Collider[10];


    //====================================================
    ///////             Magic methods               //////
    //====================================================
    private void Start()
    {
        #region Omit

        int Count = transform.childCount;

        center2EdgeDstDiv = (1f / _maxDst);
        behaviors = new WavePlatformBehavior[Count];

        /**유효한 WavePlatofrmBehaivors들을 가져온다...*/
        for(int i=0; i<Count; i++)
        {
            if ((behaviors[behaviorsNum]=transform.GetChild(i).GetComponent<WavePlatformBehavior>())!=null){

                behaviorsNum++;
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    private void Update()
    {
        #region Omit
        /**테스트용 진동 발생 로직....*/
        if (Input.GetKeyDown(KeyCode.P) && TestWaveStarters!=null)
        {
            int Count = Physics.OverlapBoxNonAlloc(
                transform.position + (Vector3.left * 3f),
                new Vector3(1, 1, 10f),
                colliders,
                transform.rotation,
                (1 << LayerMask.NameToLayer("Platform")),
                QueryTriggerInteraction.Ignore
            );

            if (Count > 0)
            {
                MakeWave(colliders);
                FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_VineSmash, Vector3.zero, 2f);
                CameraManager.GetInstance().CameraShake(.4f, CameraManager.ShakeDir.ROTATE, .5f);
            }

            //FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_VineSmash, Vector3.zero, 2f);
            //CameraManager.GetInstance().CameraShake(.4f, CameraManager.ShakeDir.ROTATE, .5f);
            //MakeWave(TestWaveStarters);
        }
        #endregion
    }
#endif



    //=====================================================
    ////////              Core methods            ////////
    //=====================================================
    public void MakeWave( params GameObject[] waveStartObjs )
    {
        #region Omit
        /********************************************************
         *    지정한 오브젝트들에게 충격을 가하고, 주변에게 전파한다...
         * *****/
        int   starterCount  = waveStartObjs.Length;
        float startCountDiv = (1f / starterCount);

        float yspeedDiv  = (yspeed * startCountDiv);
        float rotpowDiv  = (rotPow * startCountDiv); 

        for(int i=0; i<starterCount; i++){

            if (waveStartObjs[i] == null) continue;
            Vector3 centerPos = waveStartObjs[i].transform.position;

            /**주변 발판들에게 충격을 전파한다.*/
            for (int j = 0; j < behaviorsNum; j++)
            {
                float ratio    = (1f - Vector3.Distance(centerPos, behaviors[j].transform.position) * center2EdgeDstDiv);
                behaviors[j].Yspeed         += (yspeedDiv * ratio);
                behaviors[j].Rotspeed       += -rotpowDiv;
                behaviors[j].UpdateDelay    = _delay - (_delay * ratio);
                behaviors[j].SetRotDir(centerPos);
            }
        }

        /**스타터들을 갱신한다...*/
        for(int i=0; i<starterCount; i++){

            WavePlatformBehavior behavior = GetBehaviorFromStarters(waveStartObjs[i], behaviors);
            if (behavior == null) continue;

            behavior.Yspeed      = yspeed;
            behavior.Rotspeed    = -rotPow;
            behavior.UpdateDelay = 0f;
            behavior.SetRotDir(behavior.transform.position + Random.onUnitSphere);
        }
        #endregion
    }

    public void MakeWave(params Collider[] waveStartObjs)
    {
        #region Omit
        /********************************************************
         *    지정한 오브젝트들에게 충격을 가하고, 주변에게 전파한다...
         * *****/
        int starterCount = waveStartObjs.Length;
        float startCountDiv = (1f / starterCount);

        float yspeedDiv = (yspeed * startCountDiv);
        float rotpowDiv = (rotPow * startCountDiv);

        for (int i = 0; i < starterCount; i++)
        {
            if (waveStartObjs[i] == null) continue;

            if (waveStartObjs[i] == null) continue;
            Vector3 centerPos = waveStartObjs[i].transform.position;

            /**주변 발판들에게 충격을 전파한다.*/
            for (int j = 0; j < behaviorsNum; j++)
            {
                float ratio = (1f - Vector3.Distance(centerPos, behaviors[j].transform.position) * center2EdgeDstDiv);
                behaviors[j].Yspeed += (yspeedDiv * ratio);
                behaviors[j].Rotspeed += -rotpowDiv;
                behaviors[j].UpdateDelay = _delay - (_delay * ratio);
                behaviors[j].SetRotDir(centerPos);
            }
        }

        /**스타터들을 갱신한다...*/
        for (int i = 0; i < starterCount; i++)
        {
            if (waveStartObjs[i] == null) continue;

            WavePlatformBehavior behavior = GetBehaviorFromStarters(waveStartObjs[i].gameObject, behaviors);
            if (behavior == null) continue;

            behavior.Yspeed = yspeed;
            behavior.Rotspeed = -rotPow;
            behavior.UpdateDelay = 0f;
            behavior.SetRotDir(behavior.transform.position + Random.onUnitSphere);
        }
        #endregion
    }

    public void MakeWave(Vector3 waveStartObjs)
    {
        #region Omit

        /**주변 발판들에게 충격을 전파한다.*/
        for (int j = 0; j < behaviorsNum; j++)
        {
            float ratio = (1f - Vector3.Distance(waveStartObjs, behaviors[j].transform.position) * center2EdgeDstDiv);
            behaviors[j].Yspeed     += (yspeed * ratio);
            behaviors[j].Rotspeed   += rotPow;
            behaviors[j].UpdateDelay = _delay - (_delay * ratio);
            behaviors[j].SetRotDir(waveStartObjs);
        }

        #endregion
    }

    private WavePlatformBehavior GetBehaviorFromStarters(GameObject starters, WavePlatformBehavior[] behaviors  )
    {
        #region Omit
        int Count = behaviors.Length;
        for(int i=0; i<Count; i++){

            if (behaviors[i]!=null && starters.Equals(behaviors[i].gameObject)) 
                return behaviors[i];
        }

        return null;
        #endregion
    }

    private bool BehaviorIsStarter(GameObject[] starters, WavePlatformBehavior behavior)
    {
        #region Omit
        
        int Count = starters.Length;
        for(int i=0; i<Count; i++)
        {
            if (behavior.gameObject.Equals(starters[i])) return true;
        }

        return false;
        #endregion
    }



}
