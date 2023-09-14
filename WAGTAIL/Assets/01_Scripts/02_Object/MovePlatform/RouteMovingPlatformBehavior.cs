using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using IPariUtility;

#if UNITY_EDITOR
using UnityEditor;
#endif

/**********************************************************
 *   ������ ������ ��η� �̵��ϴ� �������� ���ǵ� ������Ʈ�Դϴ�. 
 * ***/
[AddComponentMenu("PlatformObject/RouteMovingPlatformBehavior")]
public sealed class RouteMovingPlatformBehavior : PlatformBehaviorBase
{
    #region Editor_Extension
#if UNITY_EDITOR
    /******************************************
     *   ������ Ȯ���� ���� private class...
     * ***/
    [CustomEditor(typeof(RouteMovingPlatformBehavior))]
    private sealed class RouteMovingPlatformBehaviorEditor : Editor
    {
        //======================================
        /////           Fields              ////
        //======================================
        private RouteMovingPlatformBehavior behavior;
        private Transform                   behaviorTransform;
        private SerializedProperty          MovingPointsProperty;
        private SerializedProperty          MovingPointsCountProperty;
        private SerializedProperty          MovingTypeProperty;
        private SerializedProperty          ApplyTimingProperty;
        private SerializedProperty          LoopCountProperty;
        private SerializedProperty          MovingDurationProperty;
        private SerializedProperty          UsedChangeDirProperty;

        private static readonly string[] MovingTypes = new string[] { "Default", "Used Bezier" };
        private bool _loopInfinity = false;


        //=====================================================
        /////       Override and magic methods             ////
        //=====================================================
        private void OnEnable()
        {
            #region Omit
            GUI_Initialized();

            if(LoopCountProperty!=null)
            {
                _loopInfinity = (LoopCountProperty.intValue < 0);
            }
            #endregion
        }

