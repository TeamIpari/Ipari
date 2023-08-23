using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FModAnimatorHelper : MonoBehaviour
{
    public void PlaySFX(FModSFXEventType eventType)
    {
        FModAudioManager.PlayOneShotSFX(eventType);
    }

    public void PlayBGM(FModBGMEventType eventType)
    {
        FModAudioManager.PlayBGM(eventType);
    }

}
