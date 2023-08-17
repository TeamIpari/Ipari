using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum CanvasType
{
    MainMenu,
    GameUI,
    EndScreen
}

public enum GameUIType
{
    Jump,
    Interactable,
    Tutorial,
    Score,
    Chapter,
    Death
}

public class UIManager : Singleton<UIManager>
{
    private List<UIController> _uiControllerList;
    private List<GameUIController> _gameUIControllerList;
    private UIController _lastActiveUI;

    protected override void Awake()
    {
        base.Awake();
        _uiControllerList = GetComponentsInChildren<UIController>().ToList();
        _uiControllerList.ForEach(x => x.gameObject.SetActive(false));

        _gameUIControllerList = GetCanvas(CanvasType.GameUI).GetComponentsInChildren<GameUIController>().ToList();
        ActiveGameUI(GameUIType.Death, false);
        ActiveGameUI(GameUIType.Interactable, false);
        ActiveGameUI(GameUIType.Jump, false);

        // 테스트 끝나면 CanvasType.MainMenu로 바꿔야함
        //SwitchCanvas(CanvasType.MainMenu);
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
}
