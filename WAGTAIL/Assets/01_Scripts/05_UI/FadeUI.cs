using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FadeType
{
    Normal,
    LetterBox
}

public class FadeUI : MonoBehaviour
{
    public delegate void OnChangeNotify(bool isDark);

    //======================================================
    ///////          Property and fields             ///////
    //======================================================
    public OnChangeNotify               OnFadeChange;
    [HideInInspector] public Animator[] fadeAnimator;


    private UIManager _uiManager;
    private static readonly int Out = Animator.StringToHash("FadeOut");



    //====================================================
    ///////              Magic methods              //////
    //====================================================
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
    public void FadeIn(FadeType fadeType, float speed=1f)
    {
        #region Omit
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(false);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(false);
                fadeAnimator[0].gameObject.SetActive(true);
                fadeAnimator[0].speed = speed;
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

    public void FadeOut(FadeType fadeType, float speed= 1f)
    {
        #region Omit
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                fadeAnimator[0].gameObject.SetActive(true);
                fadeAnimator[0].speed = speed;
                fadeAnimator[0].Play("Black_FadeOut");
                break;
            case FadeType.LetterBox:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().currentCanvas.GetComponent<Animator>().Play("Cocosi_Blue_FadeIn");
                fadeAnimator[1].SetTrigger(Out);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fadeType), fadeType, null);
        }
        #endregion
    }

    public void FadeCompleteDispatch(bool isDark)
    {
        OnFadeChange?.Invoke(isDark);
    }


}
