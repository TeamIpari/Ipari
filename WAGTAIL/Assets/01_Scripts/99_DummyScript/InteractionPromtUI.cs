using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPromtUI : MonoBehaviour
{
    private Camera _mainCam;

    // Start is called before the first frame update
    private void Start()
    {
        _mainCam = Camera.main;    
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        var rotation = _mainCam.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, 
            rotation * Vector3.up);
    }
}
