using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Parry, transform.position, 3f);
        }
    }
}
