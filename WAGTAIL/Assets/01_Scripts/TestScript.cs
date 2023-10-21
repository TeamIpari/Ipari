using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct CocosiCollection
{
    //=========================================================
    ///////           Property and fields               ///////
    //=========================================================
    public bool CollectCompleted { get { return true; } }

    private int _chapters;



    //================================================
    //////           Public methods             //////
    //================================================
    public bool ChapterIsComplete(int chapter)
    {
        chapter = Mathf.Clamp(chapter, 0, 4);
        return false;
    }

    public void SetCocosi(int chapter, bool isCollect)
    {
        chapter = Mathf.Clamp(chapter, 0, 4);
    }
}

public sealed class TestScript : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("진짜 정답이 이거라면 답대가리가 없는고네>?>?");

    }

}
