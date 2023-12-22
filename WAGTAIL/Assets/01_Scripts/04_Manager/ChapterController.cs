using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChapterController : MonoBehaviour
{
    public ChapterType Type;
    private static GamePadUIController _startBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        if(_startBtn==null)
        {
            UIManager ui = UIManager.GetInstance();
            _startBtn    = ui.transform.Find("MainMenu").Find("Title_Canvas").Find("TitleBtnArea").Find("Btn_Start").GetComponent<GamePadUIController>();
        }

        switch (Type)
        {
            case ChapterType.Title:
                GamePadUIController.UseCursorAutoVisible = true;
                GamePadUIController.Current              = _startBtn;
                UIManager.GetInstance().SwitchCanvas(CanvasType.MainMenu);
                UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(true);
                FModAudioManager.UsedBGMAutoFade = true;
                FModAudioManager.BGMAutoFadeDuration = 2f;
                //FModAudioManager.SetBusMute(FModBusType.Player, true);
                FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
                break;
            case ChapterType.Chapter01:
                GameManager.GetInstance().StartChapter(ChapterType.Chapter01);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, false);
                UIManager.GetInstance().ActiveGameUI(GameUIType.Tutorial, false);
                CameraManager.GetInstance().CameraSetting();
                break;
            case ChapterType.Chapter02:
                GameManager.GetInstance().StartChapter(ChapterType.Chapter02);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                FModAudioManager.SetBusMute(FModBusType.Player, true);
                FModAudioManager.PlayBGM(FModBGMEventType.NepenthesRoad);
                CameraManager.GetInstance().CameraSetting();
                break;
            case ChapterType.MiddleBossRoom:
                FModAudioManager.PlayBGM(FModBGMEventType.NepenthesBossBGM);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                GameManager.GetInstance().StartChapter(ChapterType.MiddleBossRoom);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().SetCanvas(1,false);
                CameraManager.GetInstance().CameraSetting();
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, false);
                break;
            case ChapterType.Chapter03:
                GameManager.GetInstance().StartChapter(ChapterType.Chapter03);
                FModAudioManager.PlayBGM(FModBGMEventType.Chapter4BGM);
                CameraManager.GetInstance().CameraSetting();
                break;
            case ChapterType.BossRoom:
                GameManager.GetInstance().StartChapter(ChapterType.BossRoom);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                //UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).GetComponent<CollectionCocosiUI>().SetCanvas(2,false);
                break;
            case ChapterType.EndCredits:
                FModAudioManager.PlayBGM(FModBGMEventType.tavuti_Credit);
                //GameManager.GetInstance().StartChapter(ChapterType.EndCredits);
                break;
            case ChapterType.Test:
                //GameManager.GetInstance().StartChapter(ChapterType.Test);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                break;
        }

        InterativeUI.HideUI();
    }

}
