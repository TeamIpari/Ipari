using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public enum CameraType
{
    Main,
    Death
}

public class CameraManager : Singleton<CameraManager>
{
    public enum ShakeDir
    {
        ROTATE,
        HORIZONTAL,
        VERTICAL
    }

    //=============================================
    /////               Property              /////
    //=============================================
    public Camera           MainCamera   { get { return (_mainCam == null ? (_mainCam = Camera.main) : _mainCam); } }
    public CinemachineBrain MainCamBrain { get { return (_mainCamBrain == null ? (_mainCamBrain = MainCamera.GetComponent<CinemachineBrain>()) : _mainCamBrain); } }

    [SerializeField] private GameObject _testCamera;



    //===============================================
    ///////               Fields               //////
    //===============================================
    private Camera             _mainCam;
    private CinemachineBrain   _mainCamBrain;

    private CameraController        _currentCamera;
    private CameraController        _prevCamera;
    private List<CameraController>  _cameraControllerList;
    private CinemachineImpulseSource source;

    private float shakeTimer = 0;



    //================================================
    //////             Magic methods            //////
    //================================================
    protected override void Awake()
    {
        #region Omit
        base.Awake();
        _cameraControllerList = GetComponentsInChildren<CameraController>().ToList();
        _cameraControllerList.ForEach(x => x.VirtualCamera.Priority = 10);
        #endregion
    }

    private void Start()
    {
        #region Omit
        source = GetComponent<CinemachineImpulseSource>();
        
        SwitchCamera(CameraType.Main);
        #endregion
    }



    //================================================
    //////             Public methods           //////
    //================================================
    public void SwitchCamera(CameraType type)
    {
        #region Omit
        CameraController desiredCamera = _cameraControllerList.Find(x => x.CameraType == type);

        if (desiredCamera != null)
        {
            desiredCamera.VirtualCamera.MoveToTopOfPrioritySubqueue();
            _prevCamera = _currentCamera;
            _currentCamera = desiredCamera;
        }

        else { Debug.LogWarning("The desired camera was not found!, SwitchCamera() was failed!"); }
        #endregion
    }

    public void SwitchPrevCamera()
    {
        #region Omit
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
        #endregion
    }

    public void CameraSetting()
    {
        #region Omit
        if (Player.Instance != null)
        {
            _cameraControllerList.ForEach(x => x.VirtualCamera.LookAt = Player.Instance.transform);
            _cameraControllerList.ForEach(x => x.VirtualCamera.Follow = Player.Instance.transform);
        }
        
        else
        {
            Debug.LogWarning("Player Instance was not found!!");
        }
        #endregion
    }

    public CameraController GetCurrentCamera()
    {
        return _currentCamera;
    }

    public CameraController GetPrevCamera()
    {
        return _prevCamera;
    }

    public void CamShake()
    {
        if(source != null)
            source.GenerateImpulse();
    }

    public void CameraShake(float value, float time)
    {
        #region Omit
        //Debug.Log($"{_currentCamera.name} ");
        // 없어서 추가함.
        GameObject obj = GameObject.Find("BossRoomCM");
        CinemachineVirtualCamera vcam = obj.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin vcamperl = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        vcamperl.m_AmplitudeGain = value;
        shakeTimer = time;
        StartCoroutine(Shaking(vcamperl, shakeTimer)); ;
        #endregion
    }

    public void CameraShake(float shakePow, ShakeDir shakeDir, float time, float cycleDuration = .025f)
    {
        int loopCount = Mathf.RoundToInt(time / cycleDuration);
        StartCoroutine(ShakeProgress(shakePow, shakeDir, loopCount, cycleDuration));
    }

    public void CameraShake(float shakePow, ShakeDir shakeDir, int loopCount = 10, float cycleDuration = .025f)
    {
        StartCoroutine(ShakeProgress(shakePow, shakeDir, loopCount, cycleDuration));
    }




    //===============================================
    //////           Core methods               /////
    //===============================================
    private IEnumerator Shaking(CinemachineBasicMultiChannelPerlin vcam, float time)
    {
        yield return new WaitForSeconds(time);

        vcam.m_AmplitudeGain = 0;
    }

    private IEnumerator ShakeProgress(float shakePow, ShakeDir shakeDir, int loopCount = 10, float cycleDuration = .025f)
    {
        #region Summary
        /*********************************************************
         *   카메라 흔들림을 적용하는데, 필요한 요소들을 모두 구한다...
         * ******/
        ICinemachineCamera targetCam = MainCamBrain.ActiveVirtualCamera;
        if (targetCam == null) yield break;

        float waitTime  = cycleDuration;
        float sub       = (shakePow / loopCount);
        Transform camTr = targetCam.VirtualCameraGameObject.transform;


        /************************************************
         *   수직, 수평 흔들림에 대한 로직 처리....
         * ****/
        if (shakeDir != ShakeDir.ROTATE){
            Vector3 dir = (shakeDir == ShakeDir.HORIZONTAL ? camTr.right : camTr.up);

            while (targetCam != null && loopCount>0)
            {
                for(int i=0; i>2; i++){
                    /**대상 virtual Camera를 움직이고, 다음 로직 적용까지 대기한다.....*/
                    targetCam.OnTargetObjectWarped(targetCam.Follow, (dir * shakePow));

                    while ((waitTime -= Time.deltaTime) > 0f) yield return null;
                    dir = -dir;
                    waitTime = cycleDuration;
                }

                loopCount -= 2;
                shakePow -= (sub*2);
            }
        }


        /*************************************************
         *   회전 흔들림에 대한 로직 처리....
         ********/
        else{

            Vector3 dir2 = targetCam.VirtualCameraGameObject.transform.up;

            while (targetCam != null && loopCount>0)
            {
                for(int i=0; i<4; i++){

                    /**대상 virtual Camera를 움직이고, 다음 로직 적용까지 대기한다.....*/
                    targetCam.OnTargetObjectWarped(targetCam.Follow, (dir2 * shakePow));

                    while ((waitTime -= Time.deltaTime) > 0f) yield return null;
                    waitTime = cycleDuration;
                    dir2     = Vector3.Cross(camTr.forward, dir2);
                }

                shakePow -= (sub*4f);
                loopCount -= 4;
            }
        }

        #endregion
    }

}
