using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour
{
    public ChapterType nextChapter;
    public string nextChapterName;

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
        if (other.CompareTag("Player") && this.enabled)
        {
            GoNextChapter();
        }
    }
}
