using Polybrush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

public enum ChapterType
{
    Title,
    Chapter01,
    Chapter02,
    BossRoom,
    EndCredits,
    Test
}

public class GameManager : Singleton<GameManager>
{
    // Option
    private float _masterVolume;
    private float _musicVolume;
    private float _soundEffectsVolume;

    // CheckPoint
    public int num;
    private List<CheckPoint> _checkPointList;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _currentCheckPoint = Vector3.zero;
    //================================================
    // Score
    private List<ScoreObject> _scoreObjectList;
    private int _coin = 0;
    private int _flower = 0;
    //================================================
    // Chapter
    private List<Chapter> _chapterList;
    private Chapter _lastActiveChapter;
    
    protected override void Awake()
    {
        base.Awake();

        // CheckPoints
        // ===========================================================================
        //_checkPointList = GetComponentsInChildren<CheckPoint>().ToList();
        //_checkPointList.ForEach(x=>x.gameObject.SetActive(true));
        //_startPoint = _checkPointList.Find(x => x.checkPointType == CheckPointType.StartPoint).transform.position;
        //_currentCheckPoint = _startPoint;
        
        // Score
        // ===========================================================================
        //_scoreObjectList = GetComponentsInChildren<ScoreObject>().ToList();
        //_scoreObjectList.ForEach(x => x.gameObject.SetActive(true));
        Coin = 0;
        Flower = 0;
        
        // Chapter
        _chapterList = GetComponentsInChildren<Chapter>().ToList();
        _chapterList.ForEach(x => x.gameObject.SetActive(false));
    }

    public void Respawn()
    {
        WrapPlayerPosition(_currentCheckPoint);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void StartChapter(ChapterType type)
    {
        num = 0;
        Chapter desiredChapter = _chapterList.Find(x => x.ChapterType == type);
        // CheckPoints
        // ==================================================================================
        _checkPointList = desiredChapter.GetComponentsInChildren<CheckPoint>().ToList();
        _checkPointList.ForEach(x => x.gameObject.SetActive(true));
        _startPoint = _checkPointList.Find(x => x.checkPointType == CheckPointType.StartPoint).transform.position;
        _currentCheckPoint = _startPoint;
        // ==================================================================================

        // Score
        // ==================================================================================
        _scoreObjectList = desiredChapter.GetComponentsInChildren<ScoreObject>().ToList();
        _scoreObjectList.ForEach(x => x.gameObject.SetActive(true));
        // ==================================================================================

        desiredChapter.gameObject.SetActive(true);
        WrapPlayerPosition(_startPoint);
        _lastActiveChapter = desiredChapter;
    }
    
    // �� Func�� ���� �۾��ؾ���
    public void RestartChapter()
    {
        Coin = 0;
        Flower = 0;
        _scoreObjectList.ForEach(x=>x.gameObject.SetActive(true));
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
        Player player = Player.Instance;
        
        if (player != null)
        {
            player.controller.enabled = false;
            player.transform.position = pos;
            player.controller.enabled = true;
        }
        
        else
        {
            Debug.Log("Player is not found!!");
        }
    }

    public void Test()
    {
        _chapterList.Find(x => x.ChapterType == ChapterType.Chapter01).gameObject.SetActive(false);
    }
    
    #region Property

    public int Coin
    {
        get => _coin;
        set
        {
            if (_coin is < 0 or >= 999) return;
            if (value < 0) return;
            _coin = value;
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