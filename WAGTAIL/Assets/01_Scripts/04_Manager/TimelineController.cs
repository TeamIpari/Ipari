using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TimelineController : MonoBehaviour
{
    [SerializeField] private GameObject _timeline;
    [SerializeField] private GameObject _fakeModel;
    [SerializeField] private GameObject[] _Models;
        
    private bool _isStart;

    private void Start()
    {
        _isStart = false;
    }

    // Player �浹 �� timeline ����.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !_isStart)
        {
            _isStart = true;
            _timeline.SetActive(true);
        }
    }

    // Timeline �������� ȣ������ Func()
    public void TimelineEnd()
    {
        if(_isStart)
        {
            _fakeModel.SetActive(false);
            _timeline.SetActive(false);
        }
    }
}
