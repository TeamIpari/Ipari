using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GamePadUIController;
using static InterativeUI;

/*********************************************************
 *   ���������� �Էµ� ��ġ�� ���� ������ UI�� �����մϴ�...
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

    private void ChangeSprite(InputDeviceType prevDevice, InputDeviceType changeDevice)
    {
        #region Omit
        if (TargetImage == null) return;

        /******************************************
         *    Ű������ ��츦 ó���Ѵ�....
         * ******/
        if (changeDevice == InputDeviceType.Keyboard && ButtonFSprite != null){

            TargetImage.sprite = ButtonFSprite;
            return;
        }

        /******************************************
         *   �����е��� ��츦 ó���Ѵ�....
         * ****/
        if (changeDevice != InputDeviceType.GamePad) return;

        GamePadKind padKind = GamePadUIController.LastInputGamePadKind;
        switch (padKind)
        {

            /**XInput�� ����� ���...*/
            case (GamePadKind.Unknown):
            case (GamePadKind.XBox):
                {
                    if (ButtonXSprite == null) break;
                    TargetImage.sprite = ButtonXSprite;
                    break;
                }

            /**����ũ/������ ����� ���...*/
            case (GamePadKind.PS):
                {
                    if (ButtonSquareSprite == null) break;
                    TargetImage.sprite = ButtonSquareSprite;
                    break;
                }

            /**���ٵ� �������� ����� ���...*/
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
