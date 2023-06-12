using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TitleUI : MonoBehaviour
{
    public GameObject OptionUI;
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private void Start()
    {
        
    }

    private void Update()
    {
        if (OptionUI.activeSelf && Input.GetKeyDown("escape"))
        {
            OptionUI.GetComponent<Animator>().SetTrigger(FadeOut);
        }
        
        else if (Input.GetKeyDown("escape"))
        {
            GameEnd();
        }
    }

    public void GameStart()
    {
        SceneLoader.GetInstance().LoadScene("Chapter01");
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
