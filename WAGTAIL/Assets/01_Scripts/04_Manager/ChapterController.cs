using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundTest.GetInstance().PlayBGM("isTitle",true);
        UIManager.GetInstance().SwitchCanvas(CanvasType.MainMenu);
    }

}