        private void OnSceneGUI()
        {
            #region Omit
            if (MovingPointsProperty == null) return;

            serializedObject.Update();

            /**************************************
             *   ��� ���� ǥ���Ѵ�....
             * ***/
            Handles.color = Color.green;
            int Count = MovingPointsProperty.arraySize;

            for (int i = 0; i < Count; i++)
            {
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {

                    SerializedProperty element = MovingPointsProperty.GetArrayElementAtIndex(i);
                    Vector3 point = element.vector3Value;
                    point = Handles.PositionHandle(point, Quaternion.identity);

                    /**���� ����Ǿ��ٸ� �����Ѵ�...*/
                    if (changeScope.changed)
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
            GUI_Initialized();

            serializedObject.Update();

            /********************************
             *   ��� ������Ƽ���� ǥ���Ѵ�...
             * ***/
            GUI_ShowApplyTiming();

            GUI_ShowMovingDuration();

            GUI_ShowLoopCount();

            GUI_ShowUsedChangeDir();

            EditorGUILayout.Space(10f);

            GUI_ShowMovingType();

            GUI_ShowMovingPoints();

            /**���� �����Ұ� �ִٸ� ����...*/
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
            if(behavior==null){

                behavior = (target as RouteMovingPlatformBehavior);
                if(behavior!=null) behaviorTransform = behavior.transform;
            }

            if(MovingPointsProperty==null){

                MovingPointsProperty = serializedObject.FindProperty("_MovingPoints");
            }

            if(MovingPointsCountProperty==null) {

                MovingPointsCountProperty = serializedObject.FindProperty ("_MovingPointsCount");
            }

            if(MovingTypeProperty==null){

                MovingTypeProperty = serializedObject.FindProperty("MovingType");
            }

            if(ApplyTimingProperty==null){

                ApplyTimingProperty = serializedObject.FindProperty("ApplyTiming");
            }

            if(MovingDurationProperty==null){

                MovingDurationProperty = serializedObject.FindProperty("MovingDuration");
            }

            if(LoopCountProperty==null){

                LoopCountProperty = serializedObject.FindProperty("LoopCount");
            }

            if(UsedChangeDirProperty==null){

                UsedChangeDirProperty = serializedObject.FindProperty("UsedChangeDirAtComplete");
            }

            #endregion
        }

        private void GUI_ShowMovingType()
        {
            #region Omit
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {

                int selected = GUILayout.Toolbar(MovingTypeProperty.intValue, MovingTypes);
                if (changeScope.changed)
                {
                    MovingTypeProperty.intValue = selected;
                }
            }
            #endregion
        }

        private void GUI_ShowMovingPoints()
        {
            #region Omit
            if (MovingPointsProperty == null || MovingPointsCountProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {

                EditorGUILayout.PropertyField(MovingPointsProperty, true);

                int prevLen = MovingPointsCountProperty.intValue;
                int currLen = MovingPointsProperty.arraySize;

                /**���� ����Ǿ��� ���...*/
                if (scope.changed)
                {
                    MovingPointsCountProperty.intValue = currLen;

                    /*�߰��� ����Ʈ�� ���� ������ �Ǵ� ���� ��ġ��...*/
                    if ((behaviorTransform != null) && (currLen > prevLen))
                    {

                        SerializedProperty lastElement = MovingPointsProperty.GetArrayElementAtIndex(currLen - 1);

                        /**ù��° ������ ���...*/
                        if (currLen == 1) lastElement.vector3Value = behaviorTransform.position;

                        /**������ ������ ���...*/
                        else
                        {

                            SerializedProperty prevElement = MovingPointsProperty.GetArrayElementAtIndex(currLen - 2);
                            lastElement.vector3Value = prevElement.vector3Value + (Vector3.up * 1.2f);
                        }
                    }
                }

                scope.Dispose();
            }
            #endregion
        }

        private void GUI_ShowApplyTiming()
        {
            #region Omit
            if (ApplyTimingProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                PlatformApplyTiming timing = ApplyTimingProperty.GetEnumValue<PlatformApplyTiming>();
                System.Enum result = EditorGUILayout.EnumFlagsField("Apply Timing",timing);

                /**���� �ٲ���ٸ� ����.*/
                if(scope.changed){

                    ApplyTimingProperty.SetEnumValue(result);
                }
            }
            #endregion
        }

        private void GUI_ShowMovingDuration()
        {
            #region Omit
            if (MovingDurationProperty == null) return;

            using (var scope = new EditorGUI.ChangeCheckScope()) {

                float value = EditorGUILayout.FloatField("Moving Durataion", MovingDurationProperty.floatValue);
                /**���� ����Ǿ��� ��� ����...*/
                if(scope.changed)
                {
                    if (value <= 0f) value = 0f;
                    MovingDurationProperty.floatValue = value;
                }
            }

            #endregion
        }

        private void GUI_ShowLoopCount()
        {
            #region Omit
            if (LoopCountProperty == null) return;

            EditorGUILayout.BeginHorizontal();
            {
                using(var disableScope = new EditorGUI.DisabledGroupScope(LoopCountProperty.intValue<0)){

                    LoopCountProperty.intValue = EditorGUILayout.IntField("Loop Count", LoopCountProperty.intValue);
                }

                using (var changedScope = new EditorGUI.ChangeCheckScope()){

                    _loopInfinity = EditorGUILayout.Toggle("Infinity", _loopInfinity);

                    if(changedScope.changed)
                    {
                        LoopCountProperty.intValue = (_loopInfinity ? -1 : 0);
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        private void GUI_ShowUsedChangeDir()
        {
            #region Omit
            if (UsedChangeDirProperty == null) return;

            using(var scope = new EditorGUI.ChangeCheckScope()){

                bool value = EditorGUILayout.Toggle("ChangeDirAtComplete", UsedChangeDirProperty.boolValue);
                if(scope.changed)
                {
                    UsedChangeDirProperty.boolValue = value;
                }
            }
            #endregion
        }



    }

#endif
    #endregion

    public enum RouteMovingType
    {
        Default,
        Bezeir
    }

    //==================================
    ////         Property           ////
    //==================================
    public Vector3[] FlyPoints { get { return _MovingPoints; } }

    [SerializeField] public PlatformApplyTiming ApplyTiming    = PlatformApplyTiming.None;
    [SerializeField] public RouteMovingType     MovingType     = RouteMovingType.Default;
    [SerializeField] public int                 LoopCount      = 1;
    [SerializeField] public float               MovingDuration = 1.5f;
    [SerializeField] public bool                UsedChangeDirAtComplete = false;



    //==================================
    ////          Fields           ////
    //==================================
    [HideInInspector] [SerializeField]
    private Vector3[]   _MovingPoints;

    [HideInInspector][SerializeField]
    private int         _MovingPointsCount = 0;

    private int   MovCount      = 0;
    private int   MovCurr       = 0;
    private int   LoopLeft = 0;
    private float currTime      = 0f;
    private float cycleGoalTime = 0f;
    private int   MovingDir     = 1;
    private float goalTimeDiv   = 0f;
    private Vector3 movOffset   = Vector3.zero;
    private Vector3 prevPos     = Vector3.zero;



    //======================================
    /////       Override methods        ////
    //======================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.BehaviorStart);
    }

    public override void BehaviorEnd(PlatformObject changedTarget)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.BehaviorEnd);
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.OnObjectEnter);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.OnObjectStay);

        //if(standingBody==null) standingTarget.transform.position += movOffset;
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.OnObjectExit);
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        CheckTimingAndExecMoving(PlatformApplyTiming.PhysicsUpdate);

        #region Omit
        /*********************************************
         *   ������ ������ ���� ��, ������ �̵��� �����Ѵ�.
         * ***/
        if (MovCurr < MovCount)
        {
            currTime += Time.deltaTime;

            int real;
            float progressRatio = Mathf.Clamp((currTime * goalTimeDiv), 0f, 1f);
            Vector3 nextPos;

            /**������ � �������� ó��...*/
            if (MovingType==RouteMovingType.Bezeir)
            {
                real = (MovingDir == 1 ? MovCurr * 2 : (MovCount * 2) - (MovCurr * 2));

                nextPos = IpariUtility.GetBezier(
                    ref _MovingPoints[real],
                    ref _MovingPoints[real + (1 * MovingDir)],
                    ref _MovingPoints[real + (2 * MovingDir)],
                    progressRatio
                );
            }
            /**�⺻ �������� ó��...*/
            else
            {
                real    = (MovingDir == 1 ? MovCurr : MovCount - MovCurr);
                nextPos = _MovingPoints[real] + (_MovingPoints[real + (1 * MovingDir)] - _MovingPoints[real]) * progressRatio;
            }

            movOffset = (nextPos - prevPos);
            affectedPlatform.UpdatePosition += movOffset;

            prevPos = nextPos;

            /**�� ���� ������ �������Ǿ��� ���...*/
            if (currTime >= cycleGoalTime){

                MovCurr++;
                currTime -= cycleGoalTime;
            }
        }

        /**�ݺ�Ƚ���� �����ִٸ� �ݺ�...*/
        else if (LoopLeft != 0)
        {
            LoopLeft--;
            MovCurr = 0;

            /**������ �ڹٲ۴�...*/
            if (UsedChangeDirAtComplete){

                MovingDir *= -1;
            }

        }
        else movOffset = Vector3.zero;
        #endregion
    }


