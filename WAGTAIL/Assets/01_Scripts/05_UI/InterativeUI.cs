using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GamePadUIController;

/**********************************************************
 *  상호작용에 알맞는 UI가 출력되되록 하는 컴포넌트입니다.
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
        /**이미 객체가 존재한다면 파괴된다...*/
        if(_ins!=null)
        {
            Destroy(gameObject);
            return;
        }

        /*******************************************************
         *   필요한 모든 참조들을 얻어온다.....
         * ****/
        _ins      = this;
        _mainCam  = Camera.main;
        _image    = GetComponent<Image>();
        _animator = GetComponent<Animator>();
        _text     = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _rectTr   = _animator.GetComponent<RectTransform>();

        _rectTr.position = new Vector3(99999f, 99999f);

        /*******************************************************
         *   장치가 바뀌었을 때, 현재 장치에 알맞게 UI를 갱신한다...
         * ****/
        GamePadUIController.OnDeviceChange += (InputDeviceType prevDevice, InputDeviceType changeDevice) =>
        {
            Debug.Log($"Change input device: ({prevDevice})->({changeDevice}) (lastPadKind: {GamePadUIController.LastInputGamePadKind})");
        };
        #endregion
    }

    private void OnDestroy()
    { 
        /**동일한 객체라면 파괴된다...*/
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

        /**표시여부에 알맞은 애니메이션을 실행한다...*/
        if(_ins._show != showType || forceAnimPlay) {

            _ins._show = showType;
            _ins._animator.Play(_anims[(int)showType], 0, 0f);
        }

        /**카메라가 유효하지 않다면 갱신한다....*/
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