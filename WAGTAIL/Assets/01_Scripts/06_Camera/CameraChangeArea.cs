using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraChangeArea : MonoBehaviour
{
    private CameraManager _cameraManager;
    private CinemachineVirtualCamera _virtualCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraManager = CameraManager.GetInstance();
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _virtualCamera.Priority = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _virtualCamera.Priority = 20;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _virtualCamera.Priority = 0;
            _cameraManager.GetCurrentCamera().VirtualCamera.MoveToTopOfPrioritySubqueue();
        }
    }
}
