using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour, IFModEventFadeComplete
{
    FModEventInstance TestIns;

    public void OnFModEventComplete(int fadeID, float goalVolume)
    {
        Debug.Log($"fade Complete(fadeID: {fadeID})/ goalVolume: {goalVolume}");
    }

    private void Start()
    {
        FModAudioManager.AutoFadeInOutBGM = true;
        FModAudioManager.AutoFadeBGMDuration = 10f;
        FModAudioManager.OnEventFadeComplete += OnFModEventComplete;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Flowers_Burst);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            FModAudioManager.PlayBGM(FModBGMEventType.tavuti_ingame1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            FModAudioManager.PlayBGM(FModBGMEventType.Wagtail_bgm_title);
        }

    }
}
