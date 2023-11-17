using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using IPariUtility;
using AmplifyShaderEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************
 *   새가 날라가는 효과가 구현된 컴포넌트입니다.
 * **/
[AddComponentMenu("Triggerable/FlyingBird")]
public sealed class FlyingBird : MonoBehaviour
{
    #region Editor_Extension
    /***************************************
     *  에디터 확장을 위한 private class...
     * ***/
#if UNITY_EDITOR
    [CustomEditor(typeof(FlyingBird))]
    private sealed class FlyingBirdEditor : Editor
    {
        //======================================
        /////           Fields              ////
        //======================================
        private FlyingBird         flyingBird;
        private Transform          birdTransform;
        private SerializedProperty FlyPointsProperty;
        private SerializedProperty FlyPointsCountProperty;
        private SerializedProperty FlyTypeProperty;

        private static readonly string[] flyTypes = new string[] { "Default", "Used Bezier" }; 



        //=====================================================
        /////       Override and mahic methods             ////
        //=====================================================
        private void OnSceneGUI()
        {
            #region Omit
            if (FlyPointsProperty == null) return;

            serializedObject.Update();

            /**************************************
             *   모든 점을 표시한다....
             * ***/
            //Tools.current = Tool.None;
            Handles.color = Color.green;
            int Count = FlyPointsProperty.arraySize;

            for(int i=0; i<Count; i++)
            {
                using (var changeScope = new EditorGUI.ChangeCheckScope()){

                    SerializedProperty element = FlyPointsProperty.GetArrayElementAtIndex(i);
                    Vector3 point = element.vector3Value;
                    point = Handles.PositionHandle(point, Quaternion.identity);

                    /**값이 변경되었다면 갱신한다...*/
                    if(changeScope.changed)
                    {
                        element.vector3Value = point;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            #endregion
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            /***************************************
             *   모든 프로퍼티들을 표시한다...
             * ***/
            GUI_Initialized();
            serializedObject.Update();

            EditorGUILayout.Space(10f);

            GUI_ShowFlyType();

            GUI_ShowFlyPoints();

            /**값에 변경요소가 있다면 갱신...*/
            if (GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
        }



        //========================================
        /////          GUI methods            ////
        //========================================
        private void GUI_Initialized()
        {
            #region Omit
            /***************************************
             *  모든 SerializedProperty를 초기화한다..
             * ***/
            if (FlyPointsProperty==null){

                FlyPointsProperty = serializedObject.FindProperty("_FlyPoints");
            }

            if(FlyPointsCountProperty==null){

                FlyPointsCountProperty = serializedObject.FindProperty("_FlyPointsCount");
            }

            if(birdTransform==null){

                flyingBird = (target as FlyingBird);
                if (flyingBird != null) birdTransform = flyingBird.transform;
            }

            if(FlyTypeProperty==null){

                FlyTypeProperty = serializedObject.FindProperty("_FlyType");
            }

            #endregion
        }

        private void GUI_ShowFlyType()
        {
            #region Omit
            using (var changeScope = new EditorGUI.ChangeCheckScope()){

                int selected = GUILayout.Toolbar(FlyTypeProperty.intValue, flyTypes);
                if(changeScope.changed)
                {
                    FlyTypeProperty.intValue = selected;    
                }
            }
            #endregion
        }

        private void GUI_ShowProperty(SerializedProperty property)
        {
            #region Omit
            if (property == null) return;
            EditorGUILayout.PropertyField(property, true);
            #endregion
        }

        private void GUI_ShowFlyPoints()
        {
            #region Omit
            if (FlyPointsProperty == null || FlyPointsCountProperty==null) return;

            using (var scope=new EditorGUI.ChangeCheckScope()){

                EditorGUILayout.PropertyField(FlyPointsProperty, true);

                int prevLen = FlyPointsCountProperty.intValue;
                int currLen = FlyPointsProperty.arraySize;

                /**값이 변경되었을 경우...*/
                if (scope.changed)
                {
                    FlyPointsCountProperty.intValue = currLen;

                    /*추가된 포인트의 값을 마지막 또는 새의 위치로...*/
                    if( (birdTransform != null) && (currLen > prevLen) ){

                        SerializedProperty lastElement = FlyPointsProperty.GetArrayElementAtIndex(currLen-1);

                        /**첫번째 원소일 경우...*/
                        if(currLen==1) lastElement.vector3Value = birdTransform.position;

                        /**마지막 원소일 경우...*/
                        else{

                            SerializedProperty prevElement = FlyPointsProperty.GetArrayElementAtIndex(currLen - 2);
                            lastElement.vector3Value = prevElement.vector3Value + (Vector3.up * 1.2f);
                        }
                    }
                }

                scope.Dispose();
            }
            #endregion
        }

        private void GUI_DrawLine(float space = 0f, float subOffset = 0f)
        {
            #region Omit
            EditorGUILayout.Space(15f);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15 + subOffset, rect.y), new Vector2(rect.width + 15 - subOffset * 2, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10f + space);
            #endregion
        }


    }
#endif
    #endregion

    public enum RotationAxis
    {
        X = 1,
        Y = 2,
        Z = 4,

        XY  = (X|Y),
        XYZ = (X|Y|Z),
        YZ  = (Y|Z)
    }

    //===================================
    //////       Property           /////
    //===================================
    public Animator  BirdAnimator        { get { return _animator; } }
    public Vector3[] FlyPoints           { get { return _FlyPoints; } }
    public float     DurationUntilFlight { get { return _DurationUntilFlight; } set { if (value < 0) value = .1f; _DurationUntilFlight = value; } }
    public int       FlyType             { get{ return _FlyType; } set { _FlyType = value; } }
    public float     FlightDuration      { get { return _FlightDuration; } set { if (value < 0) value = .1f; _FlightDuration = value; } }

    [SerializeField] public  Vector3     FlyRotation            = Vector3.zero;
    [SerializeField] private float       _DurationUntilFlight   = 1f;
    [SerializeField] private float       _FlightDuration        = 5f;
    [SerializeField] public float        RotationDuration       = .3f; 
    [SerializeField] public  bool        DestroyAtArrived       = true;
    [SerializeField] public  bool        UseFlyLoop             = false;
    [SerializeField] public  bool        FlyOnStart             = false;
    [SerializeField] public  string      IdleAnimState          = string.Empty;
    [SerializeField] public  string      ReadyAnimState         = string.Empty;
    [SerializeField] public  string      FlyAnimState           = string.Empty;
    [SerializeField] public RotationAxis RotationAxisType       = FlyingBird.RotationAxis.XYZ;


    //===================================
    //////         Fields          /////
    //===================================
    [SerializeField, HideInInspector] 
    private Vector3[]   _FlyPoints;

    [HideInInspector] [SerializeField] 
    private int         _FlyPointsCount = 0;

    [HideInInspector][SerializeField]
    private int         _FlyType = 0;

    private Animator  _animator;
    private Coroutine _flyCoroutine;
    private Transform _dirTr;



    //========================================
    //////       Magic methods           /////
    //========================================
    private void Awake()
    {
        _dirTr    = transform.Find("dir");
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if(_animator!=null)
        {
            _animator?.Play(IdleAnimState);
        }

        if (FlyOnStart) Fly();
    }

    private void OnDrawGizmosSelected()
    {
        #region Omit
#if UNITY_EDITOR
        if (_FlyPoints == null || _FlyPointsCount==0) return;

            /***********************************
             *   연결 지점들을 모두 표시한다..
             * ***/
            if (_FlyType==0)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(_FlyPoints[0], .2f);

                Gizmos.color = Color.blue;
                for (int i = 1; i < _FlyPointsCount; i++){

                    Gizmos.DrawSphere(_FlyPoints[i], .2f);
                    Gizmos.DrawLine(_FlyPoints[i - 1], _FlyPoints[i]);
                }

                return;
            }

            /***************************************
             *   연결 지점들을 배지어 곡선으로 표시한다.
             * ***/
            int j = 1;
            int goal = (_FlyPointsCount - 1);

            Gizmos.color = Color.yellow;
            for (; j<goal; j+=2)
            {
                Gizmos.DrawSphere(_FlyPoints[j-1], .2f);
                Gizmos.DrawSphere(_FlyPoints[j], .2f);
                Gizmos.DrawSphere(_FlyPoints[j+1], .2f);

                Gizmos.color = Color.black;
                Gizmos.DrawLine(_FlyPoints[j-1], _FlyPoints[j]);
                Gizmos.DrawLine(_FlyPoints[j], _FlyPoints[j + 1]);

                /**배지어 곡선을 그린다..*/
                Vector3 prev = _FlyPoints[j-1];
                Gizmos.color = Color.yellow;

                for(float w=0f; w<1f; w+=.15f)
                {
                    Vector3 curr = IpariUtility.GetBezier(ref _FlyPoints[j-1], ref _FlyPoints[j], ref _FlyPoints[j + 1], w);
                    Gizmos.DrawLine(prev, curr);
                    prev = curr;
                }

                Gizmos.color = Color.yellow;
            }

            /**못그린 지점들을 모두 그린다.*/
            for(int k=(j-1); k<_FlyPointsCount; k++ )
            {
                Gizmos.DrawSphere(_FlyPoints[k], .2f);
            }
#endif
        #endregion
    }



    //=====================================
    //////       Core methods        /////
    //=====================================
    public void Fly()
    {
        if (_flyCoroutine != null) return;
        _flyCoroutine = StartCoroutine(FlyProgress());
    }

    private void BirdLookAt( Vector3 lookPos, float ratio )
    {
        #region Omit
        Transform birdTr = transform;

        Vector3 currDir = (_dirTr.position - birdTr.position).normalized;
        Vector3 goalDir = (lookPos - birdTr.position).normalized;

        float   angle   = Vector3.Angle(currDir, goalDir) * ratio;
        Vector3 cross   = Vector3.Cross(currDir, goalDir);


        /**X축 회전을 제거한다...*/
        if ((RotationAxisType & RotationAxis.X) == 0)
                cross.x = 0f;

        /**Y축 회전을 제거한다...*/
        if ((RotationAxisType & RotationAxis.Y) == 0)
                cross.y = 0f;

        /**Z축 회전을 제거한다...*/
        if ((RotationAxisType & RotationAxis.Z) == 0)
                cross.z = 0f;


        Quaternion rotQuat = Quaternion.AngleAxis(angle, cross);
        birdTr.rotation    = (rotQuat * birdTr.rotation);
        #endregion
    }

    private IEnumerator FlyProgress()
    {
        #region Omit
        if (FlightDuration == 0f || DurationUntilFlight == 0f) yield break;

        /*******************************
         *   날기전의 대기동작...
         * ***/
        float currTime    = 0f;
        float goalTimeDiv = (1f / DurationUntilFlight);
        float rotDiv      = (1f / RotationDuration);

        //_animator?.Play(ReadyAnimState);
        Vector3 startForward = transform.forward;
        Quaternion startQuat = transform.rotation;

        while (currTime < DurationUntilFlight)
        {
            currTime += Time.deltaTime;
            float progressRatio = Mathf.Clamp((currTime * goalTimeDiv), 0f, 1f);
            BirdLookAt(_FlyPoints[1], progressRatio);
            yield return null;
        }


        /*****************************************
         *   날아가는데 필요한 모든 요소들을 구한다...
         * ***/
        int Count           = 0;
        float goalTime      = 0f;
        goalTimeDiv         = 0f;
        Transform birdTr    = transform;

        /**베지어를 사용할 경우...*/
        if (_FlyType == 1)
        {
            Count = (_FlyPointsCount - 1) / 2;
            goalTime = (FlightDuration / Count);
            goalTimeDiv = (1f / goalTime);
            currTime = 0f;

            do
            {
                for (int i = 0; i < Count; i += 1)
                {
                    Vector3 prevDir = birdTr.forward;
                    int real = (i * 2);
                    startQuat = transform.rotation;

                    while (currTime < goalTime)
                    {

                        currTime += Time.deltaTime;
                        float progressRatio = Mathf.Clamp01(currTime * goalTimeDiv);
                        float rotRatio = Mathf.Clamp01(currTime * rotDiv);

                        /**회전에 필요한 모든값들을 구한다...*/
                        Vector3 nextPos = IpariUtility.GetBezier(
                            ref _FlyPoints[real],
                            ref _FlyPoints[real + 1],
                            ref _FlyPoints[real + 2],
                            progressRatio
                        );

                        BirdLookAt(_FlyPoints[real + 2], rotRatio);
                        birdTr.position = nextPos;
                        yield return null;
                    }

                    currTime -= goalTime;
                }
            }
            while (UseFlyLoop);
        }

        /**기본값..*/
        else
        {
            Count       = (_FlyPointsCount-1);
            goalTime    = (FlightDuration / Count);
            goalTimeDiv = (1f / goalTime);
            currTime    = 0f;


            do
            {
                /**날아가는 로직을 적용한다....*/
                for (int i = 0; i < Count; i += 1){

                    Vector3 prevDir = birdTr.forward;
                    startQuat = transform.rotation;

                    while (currTime < goalTime)
                    {
                        currTime += Time.deltaTime;
                        float progressRatio = Mathf.Clamp01(currTime * goalTimeDiv);
                        float rotRatio = Mathf.Clamp01(currTime * rotDiv);
                        Vector3 nextPos = _FlyPoints[i] + (_FlyPoints[i + 1] - _FlyPoints[i]) * progressRatio;

                        BirdLookAt(_FlyPoints[i + 1], rotRatio);
                        birdTr.position = nextPos;
                        yield return null;
                    }

                    currTime = 0;
                }
            }
            while (UseFlyLoop);
        }

        /**도착 후 파괴해야 한다면 파괴...*/
        if(DestroyAtArrived){

            Destroy(gameObject);
        }
        #endregion
    }

}
