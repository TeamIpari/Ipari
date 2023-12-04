using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*****************************************************
 *   �� ����Ʈ�� ���� ������ ����� ������ ������Ʈ�Դϴ�...
 * ***/
[RequireComponent(typeof(SphereCollider))]
public class SandWave : MonoBehaviour
{
    #region Editor_Extension
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SandWave))]
    private class SandWaveEditor : Editor
    {
        //============================================
        //////              Fields               /////
        //============================================
        private SerializedProperty ApplyWaveLimitProperty;
        private SerializedProperty LimitCircleCenterProperty;
        private SerializedProperty LimitCircleRadiusProperty;


        //====================================================
        //////           Override methods               //////
        //====================================================
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            /***************************************
             *   ��� ������Ƽ���� ǥ���Ѵ�....
             * ***/
            GUI_Initialized();

            GUI_ShowLimitproperties();

            /**���� ��ȭ�� �ִٸ� �����Ѵ�...*/
            if (GUI.changed){ 

                serializedObject.ApplyModifiedProperties();
            }


        }



        //==================================================
        //////              GUI methods               //////
        //=================================================
        private void GUI_Initialized()
        {
            #region Omit

            /*****************************************
             *   ��� ������Ƽ���� �ʱ�ȭ�Ѵ�...
             * ***/
            if(ApplyWaveLimitProperty==null){

                ApplyWaveLimitProperty = serializedObject.FindProperty("ApplyWaveLimit");
            }

            if(LimitCircleCenterProperty==null){

                LimitCircleCenterProperty = serializedObject.FindProperty("LimitCircleCenter");
            }

            if(LimitCircleRadiusProperty==null){

                LimitCircleRadiusProperty = serializedObject.FindProperty("LimitCircleRadius");
            }


#endregion
        }

        private void GUI_ShowLimitproperties()
        {
            #region Omit

            /****************************************************************
             *    ���̺� ��ġ ������ ����� ��쿡�� ���� ������Ƽ���� ǥ���Ѵ�...
             * ****/
            if(ApplyWaveLimitProperty.boolValue = EditorGUILayout.ToggleLeft("Use Wave Position Limit", ApplyWaveLimitProperty.boolValue)){

                LimitCircleCenterProperty.vector3Value  = EditorGUILayout.Vector3Field("Limit Circle Center", LimitCircleCenterProperty.vector3Value);
                LimitCircleRadiusProperty.floatValue    = EditorGUILayout.FloatField("Limit Circle Radius", LimitCircleRadiusProperty.floatValue);
            }

#endregion
        }

    }
