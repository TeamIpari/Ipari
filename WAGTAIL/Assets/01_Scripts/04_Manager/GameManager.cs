using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// �߰� �ؾ� �� ��
/// 1. Respawn
/// 2. CheckPoint �����
/// 3. Chapter �����
/// 4. Chapter Ŭ���� �� ���� ������
/// </summary>

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
    
    //================================================
    
    private Player _player;
    public int coin = 0;
    public int flower = 0;

    protected override void Awake()
    {
        base.Awake();
        _player = Player.Instance;
        _checkPointList = GetComponentsInChildren<CheckPoint>().ToList();
        _scoreObjectList = GetComponentsInChildren<ScoreObject>().ToList();
        _checkPointList.ForEach(x=>x.gameObject.SetActive(true));
        _startPoint = _checkPointList.Find(x => x.checkPointType == CheckPointType.StartPoint).transform.position;
        _currentCheckPoint = _startPoint;
        coin = 0;
        flower = 0;
    }

    private void Start()
    {
        RestartChapter();
    }

    public void Respawn()
    {
        WrapPlayerPosition(_currentCheckPoint);
    }
    
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
/*
    public int Coin
    {
        get => coin;
        set
        {
            if (coin < 999)
            {
                coin = value;
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
    */
    #endregion
}
