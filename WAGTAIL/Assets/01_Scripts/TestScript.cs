using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance TestIns;

    private void Start()
    {
        FModAudioManager.AutoFadeInOutBGM = true;
        FModAudioManager.AutoFadeBGMDuration = 10f;
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
