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
        SceneLoader.GetInstance().LoadScene("Chapter01_Heejin4");
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
