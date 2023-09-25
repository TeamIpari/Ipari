using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform target;
    [SerializeField] private float distance;

    private void FixedUpdate()
    {
        if(target) TrackUI();
    }
    
    private void TrackUI()
    {
        transform.position = playerCamera.WorldToScreenPoint(new Vector3(target.position.x,target.position.y+distance,target.position.z));
    }
}
