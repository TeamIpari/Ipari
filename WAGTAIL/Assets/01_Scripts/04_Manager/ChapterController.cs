using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterController : MonoBehaviour
{
    public ChapterType Type;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Type == ChapterType.Title)
        {
            UIManager.GetInstance().SwitchCanvas(CanvasType.MainMenu);
            UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(true);

            /*******************************************
             *   타이틀 브금 적용...
             * **/
            FModAudioManager.UsedBGMAutoFade = true;
            FModAudioManager.BGMAutoFadeDuration = 2f;
            FModAudioManager.SetBusMute(FModBusType.Player, true);
            FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
        }

        if (Type == ChapterType.Chapter01)
        {
            //UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(false);
            GameManager.GetInstance().StartChapter(ChapterType.Chapter01);
            CameraManager.GetInstance().CameraSetting();
        }

        if (Type == ChapterType.Chapter02)
        {
            GameManager.GetInstance().StartChapter(ChapterType.Chapter02);
            CameraManager.GetInstance().CameraSetting();
        }

        if (Type == ChapterType.BossRoom)
        {
            GameManager.GetInstance().StartChapter(ChapterType.BossRoom);
            CameraManager.GetInstance().CameraSetting();
        }

        if (Type == ChapterType.EndCredits)
        {
            GameManager.GetInstance().StartChapter(ChapterType.EndCredits);
        }
    }

}
