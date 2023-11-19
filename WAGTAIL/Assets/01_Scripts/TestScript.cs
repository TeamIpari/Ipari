using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance Ins;

    private void Start()
    {
        Ins = FModAudioManager.CreateInstance(FModSFXEventType.Water_Stream);
        Ins.Play();
        Ins.Position = transform.position;
        Ins.Volume = 10f;
        Ins.Set3DDistance(8f, 30f);
    }

    private void OnDestroy()
    {
        Ins.Destroy();
    }

}