#endif
#endregion

    //===============================================
    //////              Property                /////
    //===============================================
    public bool IsSpeading { get; private set; } = false;

    [SerializeField] public AnimationCurve waveCurve;
    [SerializeField] public SandScriptBase SandTarget;
    [SerializeField] public GameObject     SandFX;
    [SerializeField] public float          FXScale = 1f;
    [SerializeField] public float          WaveDuration;
    [SerializeField] public float          WaveMaxRadius;
    [SerializeField] public int            Pricision = 10;
    [SerializeField] public bool           UseWaveMaterial = false;

    [SerializeField, HideInInspector]
    public bool     ApplyWaveLimit  = false;

    [SerializeField, HideInInspector]
    public Vector3  LimitCircleCenter = Vector3.zero;

    [SerializeField, HideInInspector]
    public float    LimitCircleRadius = 0f;



    //=============================================
    //////              Fields                /////
    //=============================================
    private GameObject[]   _FXLists;
    private SphereCollider _collider;
    private float          _radianDiv = 0f;

    private float _timeLeft         = -1f;
    private float _shakeTimeLeft    = 0f;
    private float _shakeTime        = .5f;
    private float _shakeMaxTime     = .8f;
    private float _timeDiv          = 1f;
    private float _delayTime        = 0f;

    /**SandWaveMaterial ����...*/
    private Material        _waveMat;
    private float           _waveTotalFrame     = 0f;
    private static float[]  _waveMatRadiusTable = new float[]{

        0.74f, 1.26f, 1.82f, 2.24f, 2.71f,      //( 0~4 )
        3.08f, 4f, 4.5f, 4.9f, 5.25f, 5.33f,    //( 5~9 )
        5.56f, 5.86f, 5.98f, 6.35f, 6.49f,      //( 10~14 )
        6.76f, 6.98f, 7.13f, 7.19f, 7.36f,      //( 15~19 )
        7.45f, 7.65f, 7.84f, 7.87f, 8.21f,      //( 20~24 )
        8.47f, 8.56f, 8.78f, 9f, 9.28f,         //( 25~29 )
        9.5f, 9.9f, 9.97f, 10.39f, 11.35f,      //( 30~34 )
        11.79f, 12.34f, 12.87f, 13.48f, 13.96f, //( 35~39 )
        0f                                      //( 40 )
    };



    //========================================
    /////          Magic methods          ////
    //========================================
    private void Awake()
    {
        #region Omit

        /**�ݶ��̴��� �ʱ�ȭ�Ѵ�...*/
        if(_collider = GetComponent<SphereCollider>()){

            _collider.isTrigger = true;
            transform.localScale = Vector3.one;
        }

        /**�ִϸ��̼� Ŀ�긦 �ʱ�ȭ�Ѵ�...*/
        if(waveCurve==null || (waveCurve!=null && waveCurve.length==0) ){

            waveCurve = new AnimationCurve();
            waveCurve.AddKey(0f, 0f);
            waveCurve.AddKey(0.869f, .523f);
            waveCurve.AddKey(1f, 1f);
            waveCurve.postWrapMode = WrapMode.Clamp;
        }

        /*********************************************
         *   Sand FX���� �ʱ�ȭ��Ų��....
         * ***/
        if (SandFX != null && UseWaveMaterial==false){

            _FXLists = new GameObject[Pricision];

            /**����Ʈ���� �����Ѵ�...*/
            for (int i=0; i< Pricision; i++){

                _FXLists[i] = Instantiate(SandFX);

                ParticleSystem            system = _FXLists[i].GetComponent<ParticleSystem>();
                ParticleSystem.MainModule module = system.main;

                module.scalingMode          = ParticleSystemScalingMode.Local;
                system.transform.localScale = (Vector3.one * FXScale);
                system.transform.position   = transform.position;
                system.transform.SetParent(transform);
                system.gameObject.SetActive(false);
            }

            _radianDiv = (Mathf.PI * 2f) / Pricision;
        }

        /**���ĵ� ���͸����� �����´�....*/
        else if(UseWaveMaterial && SandTarget)
        {
            _waveMat        = SandTarget.GetComponent<Renderer>().sharedMaterials[1];
            _waveTotalFrame = _waveMat.GetFloat("_TotalFrame");
            _waveMat.SetFloat("_NormalizedTime", 1f);
        }
        #endregion
    }

    private void Update()
    {
        #region Omit
        if (IsSpeading==false || (_FXLists==null && !UseWaveMaterial)) return;

        /**���ĵ��� ���۵Ǿ��� ��, �����ð� ����Ѵ�...*/
        float deltaTime = Time.deltaTime;
        if(_delayTime>0f)
        {
            _delayTime -= deltaTime;
            return;
        }

        _timeLeft -= deltaTime;


        /*************************************************
         *   ���� ������� �����µ� �ʿ��� ��ҵ��� ���Ѵ�...
         * *****/
        Vector3 center      = transform.position;
        float progressRatio = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1.5f);
        float currRadian    = 0f;
        float currRadius    = WaveMaxRadius * waveCurve.Evaluate(progressRatio);

        /**�ݶ��̴��� ũ�⸦ �����Ѵ�....*/
        if(UseWaveMaterial)
        {
            _collider.radius = _waveMatRadiusTable[System.Math.Clamp(Mathf.FloorToInt(_waveTotalFrame*progressRatio)-4, 0, 40)];
            _waveMat.SetFloat("_NormalizedTime", Mathf.Clamp01(progressRatio));
        }

        /**��ƼŬ ����Ʈ�� ����� ���....*/
        else
        {
            _collider.radius = currRadius;

            /********************************************
             *    ����Ʈ���� ����� �������� �̵���Ų��...
             * ****/
            float radius = _collider.radius;
            int layer = (1 << LayerMask.NameToLayer("Platform"));
            for (int i = 0; i < Pricision; i++){

                Vector3 dir = new Vector3(Mathf.Cos(currRadian), 0f, Mathf.Sin(currRadian));
                Vector3 newPos = center + (dir * radius);

                /**��ġ������ ����� ���, ��ġ�� �����Ѵ�...*/
                if (ApplyWaveLimit)
                {
                    Vector3 center2newPos = (newPos - LimitCircleCenter);
                    float center2newPosLen = (newPos - LimitCircleCenter).sqrMagnitude;
                    float limitCenterRadius = (LimitCircleRadius * LimitCircleRadius);

                    if (center2newPosLen > limitCenterRadius){

                        center2newPos.Normalize();
                        newPos = LimitCircleCenter + (center2newPos * LimitCircleRadius);
                    }
                }


                /**����Ʈ�� ���� �ٴ��� ��ġ�� ���Ѵ�....*/
                RaycastHit hit;
                if (Physics.Raycast(newPos, Vector3.down, out hit, 10f, layer)){

                    newPos.y = hit.point.y;
                }


                /**���� ����....*/
                _FXLists[i].transform.position = newPos;
                currRadian += _radianDiv;
            }
        }

        /**������ �Ǿ��� ���...*/
        if (progressRatio >= 1.5f){

            Vector3 thisPos = transform.position;
            if(UseWaveMaterial==false)
            {
                for (int i = 0; i < Pricision; i++){

                    GameObject sandFX = _FXLists[i];
                    sandFX.SetActive(false);
                    sandFX.transform.position = thisPos;
                }
            }

            _timeLeft = -1;
            _collider.radius = 0f;
            IsSpeading = false;
        }

        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        #region Omit
        if (IsSpeading == false) return;

        Rigidbody body;
        Vector3 center2Target  = (other.transform.position - transform.position);
        float center2TargetLen = center2Target.sqrMagnitude;
        float progressRatio    = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float compareLen       = (_collider.radius - 1f);

        /**�浹�ߴ��� üũ�� �Ѵ�...*/
        if (SandTarget != null && SandTarget.PlayerOnSand == false) return;
        if (!(center2TargetLen> (compareLen*compareLen))) return;
        if (progressRatio>=1f) return;


        /****************************************
         *   �÷��̾ ���� �����̸� ��ŵ�Ѵ�...
         * ***/
        bool playerOnSand = (SandTarget != null ? SandTarget.PlayerOnSand : true);
        if (other.CompareTag("Player") && playerOnSand)
        {
            if (Player.Instance.movementSM.currentState == Player.Instance.jump) return;

            Player.Instance.isDead = true;
        }


        /******************************************
         *   Rigidbody�� ������ ���� ���� ó��...
         * ****/
        else if(other.CompareTag("interactable") && !other.CompareTag("Boss") && (body=other.attachedRigidbody)!=null){

            float   dst = (compareLen - center2TargetLen);
            Vector3 pow = (-center2Target.normalized * dst *.3f * body.mass);

            body.velocity = pow;
        }

        #endregion
    }

    private void OnDrawGizmos()
    {
        #region Omit
        if (Application.isPlaying == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collider.radius - 1f);

        #endregion
    }



    //==========================================
    /////           Core methods            ////
    //==========================================
    public void StartWave()
    {
        #region Omit
        /**************************************
         *   ���ĵ��� ����Ų��....
         * ****/
        if (_FXLists == null && !UseWaveMaterial) return;

        /**�����Ǿ��� FX���� Ȱ��ȭ��Ų��...*/
        if(UseWaveMaterial==false)
        {
            for (int i = 0; i < Pricision; i++){

                _FXLists[i].transform.position = transform.position;
                _FXLists[i].SetActive(true);
            }
        }

        /**�ĵ� ��꿡 �ʿ��� ��� ��ҵ��� ���Ѵ�...*/
        _collider.radius = 0f; 
        _radianDiv       = (Mathf.PI * 2f) / Pricision;
        _timeLeft        = WaveDuration;
        _timeDiv         = (1f / WaveDuration);

        _shakeTime = 0f;
        _shakeTime = _shakeMaxTime;
        _delayTime = .1f;
        IsSpeading = true;
        #endregion
    }

    private bool CheckHitWave(Vector3 worldPos)
    {
        #region Omit

        /**************************************************
         *   �־��� worldPos�� ���������� ���Դ��� �˻��Ѵ�.
         * ***/

        float center2TargetLen = ( worldPos - transform.position ).sqrMagnitude;
        float progressRatio    = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float compareLen       = WaveMaxRadius * waveCurve.Evaluate(progressRatio)-1f;
        compareLen *= compareLen;

        return (center2TargetLen > compareLen);
#endregion
    }

    private void InitSandFXPosition()
    {
        #region Omit
        if (_FXLists == null) return;

        int Count = _FXLists.Length;
        for(int i=0; i<Count; i++)
        {
            GameObject fx = _FXLists[i];
            fx.transform.localPosition = Vector3.zero;
        }
#endregion
    }


}
