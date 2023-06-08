using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CheckPointType
{
    StartPoint,
    CheckPoint
}

public enum ScoreType
{
    Coin,
    Flower
}

public class GameManager : Singleton<GameManager>
{
    // CheckPoint
    private List<CheckPoint> _checkPointList;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _currentCheckPoint = Vector3.zero;
    //================================================
    // Score
    private List<ScoreObject> _scoreObjectList;
    private int _coin = 0;
    private int _flower = 0;
    //================================================
    private Player _player;

    protected override void Awake()
    {
        base.Awake();
        _player = Player.Instance;
        
        // CheckPoints
        // ===========================================================================
        _checkPointList = GetComponentsInChildren<CheckPoint>().ToList();
        _checkPointList.ForEach(x=>x.gameObject.SetActive(true));
        _startPoint = _checkPointList.Find(x => x.checkPointType == CheckPointType.StartPoint).transform.position;
        _currentCheckPoint = _startPoint;
        
        // Score
        // ===========================================================================
        _scoreObjectList = GetComponentsInChildren<ScoreObject>().ToList();
        _scoreObjectList.ForEach(x => x.gameObject.SetActive(true));
        Coin = 70;
        Flower = 0;
    }

    private void Start()
    {
        RestartChapter();
    }

    public void Respawn()
    {
        WrapPlayerPosition(_currentCheckPoint);
    }
    
    // 이 Func는 추후 작업해야함
    public void RestartChapter()
    {
        _checkPointList.ForEach(x=>x.gameObject.SetActive(true));
        _currentCheckPoint = _startPoint;
        WrapPlayerPosition(_startPoint);
    }
    
    public void SwitchCheckPoint(Vector3 pos)
    {
        _currentCheckPoint = pos;
    }

    public void WrapPlayerPosition(Vector3 pos)
    {
        _player.controller.enabled = false;
        _player.transform.position = pos;
        _player.controller.enabled = true;
    }
    
    #region Property

    public int Coin
    {
        get => _coin;
        set
        { 
            if (_coin < 999)
            {
                _coin = value;
            }
        }
    }

    public int Flower
    {
        get => _flower;
        set
        {
            if (_flower is >= 0 and < 50)
            {
                _flower = value;
            }
        }
    }
    
    #endregion
    
}
