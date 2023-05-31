using System;
using System.Collections;
using System.Collections.Generic;
using Polybrush;
using UnityEngine;

public class BossRunCamera : MonoBehaviour
{
    // ¿òÁ÷ÀÌ´Â ÇÃ·§Æû 
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _time;
    
    private float _currentTime;
    private bool _isEnd;
    void Start()
    {
        _isEnd = false;
        transform.position = _startPoint.position;
        _currentTime = 0;
    }

    void FixedUpdate()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= _time)
        {
            _currentTime = _time;
            _isEnd = true;
            
        }
        MoveCamera(_currentTime);
    }

    void MoveCamera(float t)
    {
        transform.position = Vector3.Lerp(_startPoint.position, _endPoint.position, t / _time);
    }
}
