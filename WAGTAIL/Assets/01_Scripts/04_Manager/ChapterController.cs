using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterController : MonoBehaviour
{
    public ChapterType Type;
    
    // Start is called before the first frame update
    void Start()
    {
        switch (Type)
        {
            case ChapterType.Title:
                UIManager.GetInstance().SwitchCanvas(CanvasType.MainMenu);
                UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(true);
                FModAudioManager.UsedBGMAutoFade = true;
                FModAudioManager.BGMAutoFadeDuration = 2f;
                FModAudioManager.SetBusMute(FModBusType.Player, true);
                FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
                break;
            case ChapterType.Chapter01:
                //UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(false);
                GameManager.GetInstance().StartChapter(ChapterType.Chapter01);
                CameraManager.GetInstance().CameraSetting();
                break;
            case ChapterType.Chapter02:
                FModAudioManager.PlayBGM(FModBGMEventType.NepenthesRoad);
                GameManager.GetInstance().StartChapter(ChapterType.Chapter02);
                CameraManager.GetInstance().CameraSetting();
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>().SetCanvas(1,true);
                break;
            case ChapterType.MiddleBossRoom:
                FModAudioManager.PlayBGM(FModBGMEventType.NepenthesBossBGM);
                GameManager.GetInstance().StartChapter(ChapterType.MiddleBossRoom);
                CameraManager.GetInstance().CameraSetting();
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, false);
                break;
            case ChapterType.Chapter03:
                FModAudioManager.PlayBGM(FModBGMEventType.Chapter4BGM);
                GameManager.GetInstance().StartChapter(ChapterType.Chapter03);
                CameraManager.GetInstance().CameraSetting();
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>().SetCanvas(2,true);
                break;
            case ChapterType.EndCredits:
                GameManager.GetInstance().StartChapter(ChapterType.EndCredits);
                break;
            case ChapterType.Test:
                //GameManager.GetInstance().StartChapter(ChapterType.Test);
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                break;
        }

        InterativeUI.HideUI();
    }

}
