using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameUIType
{
    Tutorial,
    Score,
    Chapter,
    Death
}

public class GameUIManager : Singleton<GameUIManager>
{
    private List<GameUIController> _controllerList;

    protected override void Awake()
    {
        base.Awake();
        _controllerList = GetComponentsInChildren<GameUIController>().ToList();
    }

    public void ActiveUI(GameUIType type, bool isActive)
    {
        GameUIController desiredUI = _controllerList.Find(x => x.GameUIType == type);

        if (desiredUI != null)
        {
            desiredUI.gameObject.SetActive(isActive);
        }

        else { Debug.LogWarning("The desired UI was not found!"); }
    }

    public GameUIController GetGameUI(GameUIType type)
    {
        GameUIController desiredUI = _controllerList.Find(x => x.GameUIType == type);

        return desiredUI;
    }
}
