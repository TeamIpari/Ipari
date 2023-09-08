using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance _WaterStream;

    private void Start()
    {
        _WaterStream = FModAudioManager.CreateInstance(FModSFXEventType.Water_Stream, transform.position);
        _WaterStream.Play();
        _WaterStream.Set3DDistance(1f, 20f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _WaterStream.Max3DDistance);
    }

}
