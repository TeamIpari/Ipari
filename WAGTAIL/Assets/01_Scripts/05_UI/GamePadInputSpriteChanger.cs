using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GamePadUIController;
using static InterativeUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*********************************************************
 *   마지막으로 입력된 장치에 따라서 지정한 UI를 변경합니다...
 * ****/
public class GamePadInputSpriteChanger : MonoBehaviour
{
    #region Editor_Extension
    /**********************************************
     *   Editor Extension을 위한 private class....
     * *******/
#if UNITY_EDITOR
    [CustomEditor(typeof(GamePadInputSpriteChanger))]
    private sealed class GamePadInputSpriteChangerEditor : Editor
    {
        private enum ToggleStyle
        {
            Left,
            Middle,
            Right
        }

        //===============================================
        //////               Fields                //////
        //==============================================
        private SerializedProperty OnChangedKeyboardProperty;
        private SerializedProperty OnChangedGamePadProperty;
        private SerializedProperty EventFlagsProperty;
        private GUIStyle[]         toggleOptions;



        //==================================================
        ///////            Override methods         ////////
        //==================================================
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            GUI_Initialized();

            GUI_ShowEvents();

            /**변경사항이 있다면 갱신한다....*/
            if(GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
        }



        //=================================================
        ///////              GUI methods            ///////
        //=================================================
        private void GUI_Initialized()
        {
            #region Omit
            if(OnChangedKeyboardProperty==null){

                OnChangedKeyboardProperty = serializedObject.FindProperty("OnChangedKeyboard");
            }

            if(OnChangedGamePadProperty==null){

                OnChangedGamePadProperty = serializedObject.FindProperty("OnChangedGamePad");
            }

            if(EventFlagsProperty==null){

                EventFlagsProperty = serializedObject.FindProperty("_UsedEventFlags");
            }

            if (toggleOptions == null){

                toggleOptions = new GUIStyle[]
                {
                    EditorStyles.miniButtonLeft,
                    EditorStyles.miniButtonMid,
                    EditorStyles.miniButtonRight
                };
            }

            #endregion
        }

        private void GUI_ShowEvent(int index, SerializedProperty property)
        {
            #region Omit
            if (EventFlagsProperty == null || property == null) return;

            /************************************************
             *   해당 플래그가 유효하면 대리자를 표시한다.....
             * *****/
            int flags = EventFlagsProperty.intValue;
            if ((flags & (1 << index)) != 0)
            {
                EditorGUILayout.PropertyField(property);
            }

            #endregion
        }

        private void GUI_ShowUseEventSelectToggle(int eventIndex, string eventName, ToggleStyle style = ToggleStyle.Middle)
        {
            #region Omit
            if (EventFlagsProperty == null) return;

            int layer           = (1 << eventIndex);
            bool eventIsUsed    = (EventFlagsProperty.intValue & layer) != 0;
            Color returnBgColor = GUI.backgroundColor;

            /****************************************************
             *   지정한 버튼을 표시한다....
             * ***/
            using (var scope = new EditorGUI.ChangeCheckScope()){

                GUI.backgroundColor = (eventIsUsed ? Color.red : Color.green);
                {
                    eventIsUsed = GUILayout.Toggle(eventIsUsed, eventName, toggleOptions[(int)style]);
                }
                GUI.backgroundColor = returnBgColor;

                /**값이 바뀌었을 경우 갱신한다...*/
                if (scope.changed)
                {
                    if (eventIsUsed) EventFlagsProperty.intValue |= layer;
                    else EventFlagsProperty.intValue &= ~layer;
                }
            }
            #endregion
        }

        private void GUI_ShowEvents()
        {
            #region Omit
            /********************************************
             *   모든 이벤트들을 표시한다....
             * *******/

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(0, "OnChangedKeyboard", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(1, "OnChangedGamePad", ToggleStyle.Right);
            }

            /**사용하려는 이벤트들을 표시한다....*/
            GUI_ShowEvent(0, OnChangedKeyboardProperty);
            GUI_ShowEvent(1, OnChangedGamePadProperty);

            #endregion
        }
    }
#endif
    #endregion

    #region Define
    [System.Serializable]
    public struct ChangeData
    {
        [SerializeField] public Image TargetImage;
        [SerializeField] public Sprite KeyboardSprite;
        [SerializeField] public Sprite XboxSprite;
        [SerializeField] public Sprite NintendoSprite;
        [SerializeField] public Sprite PSSprite;
    }
    #endregion

    //=============================================
    /////               property               ////
    //=============================================
    [SerializeField] public ChangeData[] ChangeDatas;

