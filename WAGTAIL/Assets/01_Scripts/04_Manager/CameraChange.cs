using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] private CameraType _camType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CameraManager.GetInstance().SwitchCamera(_camType);
        }
    }
}
