using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour, IFModEventFadeComplete
{
    FModEventInstance WaterIns;

    public void OnFModEventComplete(int fadeID, float goalVolume)
    {
        Debug.Log($"fade Complete(fadeID: {fadeID})/ goalVolume: {goalVolume}");
    }

    private void Start()
    {
        WaterIns = FModAudioManager.CreateInstance(FModSFXEventType.Water_Stream, transform.position);
        WaterIns.Play();
        WaterIns.Volume = 4f;
    }
}
