using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    private GameObject _player;
    //private CharacterController _playerCtrl;
    private GameObject _timeline;
    private bool _isStart;

    [Header("Objects")]
    // Timeline 동작시 보여줄 FakeObject
    [Tooltip("컷씬에 사용되는 FakeObject")]
    [SerializeField] private GameObject _fakeObject;
    // Timeline 동작시 안보여줄 Object;
    [Tooltip("컷씬에 등장하지 않을 Objects")]
    [SerializeField] private GameObject[] _object;

    [Header("PlayerMove")]
    // Timeline 끝났을 때 Player를 특정 위치로 옮기기 위한
    [Tooltip("플레이어를 특정 위치로 옮기기 위한 객체")]
    [SerializeField] private GameObject _fakeTavuti;
    [Tooltip("플레이어를 특정 위치로 옮길거면 체크")]
    [SerializeField] private bool _isPlayerMoveToFakeTavuti;

    private void Start()
    {
        //_playerCtrl = Player.Instance.GetComponent<CharacterController>();
        _timeline = GetComponentInChildren<PlayableDirector>().gameObject;
        _timeline.SetActive(false);
        _fakeObject.SetActive(false);
        _isStart = false;
    }

    // Player 충돌 시 timeline 실행.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isStart)
        {
            _isStart = true;
            _player = other.gameObject;
            SetActiveObjects(false);
            _fakeObject.SetActive(true);
            _timeline.SetActive(true);
            //_playerCtrl.enabled = false;
            _player.SetActive(false);
        }
    }

    // Timeline 끝났을때 호출해줄 Func()
    public void EndTimeline()
    {
        if(_isStart)
        {
            GetComponent<BoxCollider>().enabled = false;
            _fakeObject.SetActive(false);
            _timeline.SetActive(false);
            SetActiveObjects(true);

            if (_isPlayerMoveToFakeTavuti)
            {
                Player.Instance.gameObject.transform.position = _fakeTavuti.transform.position;
                Player.Instance.gameObject.transform.rotation = _fakeTavuti.transform.rotation;
            }
            _player.SetActive(true);
            //_playerCtrl.enabled = true;
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
