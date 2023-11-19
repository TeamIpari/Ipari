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
 *   ���������� �Էµ� ��ġ�� ���� ������ UI�� �����մϴ�...
 * ****/
public class GamePadInputSpriteChanger : MonoBehaviour
{
    #region Editor_Extension
    /**********************************************
     *   Editor Extension�� ���� private class....
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

            /**��������� �ִٸ� �����Ѵ�....*/
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
             *   �ش� �÷��װ� ��ȿ�ϸ� �븮�ڸ� ǥ���Ѵ�.....
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
             *   ������ ��ư�� ǥ���Ѵ�....
             * ***/
            using (var scope = new EditorGUI.ChangeCheckScope()){

                GUI.backgroundColor = (eventIsUsed ? Color.red : Color.green);
                {
                    eventIsUsed = GUILayout.Toggle(eventIsUsed, eventName, toggleOptions[(int)style]);
                }
                GUI.backgroundColor = returnBgColor;

                /**���� �ٲ���� ��� �����Ѵ�...*/
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
             *   ��� �̺�Ʈ���� ǥ���Ѵ�....
             * *******/

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUI_ShowUseEventSelectToggle(0, "OnChangedKeyboard", ToggleStyle.Left);
                GUI_ShowUseEventSelectToggle(1, "OnChangedGamePad", ToggleStyle.Right);
            }

            /**����Ϸ��� �̺�Ʈ���� ǥ���Ѵ�....*/
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
         *   ��ġ�� �ٲ���� ��, ���� ��ġ�� �˸°� UI�� �����Ѵ�...
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
         *   ��������Ʈ ��ȯ�� ����Ѵ�......
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
         *   ��������Ʈ ��ȯ ���� �̺�Ʈ�� ����� ���....
         * ****/
        bool isKeyboard = (prevDevice == InputDeviceType.Keyboard && changeDevice == InputDeviceType.Mouse)
                          || (changeDevice == InputDeviceType.Keyboard);

        /**Ű����� ��ȯ�Ǿ��� ���.....*/
        if (isKeyboard)
        {
            OnChangedKeyboard?.Invoke();
            return;
        }


        bool isGamePad = (prevDevice == InputDeviceType.GamePad && changeDevice == InputDeviceType.Mouse)
                         || (changeDevice == InputDeviceType.GamePad);

        /**�����е�� ��ȯ�Ǿ��� ���.....*/
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
         *    Ű������ ��츦 ó���Ѵ�....
         * ******/
        bool isKeyboard = (prevDevice==InputDeviceType.Keyboard && changeDevice==InputDeviceType.Mouse)
                          || (changeDevice == InputDeviceType.Keyboard);

        if (isKeyboard && data.KeyboardSprite != null){

            /**������ �̹����� ��ȿ�ϴٸ�....*/
            if(isValid && data.KeyboardSprite != null)
            {
                data.TargetImage.sprite = data.KeyboardSprite;
            }

            OnChangedKeyboard?.Invoke();
            return;
        }

        /******************************************
         *   �����е��� ��츦 ó���Ѵ�....
         * ****/
        bool isGamePad = (prevDevice == InputDeviceType.GamePad && changeDevice == InputDeviceType.Mouse)
                         || (changeDevice == InputDeviceType.GamePad);

        OnChangedGamePad?.Invoke();
        if (!isGamePad || !isValid) return;

        /**������ �̹����� ��ȿ�ϴٸ�....*/
        GamePadKind padKind = GamePadUIController.LastInputGamePadKind;
        switch (padKind){

            /**XInput�� ����� ���...*/
            case (GamePadKind.Unknown):
            case (GamePadKind.XBox):
                {
                    if (data.XboxSprite == null) break;
                    data.TargetImage.sprite = data.XboxSprite;
                    break;
                }

            /**����ũ/������ ����� ���...*/
            case (GamePadKind.PS):
                {
                    if (data.PSSprite == null) break;
                    data.TargetImage.sprite = data.PSSprite;
                    break;
                }

            /**���ٵ� �������� ����� ���...*/
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
