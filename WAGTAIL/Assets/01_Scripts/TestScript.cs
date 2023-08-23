using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    private void Start()
    {
        FModEventInstance Instance = FModAudioManager.CreateInstance(FModSFXEventType.Player_Hit);
        Instance.SetParameter(
            FModLocalParamType.PlayerHitType,
            FModParamValueRange.PlayerHitType.Max
        );
        Instance.Play();
    }


}
