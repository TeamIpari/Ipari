using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    // 챕터 시작 시 사용할 UI
    private ChapterType _chapterType;
    private GameManager _gameManager;
    private Animator _animator;
    private FadeUI _fadeUI;


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.GetInstance();
        _animator = GetComponent<Animator>();
        _chapterType = _gameManager.LastActiveChapter.ChapterType;
        _fadeUI = UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>();
        ChapterCanvasStart(GameManager.GetInstance().LastActiveChapter.ChapterType);
    }

    public void ChapterCanvasStart(ChapterType chapterType)
    {
        Debug.Log("ChapterUIStart");
        switch (_chapterType)
        {
            case ChapterType.Chapter01:
                _fadeUI.FadeIn(FadeType.Normal);
                _animator.Play("Chapter1");
                break;
            case ChapterType.Chapter02:
                _fadeUI.FadeIn(FadeType.Normal);
                _animator.Play("Chapter2");
                break;
            case ChapterType.Chapter03:
                _fadeUI.FadeIn(FadeType.Normal);
                _animator.Play("Chapter3");
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }

    public void FadeOut()
    {
        _fadeUI.FadeOut(FadeType.Normal);
    }
}