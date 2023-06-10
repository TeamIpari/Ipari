using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cinemachine;
using UnityEngine;

public enum CameraType
{
    Main,
    Side,
    Top,
    Back,
    Death
}

public class CameraManager : Singleton<CameraManager>
{
    private List<CameraController> _cameraControllerList;

    private CameraController _currentCamera;

    private CameraController _prevCamera;

    protected override void Awake()
    {
        base.Awake();
        _cameraControllerList = GetComponentsInChildren<CameraController>().ToList();
        _cameraControllerList.ForEach(x => x.VirtualCamera.Priority = 10);
        //_cameraControllerList.ForEach(x => x.gameObject.SetActive(false));
    }

    private void Start()
    {
        SwitchCamera(CameraType.Main);
    }

    public void SwitchCamera(CameraType type)
    {
        CameraController desiredCamera = _cameraControllerList.Find(x => x.CameraType == type);

        if (desiredCamera != null)
        {
            desiredCamera.VirtualCamera.MoveToTopOfPrioritySubqueue();
            _prevCamera = _currentCamera;
            _currentCamera = desiredCamera;
        }

        else { Debug.LogWarning("The desired camera was not found!, SwitchCamera() was failed!"); }
    }

    public void SwitchPrevCamera()
    {
        var cam = _currentCamera;

        if(_prevCamera != null )
        {
            _prevCamera.VirtualCamera.MoveToTopOfPrioritySubqueue();
            _currentCamera = _prevCamera;
            _prevCamera = cam;
        }

        else
        {
            SwitchCamera(CameraType.Main);
            Debug.LogWarning("PrevCamera was not found! Switch MainCam");
        }
    }

    public CameraController GetCurrentCamera()
    {
        return _currentCamera;
    }

    public CameraController GetPrevCamera()
    {
        return _prevCamera;
    }
}
