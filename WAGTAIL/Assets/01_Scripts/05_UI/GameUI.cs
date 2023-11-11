using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject PauseBtn;

    public GameObject RestartChapterUI;

    public GameObject RestartTutorialUI;

    private GamePadUIController _pauseButton;

    // Update is called once per frame
    void Update()
    {
        Gamepad currPad       = Gamepad.current;
        bool    pressedSelect = (currPad!=null && currPad.selectButton.value!=0f);

        if (Input.GetKeyDown("escape") || pressedSelect)
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
        GamePadUIController.Current = null;
        PauseBtn.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        if (_pauseButton==null){

            _pauseButton = PauseBtn.transform.Find("PopUp").Find("Restart").GetComponent<GamePadUIController>();
        }

        if (_pauseButton!=null){

            _pauseButton.ChangeCurrentThis();
        }

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
