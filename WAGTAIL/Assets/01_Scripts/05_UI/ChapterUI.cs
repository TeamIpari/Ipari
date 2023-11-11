using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChapterUI : MonoBehaviour
{
    // 챕터 시작 시 사용할 UI
    public ChapterType chapterType;
    private Animator _animator;
    private FadeUI _fadeUI;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _fadeUI = UIManager.GetInstance().GetGameUI(GameUIType.Fade).GetComponent<FadeUI>();
        ChapterCanvasStart();
        //gameObject.SetActive(false);
    }

    public void ChapterCanvasStart()
    {
        switch (chapterType)
        {
            case ChapterType.Chapter01:
                Player.Instance.playerInput.enabled = false;
                _animator.Play("Chapter1");
                break;
            case ChapterType.Chapter02:
                Player.Instance.playerInput.enabled = false;
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, false);
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, false);
                _animator.Play("Chapter2");
                break;
            case ChapterType.Chapter03:
                Player.Instance.playerInput.enabled = false;
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, false);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().SetCanvas(2,true);
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, false);
                _animator.Play("Chapter3");
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }

    public void ChapterCanvasEnd()
    {
        chapterType = GameManager.GetInstance().LastActiveChapter.ChapterType;

        switch (chapterType)
        {
            case ChapterType.Chapter01:
                //FadeOut();
                Player.Instance.playerInput.enabled = true;
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>()
                    .SetCanvas(0, true);
                FModAudioManager.SetBusMute(FModBusType.Environment, false);
                FModAudioManager.SetBusMute(FModBusType.Player, false);
                gameObject.SetActive(false);
                break;
            case ChapterType.Chapter02:
                Player.Instance.playerInput.enabled = true;
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, true);
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>().SetCanvas(1,true);
                FModAudioManager.SetBusMute(FModBusType.Player, false);
                gameObject.SetActive(false);
                break;
            case ChapterType.Chapter03:
                Player.Instance.playerInput.enabled = true;
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, true);
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>().SetCanvas(2,true);
                FModAudioManager.SetBusMute(FModBusType.Player, false);
                gameObject.SetActive(false);
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

    public void FadeIn()
    {
        _fadeUI.FadeIn(FadeType.Normal);
    }
}