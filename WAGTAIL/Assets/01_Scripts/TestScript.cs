using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    private WaterPlatformBehavior[] behaviors;

    private void Start()
    {
        int Count = transform.childCount;

        behaviors = new WaterPlatformBehavior[Count];
        for (int i = 0; i<Count; i++)
        {
            behaviors[i] = transform.GetChild(i).GetComponent<WaterPlatformBehavior>();
            if (behaviors[i] == null) Debug.Log($"({i})¹øÂ°´Â null!!!");
            Debug.Log($"behaviors added: {behaviors[i].name}/ yspeed: {behaviors[i].Yspeed}");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ShakeProgress());
        }
    }

    private IEnumerator ShakeProgress()
    {
        float yspeed = -.5f;
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(.5f);

        int Count = behaviors.Length;
        for(int i=0; i<Count; i++)
        {
            behaviors[i].Yspeed += yspeed;
            yield return waitTime;
        }

    }


}
