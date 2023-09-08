using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance _WaterStream;

    private void Start()
    {
        _WaterStream = FModAudioManager.CreateInstance(FModSFXEventType.Water_Stream, transform.position);
        _WaterStream.Volume = 10f;
        _WaterStream.Set3DDistance(1f, 20f);
        _WaterStream.Play();
    }

    private void OnDestroy()
    {
        _WaterStream.Destroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _WaterStream.Max3DDistance);
    }

}
