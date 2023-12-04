using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript3 : MonoBehaviour
{
    [SerializeField] SandWave TargetWave;
    [SerializeField] float    targetScale = 1f;

    // Update is called once per frame
    void Update()
    {
        if(TargetWave && Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = targetScale;
            TargetWave.StartWave();
        }

        if(TargetWave && Input.GetKeyDown(KeyCode.O))
        {
            TargetWave.SandTarget.IntakeSand(true);
        }
    }
}
