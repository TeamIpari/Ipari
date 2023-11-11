using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public enum CanvasType
{
    MainMenu,
    GameUI
}

public enum LanguageType
{
    KOR,
    ENG,
    JP,
    CN,
}

public enum GameUIType
{
    Interactable,
    Tutorial,
    Coin,
    CoCosi,
    TextBox,
    Chapter,
    Death,
    Fade
}

public class UIManager : Singleton<UIManager>
{
    private List<UIController> _uiControllerList;
    private List<GameUIController> _gameUIControllerList;
    private UIController _lastActiveUI;
    private LanguageType _languageType;

    protected override void Awake()
    {
        base.Awake();
        _uiControllerList = GetComponentsInChildren<UIController>().ToList();
        _uiControllerList.ForEach(x => x.gameObject.SetActive(false));
        _languageType = LanguageType.KOR;

        _gameUIControllerList = GetCanvas(CanvasType.GameUI).GetComponentsInChildren<GameUIController>().ToList();
        
        ActiveGameUI(GameUIType.Death, false);
        ActiveGameUI(GameUIType.TextBox,false);
    }

    public void ActiveGameUI(GameUIType type, bool isActive)
    {
        GameUIController desiredUI = _gameUIControllerList.Find(x => x.GameUIType == type);
        desiredUI.gameObject.SetActive(isActive);
        if (desiredUI != null)
        {
            desiredUI.gameObject.SetActive(isActive);
        }
        
        else Debug.LogWarning("The desired UI was not found!");
    }
    
    public void SwitchCanvas(CanvasType type) 
    {
        if (_lastActiveUI != null)
        {
            _lastActiveUI.gameObject.SetActive(false);
        }

        UIController desiredCanvas = _uiControllerList.Find(x => x.canvasType == type);
        
        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            _lastActiveUI = desiredCanvas;
        }

        else {Debug.LogWarning("The desired canvas was not found!");}
    }

    public UIController GetActiveCanvas()
    {
        return _lastActiveUI;
    }

    public UIController GetCanvas(CanvasType type)
    {
        UIController desiredCanvas = _uiControllerList.Find(x => x.canvasType == type);

        return desiredCanvas;
    }

    public GameUIController GetGameUI(GameUIType type)
    {
        GameUIController desiredUI = _gameUIControllerList.Find(x => x.GameUIType == type);

        return desiredUI;
    }
    public LanguageType GetLanguageType
    {
        get
        {
            return _languageType;
        }
    }

    public void ChangeLanguage(TextMeshProUGUI Label)
    {
        Debug.Log($"{Label.text}");
        switch ( Label.text)
        {
            case "«—±πæÓ":
                _languageType = LanguageType.KOR;
                break;
            case "ENGLISH":
                _languageType = LanguageType.ENG;
                break;
            case "ÏÌ‹‚Âﬁ":
                _languageType = LanguageType.JP;
                break;
            case "ÒÈœ–Âﬁ":
                _languageType = LanguageType.CN;
                break;
            default:
                break;
        } 
    } 
}
