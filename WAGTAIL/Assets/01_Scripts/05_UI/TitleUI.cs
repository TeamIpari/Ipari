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

    private void Update()
    {
        if (OptionUI.activeSelf && Input.GetKeyDown("escape"))
        {
            OptionUI.GetComponent<Animator>().SetTrigger(FadeOut);
        }
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
                SceneLoader.GetInstance().LoadScene("Chapter01_Heejin4");
                break;
            case ChapterType.Chapter02 or ChapterType.MiddleBossRoom:
                SceneLoader.GetInstance().LoadScene("Chapter02_Heejin");
                break;
            case ChapterType.Chapter03:
                SceneLoader.GetInstance().LoadScene("Chapter04_mini2");
                break;
            case ChapterType.BossRoom:
                SceneLoader.GetInstance().LoadScene("Boss_Crap_FINAL_Front");
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
