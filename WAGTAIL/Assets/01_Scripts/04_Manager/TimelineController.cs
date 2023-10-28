using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _playerCtrl;
    private GameObject _timeline;
    private bool _isStart;
    
    // Timeline ���۽� ������ FakeObject
    [Header("�ƾ��� ���Ǵ� FakeObject")]
    [SerializeField] private GameObject _fakeObject;
    // Timeline ���۽� �Ⱥ����� Object;
    [Header("�ƾ��� �������� ���� Objects")]
    [SerializeField] private GameObject[] _object;

    [Header("PlayerMove")]
    // Timeline ������ �� Player�� Ư�� ��ġ�� �ű�� ����
    [Tooltip("�÷��̾ Ư�� ��ġ�� �ű�� ���� ��ü")]
    [SerializeField] private GameObject _fakeTavuti;
    [Tooltip("�÷��̾ Ư�� ��ġ�� �ű�Ÿ� üũ")]
    [SerializeField] private bool _isPlayerMoveToFakeTavuti;

    private void Start()
    {
        _playerCtrl = Player.Instance.GetComponent<CharacterController>();
        _timeline = GetComponentInChildren<PlayableDirector>().gameObject;
        _timeline.SetActive(false);
        _fakeObject.SetActive(false);
        _isStart = false;
    }

    // Player �浹 �� timeline ����.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isStart)
        {
            _isStart = true;
            SetActiveObjects(false);
            if (_fakeObject != null) _fakeObject.SetActive(true);
            _timeline.SetActive(true);
            _playerCtrl.enabled = false;
        }
    }

    // Timeline �������� ȣ������ Func()
    public void EndTimeline()
    {
        if(_isStart)
        {
            GetComponent<BoxCollider>().enabled = false;
            if (_fakeObject != null) _fakeObject.SetActive(false);
            _timeline.SetActive(false);
            SetActiveObjects(true);

            if (_isPlayerMoveToFakeTavuti)
            {
                Player.Instance.gameObject.transform.position = _fakeTavuti.transform.position;
                Player.Instance.gameObject.transform.rotation = _fakeTavuti.transform.rotation;
            }
            _playerCtrl.enabled = true;
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
