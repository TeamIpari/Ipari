using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRunCamera : MonoBehaviour
{
    private Transform _target;
    private Transform _cameraTransform;
    public float calibratedDistance;

    private void Start()
    {
        _target = Player.Instance.transform;
        _cameraTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        _cameraTransform.position = new Vector3(
            _cameraTransform.position.x, _cameraTransform.position.y, _target.position.z - calibratedDistance);
    }
}
