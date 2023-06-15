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
            Debug.Log("Start");
            SoundTest.GetInstance().PlayBGM("isInGame",false);
            SoundTest.GetInstance().PlayBGM("isTitle", true);
            UIManager.GetInstance().SwitchCanvas(CanvasType.MainMenu);
            UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(true);
        }

        if (Type == ChapterType.Chapter01)
        {
            //UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(false);
            GameManager.GetInstance().StartChapter(ChapterType.Chapter01);
            CameraManager.GetInstance().CameraSetting();
        }
    }

}
