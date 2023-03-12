using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform PlayerPos;
    [SerializeField] public Vector3 offset;

    void Update()
    {
        transform.position = PlayerPos.position + offset;
    }
}
