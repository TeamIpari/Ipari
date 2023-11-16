using IPariUtility;
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
    Cocosi
}

public enum ChapterType
{
    Title,
    Chapter01,
    Chapter02,
    MiddleBossRoom,
    Chapter03,
    BossRoom,
    EndCredits,
    Test
}

public class GameManager : Singleton<GameManager>
{
    // CheckPoint
    public int num;
    private List<CheckPoint> _checkPointList;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _currentCheckPoint = Vector3.zero;
    //================================================
    // Score
    private int _coin = 0;
    public bool[] cocosi = new bool[11];
    
    //================================================
    // Chapter
    private List<Chapter> _chapterList;

    //================================================

    protected override void Awake()
    {
        base.Awake();

        if (!DataManager.Instance.LoadGameData())
        {
            Coin = 0;
            cocosi = new bool[] { false, false, false, false, false, false, false, false, false, false, false };
        
            IsKeyboard = true;
            IsGamepad = false;

            PrevCoin = 0;
        }
        
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
        
        // 마지막 Coin 저장
        PrevCoin = _coin;
        desiredChapter.gameObject.SetActive(true);
        WrapPlayerPosition(_startPoint);
        LastActiveChapter = desiredChapter;
        
        DataManager.Instance.SaveGameDate();
    }
    
    // 이 Func는 추후 작업해야함
    public void RestartChapter()
    {
        switch (LastActiveChapter.ChapterType)
        {
            case ChapterType.Chapter01:
                for (var i = 0; i < 5; i++)
                {
                    cocosi[i] = false;
                }
                break;
            case ChapterType.Chapter02 or ChapterType.MiddleBossRoom:
                for (var i = 5; i < 8; i++)
                {
                    cocosi[i] = false;
                }
                LastActiveChapter = _chapterList.Find(x => x.ChapterType == ChapterType.Chapter02);
                break;
            case ChapterType.Chapter03:
                for(var i = 8; i < 11; i++)
                {
                    cocosi[i] = false;
                }
                break;
        }
        
        // 코인 초기화
        Coin = PrevCoin;
        
        // 체크포인트 초기화
        _startPoint = Vector3.zero;
        _currentCheckPoint = Vector3.zero;
    }
    
    public void RestartGame()
    {
        // 코코시 초기화
        cocosi = new bool[] { false, false, false, false, false, false, false, false, false, false, false };
        
        // 코인 초기화
        PrevCoin = 0;
        Coin = 0;

        _startPoint = Vector3.zero;
        _currentCheckPoint = Vector3.zero;

        LastActiveChapter = null;
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
#if UNITY_EDITOR
            Debug.Log("Player is not found!!");
#endif
        }
    }

    public void Test()
    {
        _chapterList.Find(x => x.ChapterType == ChapterType.Chapter01).gameObject.SetActive(false);
    }

    protected override void OnSigletonDestroy()
    {
        IpariUtility.ClearUtilityState();
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
    
    public int PrevCoin { get; set; }
    
    public Chapter LastActiveChapter { get; set; }

    public bool IsKeyboard { get; set; }

    public bool IsGamepad { get; set; }

    #endregion
}