using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    // 챕터 시작 시 사용할 UI
    
    private ChapterType _chapterType;
    private GameObject _fideOut;
    private GameObject _fideIn;
    private GameManager _gameManager;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.GetInstance();
        _chapterType = _gameManager.LastActiveChapter.ChapterType;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
