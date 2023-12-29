using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public GameObject OptionUI;
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private void Start()
    {
        
    }

    public void ButtonSound()
    {
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.UI_Button);
    }

    public void GameStart()
    {
        GameManager.GetInstance().RestartGame();
        SceneLoader.GetInstance().LoadScene("Chapter01");
    }
    
    public void LoadGame()
    {
        var loadChapter = GameManager.GetInstance().LastActiveChapter.ChapterType;
        
        switch (loadChapter)
        {
            case ChapterType.Chapter01:
                SceneLoader.GetInstance().LoadScene("Chapter01");
                break;
            case ChapterType.Chapter02 or ChapterType.MiddleBossRoom:
                SceneLoader.GetInstance().LoadScene("Chapter02");
                break;
            case ChapterType.Chapter03:
                SceneLoader.GetInstance().LoadScene("Chapter03");
                break;
            case ChapterType.BossRoom:
                SceneLoader.GetInstance().LoadScene("BossRoom");
                break;
        }
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
