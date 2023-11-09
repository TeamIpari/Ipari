using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    //===============================================
    //////                Property              /////
    //===============================================
    [SerializeField] private Transform centerPlatformTr;



    //================================================
    ///////               Fields                //////
    //================================================
    private WaterPlatformBehavior[] behaviors;

    private Vector3     centerPos         = Vector3.zero;
    private const float maxDst            = 11f;
    private const float yspeed            = -.6f;
    private const float delay             = .2f;
    private float       center2EdgeDstDiv = 0f;



    //===================================================
    /////             Magic methods                 /////
    //==================================================
    private void Start()
    {
        #region Omit
        int Count = transform.childCount;

        center2EdgeDstDiv = (1f / maxDst);
        behaviors         = new WaterPlatformBehavior[Count];

        for (int i = 0; i<Count; i++){

            behaviors[i] = transform.GetChild(i).GetComponent<WaterPlatformBehavior>();
            if (behaviors[i] == null) continue;
        }

        centerPos = centerPlatformTr.position;  
        #endregion
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            int Count = behaviors.Length;

            for (int i = 0; i < Count; i++)
            {
                float ratio = (1f - Vector3.Distance(centerPos, behaviors[i].transform.position) * center2EdgeDstDiv);
                behaviors[i].Yspeed     += (yspeed * ratio);
                behaviors[i].Rotspeed   += (yspeed * ratio * .7f);
                behaviors[i].UpdateDelay = delay - (delay * ratio);
                behaviors[i].SetRotDir(transform.position + Vector3.right * .3f);
            }

            FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_VineSmash, Vector3.zero, 2f);
            CameraManager.GetInstance().CameraShake(.4f, CameraManager.ShakeDir.ROTATE, .5f);
        }
    }
#endif


}
