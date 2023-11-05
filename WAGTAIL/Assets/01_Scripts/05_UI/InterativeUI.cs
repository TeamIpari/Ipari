using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GamePadUIController;

/**********************************************************
 *  ��ȣ�ۿ뿡 �˸´� UI�� ��µǵǷ� �ϴ� ������Ʈ�Դϴ�.
 * ***/
public sealed class InterativeUI : MonoBehaviour
{
    public enum ShowType
    {
        Visible = 0,
        InVisible = 1
    }

    //=============================================
    /////               property               ////
    //=============================================
    public static bool         IsShow
    {
        get
        {
            if (_ins == null) return false;
            return (_ins._show==ShowType.Visible);
        }
    }
    public static string       Text
    {
        get
        {
            if(_ins==null) return string.Empty;
            return _ins._text.text;
        }

        set
        {
            if (_ins == null) return;
            _ins._text.text = value;
        }
    }
    public static Vector3      WorldPosition
    {
        get
        {
            if(_ins== null || _ins._mainCam == null) return Vector3.zero;
            return _ins._mainCam.ScreenToWorldPoint(_ins._rectTr.position);   
        }
        set
        {
            if (_ins == null) return;
            _ins._rectTr.position = value;
        }
    }
    public static Vector3      ScreenPosition
    {
        get
        {
            if (_ins == null) return Vector3.zero;
            return _ins._rectTr.position;
        }
        set
        {
            if (_ins == null) return;
            _ins._rectTr.position = value;
        }
    }



    //=========================================
    //////              Field             /////
    //=========================================
    private Camera          _mainCam;
    private Animator        _animator;
    private Image           _image;
    private TextMeshProUGUI _text;
    private RectTransform   _rectTr;
    private ShowType        _show = ShowType.InVisible;

    private static InterativeUI _ins;
    private readonly static string[] _anims = new string[2] { "Interaction_FadeIn", "Interaction_FadeOut" };



    //===========================================
    /////           Core methods           /////
    //===========================================
    private void Start()
    {
        #region Omit
        /**�̹� ��ü�� �����Ѵٸ� �ı��ȴ�...*/
        if(_ins!=null)
        {
            Destroy(gameObject);
            return;
        }

        /*******************************************************
         *   �ʿ��� ��� �������� ���´�.....
         * ****/
        _ins      = this;
        _mainCam  = Camera.main;
        _image    = GetComponent<Image>();
        _animator = GetComponent<Animator>();
        _text     = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _rectTr   = _animator.GetComponent<RectTransform>();

        _rectTr.position = new Vector3(99999f, 99999f);

        /*******************************************************
         *   ��ġ�� �ٲ���� ��, ���� ��ġ�� �˸°� UI�� �����Ѵ�...
         * ****/
        GamePadUIController.OnDeviceChange += (InputDeviceType prevDevice, InputDeviceType changeDevice) =>
        {
            Debug.Log($"Change input device: ({prevDevice})->({changeDevice}) (lastPadKind: {GamePadUIController.LastInputGamePadKind})");
        };
        #endregion
    }

    private void OnDestroy()
    { 
        /**������ ��ü��� �ı��ȴ�...*/
        if (_ins == this){
            _ins = null;
        }
    }



    //===============================================
    //////           Public methods             /////
    //==============================================
    public static void PopupUI(ShowType showType, string msg, Vector3 worldPosition, bool forceAnimPlay=false)
    {
        #region Omit
        if (_ins == null) return;

        /**ǥ�ÿ��ο� �˸��� �ִϸ��̼��� �����Ѵ�...*/
        if(_ins._show != showType || forceAnimPlay) {

            _ins._show = showType;
            _ins._animator.Play(_anims[(int)showType], 0, 0f);
        }

        /**ī�޶� ��ȿ���� �ʴٸ� �����Ѵ�....*/
        if(_ins._mainCam==null){

            _ins._mainCam = Camera.main;
        }

        _ins._text.text       = msg;
        _ins._rectTr.position = _ins._mainCam.WorldToScreenPoint(worldPosition);
        #endregion
    }

    public static void HideUI()
    {
        #region Omit
        if (_ins == null) return;

        _ins._rectTr.position = new Vector3(99999f, 99999f, 99999f);
        #endregion
    }

}