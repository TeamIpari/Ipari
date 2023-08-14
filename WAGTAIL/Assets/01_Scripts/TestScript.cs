using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance TestIns;

    private void Start()
    {
        
    }

    private void Update()
    {
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
