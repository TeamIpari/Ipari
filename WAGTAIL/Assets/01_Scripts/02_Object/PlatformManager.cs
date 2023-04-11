using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    // ¿òÁ÷ÀÌ´Â ÇÃ·§Æû 
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _speed;
    [SerializeField] private float _time;
    [SerializeField] private Player _player;

    private Vector3 _pointSize = new Vector3(1, 1, 1);

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_startPoint.position, _pointSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_endPoint.position, _pointSize);
    }
}