    //=====================================================
    /////         Magic and Utility methods            ////
    //=====================================================
    private void CheckTimingAndExecMoving(PlatformApplyTiming execTiming, bool pass=false)
    {
        #region Omit
        /************************************
         *   �ش� Ÿ�̹��� ����Ѵٸ� �̵��� �����Ѵ�..
         * ***/
        bool timingIsContain = ((int)ApplyTiming & (int)execTiming) > 0;
        bool isMoving        = (LoopLeft!=0);
        bool isNotValid      = (MovingDuration <= 0 || _MovingPointsCount==0);

        if (timingIsContain && !isMoving && !isNotValid)
        {
            prevPos = _MovingPoints[0];
            LoopLeft = LoopCount;

            /**������ ��� ����� ���*/
            if (MovingType==RouteMovingType.Bezeir){

                MovCount    = (_MovingPointsCount - 1) / 2;
                MovCurr     = 0;

                cycleGoalTime   = (MovingDuration / MovCount);
                goalTimeDiv     = (1f / cycleGoalTime);
                currTime        = 0f;
            }

            /**�⺻��...*/
            else{

                MovCount        = (_MovingPointsCount - 1);
                MovCurr         = 0;
                cycleGoalTime   = (MovingDuration / MovCount);
                goalTimeDiv     = (1f / cycleGoalTime);
                currTime        = 0f;
            }
        }
        #endregion
    }

    private void OnDrawGizmosSelected()
    {
        #region Omit
#if UNITY_EDITOR
        if (_MovingPoints == null || _MovingPointsCount == 0) return;

        /***********************************
         *   ���� �������� ��� ǥ���Ѵ�..
         * ***/
        if (MovingType == 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(_MovingPoints[0], .2f);

            Gizmos.color = Color.blue;
            for (int i = 1; i < _MovingPointsCount; i++)
            {

                Gizmos.DrawSphere(_MovingPoints[i], .2f);
                Gizmos.DrawLine(_MovingPoints[i - 1], _MovingPoints[i]);
            }

            return;
        }

        /***************************************
         *   ���� �������� ������ ����� ǥ���Ѵ�.
         * ***/
        int j = 1;
        int goal = (_MovingPointsCount - 1);

        Gizmos.color = Color.yellow;
        for (; j < goal; j += 2){

            Gizmos.DrawSphere(_MovingPoints[j - 1], .2f);
            Gizmos.DrawSphere(_MovingPoints[j], .2f);
            Gizmos.DrawSphere(_MovingPoints[j + 1], .2f);

            Gizmos.color = Color.black;
            Gizmos.DrawLine(_MovingPoints[j - 1], _MovingPoints[j]);
            Gizmos.DrawLine(_MovingPoints[j], _MovingPoints[j + 1]);

            /**������ ��� �׸���..*/
            Vector3 prev = _MovingPoints[j - 1];
            Gizmos.color = Color.yellow;

            for (float w = 0f; w < 1f; w += .15f)
            {
                Vector3 curr = IpariUtility.GetBezier(ref _MovingPoints[j - 1], ref _MovingPoints[j], ref _MovingPoints[j + 1], w);
                Gizmos.DrawLine(prev, curr);
                prev = curr;
            }

            Gizmos.color = Color.yellow;
        }

        /**���׸� �������� ��� �׸���.*/
        for (int k = (j - 1); k < _MovingPointsCount; k++) {

            Gizmos.DrawSphere(_MovingPoints[k], .2f);
        }
#endif
        #endregion
    }

}
