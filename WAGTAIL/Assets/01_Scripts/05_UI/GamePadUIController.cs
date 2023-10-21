using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*******************************************************
 *   게임패드로 메뉴를 조작할 수 있도록 하는 컴포넌트입니다..
 * ****/
public sealed class GamePadUIController : MonoBehaviour
{
    #region Define
    [System.Serializable]
    public sealed class GamePadUIEvent : UnityEvent
    {
    }

    public enum InputDeviceType
    {
        Keyboard,
        GamePad
    }

    public enum GamePadInputType
    {
        Interaction,
        Jump
    }
    #endregion

    #region Editor_Extension
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GamePadUIController))]
    private sealed class GamePadUIControllerEditor : Editor
    {
        private enum ToggleStyle
        {
            Left,
            Middle,
            Right
        }

        //===========================================
        /////              Fields               /////
        //===========================================
        private SerializedProperty StartSelectProperty;
        private SerializedProperty UsedEventLayerProperty;
        private SerializedProperty UseMoveLockProperty;

        private SerializedProperty UpTargetProperty;
        private SerializedProperty DownTargetProperty;
        private SerializedProperty LeftTargetProperty;
        private SerializedProperty RightTargetProperty;
        private SerializedProperty CancelTargetProperty;
        private SerializedProperty SelectTargetProperty;

        private SerializedProperty OnSelectProperty;
        private SerializedProperty OnDisSelectProperty;
        private SerializedProperty OnPressedProperty;
        private SerializedProperty OnCancelProperty;
        private SerializedProperty OnDPadLeftProperty;
        private SerializedProperty OnDPadRightProperty;
        private SerializedProperty OnDPadUpProperty;
        private SerializedProperty OnDPadDownProperty;
        private SerializedProperty OnPressedLockProperty;
        private SerializedProperty OnCancelLockProperty;
        private SerializedProperty OnDPadLeftLockProperty;
        private SerializedProperty OnDPadRightLockProperty;
        private SerializedProperty OnDPadUpLockProperty;
        private SerializedProperty OnDPadDownLockProperty;



        private GUIStyle[] toggleOptions;


        //===============================================
        /////           Override methods            /////
        //===============================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI_Initialized();

            GUI_ShowStartSelectAndUseMoveLock();

            GUI_ShowMoveTargets();

            EditorGUILayout.Space(10f);

            GUI_ShowUseEventSelectToggles();

            GUI_ShowEvents();

            /**값이 변화하였다면 갱신한다....*/
            if (GUI.changed) {

                serializedObject.ApplyModifiedProperties();
            }
        }



        //=================================================
        /////               GUI methods               /////
        //=================================================
        private void GUI_Initialized()
        {
            #region Omit
            /***********************************
             *   모든 프로퍼티들을 초기화한다...
             * ***/
            if (StartSelectProperty == null) {

                StartSelectProperty = serializedObject.FindProperty("StartSelect");
            }

            if (UseMoveLockProperty == null) {

                UseMoveLockProperty = serializedObject.FindProperty("UseMoveLock");
            }

            if (UsedEventLayerProperty == null) {

                UsedEventLayerProperty = serializedObject.FindProperty("UsedEventLayer");
            }

            if (UpTargetProperty == null) {

                UpTargetProperty = serializedObject.FindProperty("UpTarget");
            }

            if (DownTargetProperty == null) {

                DownTargetProperty = serializedObject.FindProperty("DownTarget");
            }

            if (LeftTargetProperty == null) {

                LeftTargetProperty = serializedObject.FindProperty("LeftTarget");
            }

            if (RightTargetProperty == null) {

                RightTargetProperty = serializedObject.FindProperty("RightTarget");
            }

            if (CancelTargetProperty == null) {

                CancelTargetProperty = serializedObject.FindProperty("CancelTarget");
            }

            if (SelectTargetProperty == null) {

                SelectTargetProperty = serializedObject.FindProperty("SelectTarget");
            }

            if (OnSelectProperty == null) {

                OnSelectProperty = serializedObject.FindProperty("OnSelect");
            }

            if (OnDisSelectProperty == null) {

                OnDisSelectProperty = serializedObject.FindProperty("OnDisSelect");
            }

            if (OnPressedProperty == null) {

                OnPressedProperty = serializedObject.FindProperty("OnPressd");
            }

            if (OnCancelProperty == null) {

                OnCancelProperty = serializedObject.FindProperty("OnCancel");
            }

            if (OnDPadLeftProperty == null) {

                OnDPadLeftProperty = serializedObject.FindProperty("OnDPadLeft");
            }

            if (OnDPadRightProperty == null) {

                OnDPadRightProperty = serializedObject.FindProperty("OnDPadRight");
            }

            if (OnDPadUpProperty == null) {

                OnDPadUpProperty = serializedObject.FindProperty("OnDPadUp");
            }

            if (OnDPadDownProperty == null) {

                OnDPadDownProperty = serializedObject.FindProperty("OnDPadDown");
            }

            if (OnCancelLockProperty == null) {

                OnCancelLockProperty = serializedObject.FindProperty("OnCancelLock");
            }

            if (OnDPadLeftLockProperty == null) {

                OnDPadLeftLockProperty = serializedObject.FindProperty("OnDPadLeftLock");
            }

            if (OnDPadRightLockProperty == null) {

                OnDPadRightLockProperty = serializedObject.FindProperty("OnDPadRightLock");
            }

            if (OnDPadUpLockProperty == null) {

                OnDPadUpLockProperty = serializedObject.FindProperty("OnDPadUpLock");
            }

            if (OnDPadDownLockProperty == null) {

                OnDPadDownLockProperty = serializedObject.FindProperty("OnDPadDownLock");
            }

            if (OnPressedLockProperty == null)
            {
                OnPressedLockProperty = serializedObject.FindProperty("OnPressdLock");
            }

            if (toggleOptions == null) {

                toggleOptions = new GUIStyle[]
                {
                    EditorStyles.miniButtonLeft,
                    EditorStyles.miniButtonMid,
                    EditorStyles.miniButtonRight
                };
            }


            #endregion
        }

        private void GUI_ShowStartSelectAndUseMoveLock()
        {
            #region Omit

            if (StartSelectProperty == null) return;

            StartSelectProperty.boolValue = EditorGUILayout.ToggleLeft("Start Selected", StartSelectProperty.boolValue);
            UseMoveLockProperty.boolValue = EditorGUILayout.ToggleLeft("Use Move Lock", UseMoveLockProperty.boolValue);
            #endregion
        }

        private void GUI_ShowUseEventSelectToggles()
        {
            #region Omit
            if (UsedEventLayerProperty == null) return;

            /***********************************************
             *   선택 및 취소 관련...
             * ****/
            using (var hztScope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(0, "OnSelect", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(1, "OnDisselect");
                GUI_ShowUseEventSelectToggle(2, "OnPressed");
                GUI_ShowUseEventSelectToggle(3, "OnCancel", ToggleStyle.Right);
            }

            /***********************************************
             *   이동 관련...
             * ****/
            using (var hztScope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(4, "OnLeft", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(5, "OnRight");
                GUI_ShowUseEventSelectToggle(6, "OnUp");
                GUI_ShowUseEventSelectToggle(7, "OnDown", ToggleStyle.Right);
            }


            /************************************************
             *    이동 제한의 경우의 조작 관련...
             * ****/
            using (var hztScope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(8, "OnLeftLock", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(9, "OnRightLock");
                GUI_ShowUseEventSelectToggle(10, "OnUpLock", ToggleStyle.Right);
            }

            using (var hztScope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(11, "OnDownLock", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(12, "OnCancelLock");
                GUI_ShowUseEventSelectToggle(13, "OnPressedLock", ToggleStyle.Right);
            }


            #endregion
        }

        private void GUI_ShowUseEventSelectToggle(int eventIndex, string eventName, ToggleStyle style = ToggleStyle.Middle)
        {
            #region Omit
            if (UsedEventLayerProperty == null || toggleOptions == null) return;
            int layer = (1 << eventIndex);
            bool eventIsUsed = (UsedEventLayerProperty.intValue & layer) != 0;
            Color returnBgColor = GUI.backgroundColor;

            /****************************************************
             *   지정한 버튼을 표시한다....
             * ***/
            using (var scope = new EditorGUI.ChangeCheckScope()) {

                GUI.backgroundColor = (eventIsUsed ? Color.red : Color.green);
                {
                    eventIsUsed = GUILayout.Toggle(eventIsUsed, eventName, toggleOptions[(int)style]);
                }
                GUI.backgroundColor = returnBgColor;

                /**값이 바뀌었을 경우 갱신한다...*/
                if (scope.changed)
                {
                    if (eventIsUsed) UsedEventLayerProperty.intValue |= layer;
                    else UsedEventLayerProperty.intValue &= ~layer;
                }
            }
            #endregion
        }

        private void GUI_ShowMoveTargets()
        {
            #region Omit
            UpTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Up Move Target", UpTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);
            DownTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Down Move Target", DownTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);
            LeftTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Left Move Target", LeftTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);
            RightTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Right Move Target", RightTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);

            EditorGUILayout.Space(5f);

            CancelTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Cancle Move Target", CancelTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);
            SelectTargetProperty.objectReferenceValue = EditorGUILayout.ObjectField("Select Move Target", SelectTargetProperty.objectReferenceValue, typeof(GamePadUIController), true);
            #endregion
        }

        private void GUI_ShowEvents()
        {
            #region Omit

            GUI_ShowEvent(0, OnSelectProperty);
            GUI_ShowEvent(1, OnDisSelectProperty);
            GUI_ShowEvent(2, OnPressedProperty);
            GUI_ShowEvent(3, OnCancelProperty);
            GUI_ShowEvent(4, OnDPadLeftProperty);
            GUI_ShowEvent(5, OnDPadRightProperty);
            GUI_ShowEvent(6, OnDPadUpProperty);
            GUI_ShowEvent(7, OnDPadDownProperty);
            GUI_ShowEvent(8, OnDPadLeftLockProperty);
            GUI_ShowEvent(9, OnDPadRightLockProperty);
            GUI_ShowEvent(10, OnDPadUpLockProperty);
            GUI_ShowEvent(11, OnDPadDownLockProperty);
            GUI_ShowEvent(12, OnCancelLockProperty);
            GUI_ShowEvent(13, OnPressedLockProperty);

            #endregion
        }

        private void GUI_ShowEvent(int eventIndex, SerializedProperty eventProperty)
        {
            #region Omit
            if (UsedEventLayerProperty == null || eventProperty == null) return;
            int layer = (1 << eventIndex);
            bool eventIsUsed = (UsedEventLayerProperty.intValue & layer) != 0;

            if (eventIsUsed) EditorGUILayout.PropertyField(eventProperty);

            #endregion
        }

    }
#endif
    #endregion

    //=======================================================
    //////            Property and fields               /////
    //=======================================================
    public static GamePadUIController Current         { get; set; }
    public InputDeviceType            LastInputDevice { get; private set; } = InputDeviceType.Keyboard;

    [SerializeField] public GamePadUIController UpTarget;
    [SerializeField] public GamePadUIController DownTarget;
    [SerializeField] public GamePadUIController LeftTarget;
    [SerializeField] public GamePadUIController RightTarget;
    [SerializeField] public GamePadUIController CancelTarget;
    [SerializeField] public GamePadUIController SelectTarget;

    [SerializeField] public GamePadUIEvent OnSelect;
    [SerializeField] public GamePadUIEvent OnDisSelect;
    [SerializeField] public GamePadUIEvent OnPressd;
    [SerializeField] public GamePadUIEvent OnCancel;
    [SerializeField] public GamePadUIEvent OnDPadLeft;
    [SerializeField] public GamePadUIEvent OnDPadRight;
    [SerializeField] public GamePadUIEvent OnDPadUp;
    [SerializeField] public GamePadUIEvent OnDPadDown;
    [SerializeField] public GamePadUIEvent OnPressdLock;
    [SerializeField] public GamePadUIEvent OnDPadLeftLock;
    [SerializeField] public GamePadUIEvent OnDPadRightLock;
    [SerializeField] public GamePadUIEvent OnDPadUpLock;
    [SerializeField] public GamePadUIEvent OnDPadDownLock;
    [SerializeField] public GamePadUIEvent OnCancelLock;

    [SerializeField] public bool StartSelect = false;
    [SerializeField] public bool UseMoveLock = false;
    [SerializeField] private int UsedEventLayer = 0;


    private static Coroutine UICoroutine;


    //========================================================
    //////                 Magic methods                //////
    //=========================================================
    private void Start()
    {
        #region Omit
        /**시작지점으로 선택된 개체를 갱신한다... */
        if (StartSelect)
        {
            Current = this;
        }

        /**메인 코루틴을 실행한다...*/
        UIManager manager = UIManager.GetInstance();
        if (manager != null && UICoroutine == null)
        {
            UICoroutine = manager.StartCoroutine(PadInputProgress());
        }
        #endregion
    }

    private void OnDestroy()
    {
        #region Omit
        UIManager manager = UIManager.GetInstance();
        if (manager != null && Current == this)
        {
            Current = null;
            manager.StopCoroutine(UICoroutine);
            UICoroutine = null;
        }
        #endregion
    }



    //====================================================
    //////               Core methods               //////
    //====================================================
    public void ClearCurrent()
    {
        Current = null;
    }

    public void ChangeCurrentThis()
    {
        Current = this;
    }

    public void SetMoveLock(bool isLock)
    {
        #region Omit
        if (Current == null) return;
        Current.UseMoveLock = isLock;
        #endregion
    }

    private void SelectUI(Vector2 dir)
    {
        #region Omit
        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.UI_Button,
            Vector3.zero,
            2f
        );

        /**좌측 클릭*/
        if (dir.x<0 ){

            if(Current.UseMoveLock) Current.OnDPadLeftLock?.Invoke();
            else Current.OnDPadLeft?.Invoke();

            if (Current.LeftTarget != null && !Current.UseMoveLock)
            {
                Current.OnDisSelect?.Invoke();
                Current = Current.LeftTarget;
                Current.OnSelect?.Invoke();
            }
        }

        /**우측 클릭*/
        else if (dir.x > 0){

            if (Current.UseMoveLock) Current.OnDPadRightLock?.Invoke();
            else Current.OnDPadRight?.Invoke();

            if(Current.RightTarget != null && !Current.UseMoveLock)
            {
                Current.OnDisSelect?.Invoke();
                Current = Current.RightTarget;
                Current.OnSelect?.Invoke();
            }
        }

        /**상측 클릭*/
        else if (dir.y > 0){

            if (Current.UseMoveLock) Current.OnDPadUpLock?.Invoke();
            else Current.OnDPadUp?.Invoke();

            if(Current.UpTarget != null && !Current.UseMoveLock)
            {
                Current.OnDisSelect?.Invoke();
                Current = Current.UpTarget;
                Current.OnSelect?.Invoke();
            }
        }

        /**하측 클릭*/
        else if (dir.y < 0){

            if (Current.UseMoveLock) Current.OnDPadDownLock?.Invoke();
            else Current.OnDPadDown?.Invoke();

            if(Current.DownTarget != null && !Current.UseMoveLock)
            {
                Current.OnDisSelect?.Invoke();
                Current = Current.DownTarget;
                Current.OnSelect?.Invoke();
            }
        }

        /**자기자신이 선택될 경우...*/
        else if(Current==this){

            Current.OnSelect?.Invoke();
        }
        #endregion
    }

    private bool ButtonIsDown(Gamepad currPad, GamePadInputType type)
    {
        #region Omit
        string padName = currPad.name;

        /**Xbox Controller의 경우...*/
        if(padName.ToLower().Contains("xbox"))
        {
           if(type==GamePadInputType.Interaction) return (currPad.xButton.value!=0f);
           else return (currPad.aButton.value!=0f);
        }

        /**듀얼쇼크의 경우...*/
        if (padName.ToLower().Contains("playstation"))
        {
            if (type == GamePadInputType.Interaction) return (currPad.squareButton.value!= 0f);
            else return (currPad.crossButton.value != 0f);
        }

        /**닌텐도 Controller의 경우...*/
        if (padName.ToLower().Contains("Nintendo"))
        {
            if (type == GamePadInputType.Interaction) return (currPad.yButton.value != 0f);
            else return (currPad.aButton.value != 0f);
        }

        if (type == GamePadInputType.Interaction) return (currPad.bButton.value != 0f);
        else return (currPad.aButton.value != 0f);
        #endregion
    }

    private IEnumerator PadInputProgress()
    {
        #region Omit
        /************************************************
         *    게임패드의 DPad 입력을 받고, UI를 움직인다...
         * ****/
        Vector2 lastInput = Vector2.zero;
        Vector2 dpadDir   = Vector2.zero;
        bool lastOk     = false;
        bool lastCancel = false;

        while (true)
        {
            /***********************************************************
             *   현재 연결된 게임패드 또는 선택된 UI가 존재하는지 체크한다...
             * ****/
            Gamepad currPad = Gamepad.current;
            if(currPad==null || Current == null)
            {
                yield return null;
                continue;
            }


            /***************************************************
             *   현재 입력된 DPad값에 따라 연결된 메뉴로 이동한다...
             * ****/
            if ((dpadDir = currPad.dpad.value) != Vector2.zero && lastInput == Vector2.zero){

                SelectUI(dpadDir);
            }
            lastInput = dpadDir;


            /****************************************************
             *   확인 버튼을 눌렀을 때의 처리를 한다....
             * ****/
            bool pressOkBtn = ButtonIsDown(currPad, GamePadInputType.Jump);
            if(Current != null && pressOkBtn && lastOk==false){

                bool lastMoveLock = Current.UseMoveLock;

                if (Current.UseMoveLock) Current.OnPressdLock?.Invoke();
                else Current.OnPressd?.Invoke();

                if (Current != null && Current.SelectTarget != null && !lastMoveLock)
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.UI_Button,
                        Vector3.zero,
                        2f
                    );

                   Current.OnDisSelect?.Invoke();
                   Current = Current.SelectTarget;
                   Current.OnSelect?.Invoke();
                }
            }
            lastOk = pressOkBtn;

            

            /****************************************************
             *    취소 버튼을 눌렀을 때의 처리를 한다....
             * ****/
            bool pressCancelBtn = ButtonIsDown(currPad, GamePadInputType.Interaction);
            if (Current != null && pressCancelBtn && lastCancel==false){

                bool lastMoveLock = Current.UseMoveLock;

                if (Current.UseMoveLock) Current.OnCancelLock?.Invoke();
                else Current.OnCancel?.Invoke();

                if(Current != null && Current.CancelTarget!=null && !lastMoveLock)
                {
                    FModAudioManager.PlayOneShotSFX(
                        FModSFXEventType.UI_Button,
                        Vector3.zero,
                        2f
                    );

                    Current.OnDisSelect?.Invoke();
                    Current = Current.CancelTarget;
                    Current.OnSelect?.Invoke();
                }
            }
            lastCancel = pressCancelBtn;

            yield return null;
        }
        #endregion
    }
}
