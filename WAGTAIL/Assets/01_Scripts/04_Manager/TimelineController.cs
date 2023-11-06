using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    private GameObject _player;
    private PlayerInput _playerInput;
    private CharacterController _playerCtrl;
    private GameObject _timeline;
    private bool _isStart;
    
    // Timeline 동작시 보여줄 FakeObject
    [Header("컷씬에 사용되는 FakeObject")]
    [SerializeField] private GameObject _fakeObject;
    // Timeline 동작시 안보여줄 Object;
    [Header("컷씬에 등장하지 않을 Objects")]
    [SerializeField] private GameObject[] _object;

    [Header("PlayerMove")]
    // Timeline 끝났을 때 Player를 특정 위치로 옮기기 위한
    [Tooltip("플레이어를 특정 위치로 옮기기 위한 객체")]
    [SerializeField] private GameObject _fakeTavuti;
    [Tooltip("플레이어를 특정 위치로 옮길거면 체크")]
    [SerializeField] private bool _isPlayerMoveToFakeTavuti;

    [Header("Debug")] 
    [SerializeField] private bool _isDebug = false;
    public bool chapter03 = false;

    private void Start()
    {
        _player = Player.Instance.gameObject;
        _playerInput = _player.GetComponent<PlayerInput>();
        _playerCtrl = _player.GetComponent<CharacterController>();
        _timeline = GetComponentInChildren<PlayableDirector>().gameObject;
        _timeline.SetActive(false);
        if (_fakeObject != null) _fakeObject.SetActive(false);
        _isStart = false;
        var collider = GetComponent<BoxCollider>();
        if(collider == null)
            StartTimeline();
    }

    // Player 충돌 시 timeline 실행.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isStart)
        {
            GetComponent<BoxCollider>().enabled = false;
            StartTimeline();
        }
    }

    public void StartTimeline()
    {
        _isStart = true;
        SetActiveObjects(false);
        if (_fakeObject != null) _fakeObject.SetActive(true);
        _timeline.SetActive(true);
        _playerInput.enabled = false;
        _playerCtrl.enabled = false;
        _player.SetActive(false);
    }

    // Timeline 끝났을때 호출해줄 Func()
    public void EndTimeline()
    {
        if(_isStart)
        {
            if (_isPlayerMoveToFakeTavuti)
            {
                Player.Instance.gameObject.transform.position = _fakeTavuti.transform.position;
                Player.Instance.gameObject.transform.rotation = _fakeTavuti.transform.rotation;
            }
            
            if (_fakeObject != null) _fakeObject.SetActive(false);
            _timeline.SetActive(false);
            SetActiveObjects(true);
            
            _player.SetActive(true);
            _playerInput.enabled = true;
            _playerCtrl.enabled = true;
            if (CameraManager.GetInstance() != null) CameraManager.GetInstance().SwitchCamera(CameraType.Main);
            if (chapter03)
            {
                UIManager.GetInstance().ActiveGameUI(GameUIType.Coin, true);
                UIManager.GetInstance().ActiveGameUI(GameUIType.CoCosi, true);
                UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>().SetCanvas(2,true);
            }
        }
    }

    private void SetActiveObjects(bool b)
    {
        if (_object.Length == 0)
            return;

        for (int i = 0; i < _object.Length; i++)
        {
            _object[i].SetActive(b);
        }
    }
}
