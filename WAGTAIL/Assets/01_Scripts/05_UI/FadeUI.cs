using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FadeType
{
    Normal,
    LetterBox
}

public enum FadeApplyType : byte
{
    WHITE_TO_DARK           = 0b011,
    WHITE_TO_DARK_TO_WHITE  = 0b010,
    DARK_TO_WHITE           = 0b100,
    DARK_TO_WHITE_TO_DARK   = 0b101
}

public class FadeUI : MonoBehaviour
{
    public delegate void OnChangeNotify(bool isDark);

    //======================================================
    ///////          Property and fields             ///////
    //======================================================
    public static OnChangeNotify        OnFadeChange;
    [HideInInspector] public Animator[] fadeAnimator;


    private UIManager _uiManager;
    private byte      _fadeInfo     = 0, 
                      _fadeIndex    = 0;
    private static readonly int Out = Animator.StringToHash("FadeOut");
    private static FadeUI       _ins;



    //====================================================
    ///////              Magic methods              //////
    //====================================================
    private void Awake()
    {
        #region Omit

        if (_ins==null)
        {
            _ins = this;
        }

        #endregion
    }

    private void Start()
    {
        #region Omit
        _uiManager = UIManager.GetInstance();
        fadeAnimator = GetComponentsInChildren<Animator>();
        foreach (var animator in fadeAnimator)
        {
            animator.gameObject.SetActive(false);
        }
        #endregion
    }



    //=================================================
    ///////            Core methods             //////
    //=================================================
    public static void ApplyScreenFade(FadeApplyType applyType, float speed=1f)
    {
        #region Omit

        /*******************************************
         *    페이드를 적용한다....
         * ******/
        if (_ins == null) return;

        /**정보기록....*/
        _ins._fadeInfo  = (byte)applyType;
        _ins._fadeIndex = 0;

        /**페이드 실행...*/
        _ins.fadeAnimator[0].speed = speed;
        if ((_ins._fadeInfo & 1) >= 0) _ins.FadeIn(FadeType.Normal);
        else _ins.FadeOut(FadeType.Normal);

        _ins._fadeIndex++;
        #endregion
    }

    public void FadeIn(FadeType fadeType)
    {
        #region Omit
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(false);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(false);
                fadeAnimator[0].gameObject.SetActive(true);
                fadeAnimator[0].Play("Black_FadeIn");
                break;
            case FadeType.LetterBox:
                _uiManager.GetGameUI(GameUIType.Coin).GetComponent<Animator>().SetTrigger("FadeOut");
                _uiManager.GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().currentCanvas.GetComponent<Animator>().SetTrigger("FadeOut");
                fadeAnimator[1].gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fadeType), fadeType, null);
        }
        #endregion
    }

    public void FadeOut(FadeType fadeType)
    {
        #region Omit
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                fadeAnimator[0].gameObject.SetActive(true);
                fadeAnimator[0].Play("Black_FadeOut");
                break;
            case FadeType.LetterBox:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                fadeAnimator[1].SetTrigger(Out);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fadeType), fadeType, null);
        }
        #endregion
    }

    public void SetFadeComplete_Internal(bool isDark)
    {
        #region Omit

        bool prev = (_fadeInfo & (1 << _fadeIndex-1))>= 0;
        bool curr = (_fadeInfo & (1 << _fadeIndex)) >= 0;

        OnFadeChange?.Invoke(isDark);
        if (prev == curr || _fadeIndex>2)
        {
            _fadeInfo  = 0;
            _fadeIndex = 0;
            return;
        }

        /**페이드 실행...*/
        if ((_fadeInfo & (1<<_fadeIndex)) >= 0) FadeIn(FadeType.Normal);
        else FadeOut(FadeType.Normal);

        _fadeIndex++;

        #endregion
    }


}
