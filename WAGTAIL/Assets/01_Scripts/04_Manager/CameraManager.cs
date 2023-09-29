using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public enum CameraType
{
    Village,
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

    [SerializeField] private GameObject _testCamera;
    private float shakeTimer = 0;

    protected override void Awake()
    {
        base.Awake();
        _cameraControllerList = GetComponentsInChildren<CameraController>().ToList();
        _cameraControllerList.ForEach(x => x.VirtualCamera.Priority = 10);
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

    public void CameraSetting()
    {
        if (Player.Instance != null)
        {
            _cameraControllerList.ForEach(x => x.VirtualCamera.LookAt = Player.Instance.transform);
            _cameraControllerList.ForEach(x => x.VirtualCamera.Follow = Player.Instance.transform);
        }
        
        else
        {
            Debug.LogWarning("Player Instance was not found!!");
        }
    }

    public void Test()
    {
        _cameraControllerList.ForEach(x => x.gameObject.SetActive(false));
    }

    public CameraController GetCurrentCamera()
    {
        return _currentCamera;
    }

    public CameraController GetPrevCamera()
    {
        return _prevCamera;
    }

    public void CameraShake(float value, float time)
    {
        //Debug.Log($"{_currentCamera.name} ");
        // 없어서 추가함.
        GameObject obj = GameObject.Find("BossRoomCM");
        CinemachineVirtualCamera vcam = obj.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin vcamperl = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        vcamperl.m_AmplitudeGain = value;
        shakeTimer = time;
        StartCoroutine(Shaking(vcamperl)); ;
    }

    private IEnumerator Shaking(CinemachineBasicMultiChannelPerlin vcam)
    {

        while (shakeTimer > 0)
        {
            yield return new WaitForSeconds(0.001f);
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                vcam.m_AmplitudeGain = 0f;
            }
        }
    }

}
