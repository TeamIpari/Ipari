using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    private bool _isPassed;

    // 
    [SerializeField] private CameraManager _manager;
    // Start is called before the first frame update
    void Start()
    {
        _isPassed = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isPassed)
            {
                _manager.isChange = true;
                _isPassed = true;
            }
            
            else if (_isPassed)
            {
                _manager.isBack = true;
                _isPassed = false;
            }
        }
    }
}
