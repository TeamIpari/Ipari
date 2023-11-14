using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GamePadUIController;
using static InterativeUI;

/*********************************************************
 *   마지막으로 입력된 장치에 따라서 지정한 UI를 변경합니다...
 * ****/
public class GamePadInputSpriteChanger : MonoBehaviour
{
    //=============================================
    /////               property               ////
    //=============================================
    [SerializeField] public Sprite ButtonFSprite;
    [SerializeField] public Sprite ButtonXSprite;
    [SerializeField] public Sprite ButtonYSprite;
    [SerializeField] public Sprite ButtonSquareSprite;
    [SerializeField] public Image TargetImage;



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
    ////////             Core methods             ///////
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

    private void ChangeSprite(InputDeviceType prevDevice, InputDeviceType changeDevice)
    {
        #region Omit
        if (TargetImage == null) return;

        /******************************************
         *    키보드의 경우를 처리한다....
         * ******/
        if (changeDevice == InputDeviceType.Keyboard && ButtonFSprite != null){

            TargetImage.sprite = ButtonFSprite;
            return;
        }

        /******************************************
         *   게임패드의 경우를 처리한다....
         * ****/
        if (changeDevice != InputDeviceType.GamePad) return;

        GamePadKind padKind = GamePadUIController.LastInputGamePadKind;
        switch (padKind)
        {

            /**XInput을 사용할 경우...*/
            case (GamePadKind.Unknown):
            case (GamePadKind.XBox):
                {
                    if (ButtonXSprite == null) break;
                    TargetImage.sprite = ButtonXSprite;
                    break;
                }

            /**듀얼쇼크/센스를 사용할 경우...*/
            case (GamePadKind.PS):
                {
                    if (ButtonSquareSprite == null) break;
                    TargetImage.sprite = ButtonSquareSprite;
                    break;
                }

            /**닌텐도 프로콘을 사용할 경우...*/
            case (GamePadKind.Nintendo):
                {
                    if (ButtonYSprite == null) break;
                    TargetImage.sprite = ButtonYSprite;
                    break;
                }

        }
        #endregion
    }

}
