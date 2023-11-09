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
    [HideInInspector] public Animator[] fadeAnimator;
    private UIManager _uiManager;
    private static readonly int Out = Animator.StringToHash("FadeOut");

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = UIManager.GetInstance();
        fadeAnimator = GetComponentsInChildren<Animator>();
        foreach (var animator in fadeAnimator)
        {
            animator.gameObject.SetActive(false);
        }
    }
    
    public void FadeIn(FadeType fadeType)
    {
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(false);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(false);
                fadeAnimator[0].gameObject.SetActive(true);
                fadeAnimator[0].pl
                break;
            case FadeType.LetterBox:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(false);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(false);
                fadeAnimator[1].gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fadeType), fadeType, null);
        }
    }

    public void FadeOut(FadeType fadeType)
    {
        switch (fadeType)
        {
            case FadeType.Normal:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                fadeAnimator[0].SetTrigger(Out);
                break;
            case FadeType.LetterBox:
                _uiManager.GetGameUI(GameUIType.Coin).gameObject.SetActive(true);
                _uiManager.GetGameUI(GameUIType.CoCosi).gameObject.SetActive(true);
                fadeAnimator[1].SetTrigger(Out);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fadeType), fadeType, null);
        }
    }
}
