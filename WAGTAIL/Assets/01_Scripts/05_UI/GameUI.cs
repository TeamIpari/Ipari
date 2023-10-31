using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject PauseBtn;

    public GameObject RestartChapterUI;

    public GameObject RestartTutorialUI;
    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            switch (PauseBtn.activeSelf)
            {
                case true:
                    Continue();
                    break;
                
                case false:
                    Pause();
                    break;
            }
        }
    }
    
    public void Continue()
    {
        PauseBtn.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        PauseBtn.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReStart()
    {
        Continue();
        GameManager.GetInstance().RestartGame();
        SceneLoader.GetInstance().LoadScene("Chapter01_Heejin4");
        FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
    }

    public void GoMain()
    {
        Continue();
        SceneLoader.GetInstance().LoadScene("Title");
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
