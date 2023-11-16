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
    private Animator            _pauseAnimator;
    private bool                _isPressed = false;
    private float               _lastBGMVoulme = 0f;

    // Update is called once per frame
    void Update()
    {
        Gamepad currPad         = Gamepad.current;
        bool    pressedSelect   = (currPad!=null && currPad.startButton.value!=0f);
        bool    ubpressedSelect = (currPad != null && currPad.startButton.value== 0f);

        if (Input.GetKeyDown("escape") || (pressedSelect && !_isPressed)){

            /**일시정지 또는 게임을 이어서 한다...*/
            _isPressed = true;

            if (Time.timeScale > 0f) Pause();
            else Continue();
        }
        else if (_isPressed && ubpressedSelect) _isPressed = false;
    }
    
    public void Continue()
    {
        GamePadUIController.UseCursorAutoVisible = false;
        GamePadUIController.Current.OnDisSelect?.Invoke();
        GamePadUIController.Current = null;
        FModAudioManager.SetBusVolume(FModBusType.BGM, _lastBGMVoulme);
        _pauseAnimator.Play("Pause_FadeOut", 0, 0f);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        GamePadUIController.UseCursorAutoVisible = true;
        if (_pauseButton==null){

            _pauseButton   = PauseBtn.transform.Find("PopUp").Find("Restart").GetComponent<GamePadUIController>();
            _pauseAnimator = _pauseButton.transform.parent.parent.GetComponent<Animator>();
        }

        if (_pauseButton!=null){

            _pauseButton.ChangeCurrentThis();
            _pauseButton.SetMoveLock(1);
        }

        PauseBtn.SetActive(true);
        _pauseAnimator.Play("Pause_FadeIn", 0, 0f);
        _lastBGMVoulme = FModAudioManager.GetBusVolume(FModBusType.BGM);
        FModAudioManager.SetBusVolume(FModBusType.BGM, _lastBGMVoulme*.5f);
        InterativeUI.HideUI();
        Time.timeScale = 0f;
    }

    public void ReStart()
    {
        Continue();
        GameManager.GetInstance().RestartGame();
        SceneLoader.GetInstance().LoadScene("Chapter01");
        UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().SetCanvas(1,false);
        FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
    }

    public void ChapterRestart()
    {
        Continue();
        GameManager.GetInstance().RestartChapter();
        SceneLoader.GetInstance().LoadScene(GameManager.GetInstance().LastActiveChapter.ChapterType.ToString());
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