    [SerializeField, HideInInspector] 
    public UnityEvent   OnChangedKeyboard;

    [SerializeField, HideInInspector] 
    public UnityEvent   OnChangedGamePad;

    [SerializeField, HideInInspector]
    private int        _UsedEventFlags = 0;



    //====================================================
    ///////             Virtual methods            ///////
    //===================================================
    protected virtual void OnChangerStart()
    {
        //Virtual methods....
    }

    protected virtual void OnChangerDestroy()
    {
        //Virtual methods...
    }



    //====================================================
    ////////             Magic methods             ///////
    //===================================================
    private void Start()
    {
        #region Omit
        /*******************************************************
         *   장치가 바뀌었을 때, 현재 장치에 알맞게 UI를 갱신한다...
         * ****/
        OnChangerStart();

        GamePadUIController.OnDeviceChange += ChangeSprite;
        ChangeSprite( GamePadUIController.LastInputDevice, GamePadUIController.LastInputDevice );
        #endregion
    }

    private void OnDestroy()
    {
        #region Omit
        GamePadUIController.OnDeviceChange -= ChangeSprite;

        OnChangerDestroy();
        #endregion
    }



    //====================================================
    ////////             Core methods             ///////
    //====================================================
    private void ChangeSprite(InputDeviceType prevDevice, InputDeviceType changeDevice)
    {
        #region Omit

        /***********************************************
         *   소프라이트 전환을 사용한다......
         * ****/
        if (ChangeDatas!=null && ChangeDatas.Length>0)
        {
            int  Count       = ChangeDatas.Length;
            for(int i=0; i<Count; i++){

                ApplyMainChange(prevDevice, changeDevice, ref ChangeDatas[i]); 
            }

            return;
        }

        /***********************************************
         *   소프라이트 전환 없이 이벤트만 사용할 경우....
         * ****/
        bool isKeyboard = (prevDevice == InputDeviceType.Keyboard && changeDevice == InputDeviceType.Mouse)
                          || (changeDevice == InputDeviceType.Keyboard);

        /**키보드로 전환되었을 경우.....*/
        if (isKeyboard)
        {
            OnChangedKeyboard?.Invoke();
            return;
        }


        bool isGamePad = (prevDevice == InputDeviceType.GamePad && changeDevice == InputDeviceType.Mouse)
                         || (changeDevice == InputDeviceType.GamePad);

        /**게임패드로 전환되었을 경우.....*/
        if (isGamePad)
        {
            OnChangedGamePad?.Invoke();
            return;
        }

        #endregion
    }

    private void ApplyMainChange(InputDeviceType prevDevice, InputDeviceType changeDevice, ref ChangeData data)
    {
        #region Omit

        bool isValid = (data.TargetImage!=null);

        /******************************************
         *    키보드의 경우를 처리한다....
         * ******/
        bool isKeyboard = (prevDevice==InputDeviceType.Keyboard && changeDevice==InputDeviceType.Mouse)
                          || (changeDevice == InputDeviceType.Keyboard);

        if (isKeyboard && data.KeyboardSprite != null){

            /**변경할 이미지가 유효하다면....*/
            if(isValid && data.KeyboardSprite != null)
            {
                data.TargetImage.sprite = data.KeyboardSprite;
            }

            OnChangedKeyboard?.Invoke();
            return;
        }

        /******************************************
         *   게임패드의 경우를 처리한다....
         * ****/
        bool isGamePad = (prevDevice == InputDeviceType.GamePad && changeDevice == InputDeviceType.Mouse)
                         || (changeDevice == InputDeviceType.GamePad);

        OnChangedGamePad?.Invoke();
        if (!isGamePad || !isValid) return;

        /**변경할 이미지가 유효하다면....*/
        GamePadKind padKind = GamePadUIController.LastInputGamePadKind;
        switch (padKind){

            /**XInput을 사용할 경우...*/
            case (GamePadKind.Unknown):
            case (GamePadKind.XBox):
                {
                    if (data.XboxSprite == null) break;
                    data.TargetImage.sprite = data.XboxSprite;
                    break;
                }

            /**듀얼쇼크/센스를 사용할 경우...*/
            case (GamePadKind.PS):
                {
                    if (data.PSSprite == null) break;
                    data.TargetImage.sprite = data.PSSprite;
                    break;
                }

            /**닌텐도 프로콘을 사용할 경우...*/
            case (GamePadKind.Nintendo):
                {
                    if (data.NintendoSprite == null) break;
                    data.TargetImage.sprite = data.NintendoSprite;
                    break;
                }

        }
        #endregion
    }
}
