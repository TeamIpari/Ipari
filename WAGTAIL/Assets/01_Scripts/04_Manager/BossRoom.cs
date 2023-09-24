using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public ChapterType NextChapter;
    public string NextChapterName;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SceneLoader.GetInstance().LoadScene(NextChapterName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.GetInstance().LoadScene(NextChapterName);
        }
    }
}
