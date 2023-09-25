using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BossRoom : MonoBehaviour
{
    [FormerlySerializedAs("NextChapter")] public ChapterType nextChapter;
    [FormerlySerializedAs("NextChapterName")] public string nextChapterName;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GoNextChapter();
        }
    }
    
    public void GoNextChapter()
    {
        SceneLoader.GetInstance().LoadScene(nextChapterName);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoNextChapter();
        }
    }
}
