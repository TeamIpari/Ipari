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

public class UIManager : Singleton<UIManager>
{
    private List<UIController> _uiControllerList;
    private UIController _lastActiveUI;

    protected override void Awake()
    {
        base.Awake();
        _uiControllerList = GetComponentsInChildren<UIController>().ToList();
        _uiControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.GameUI);
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
}
