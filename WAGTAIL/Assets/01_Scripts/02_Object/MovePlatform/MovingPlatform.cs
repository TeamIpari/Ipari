using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    [Header("CustomCollider")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _size;

    // ¿òÁ÷ÀÌ´Â ÇÃ·§Æû 
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _time;


    private Collider _player;

    private float _currentTime;
    private Transform _prevWayPoint;
    private Transform _nextWayPoint;

    private Vector3 _pointSize = new Vector3(1, 1, 1);

    void Start()
    {
        transform.position = _startPoint.position;
        _prevWayPoint = _startPoint;
        _nextWayPoint = _endPoint;
        _currentTime = 0;
    }

    void FixedUpdate()
    {
        MyCustomCollider();
        _currentTime += Time.deltaTime;

        if (_currentTime >= _time)
        {
            _currentTime = _time;
            TargetNextWayPoint();
        }
        
        transform.position = Vector3.Lerp(_prevWayPoint.position, _nextWayPoint.position, _currentTime / _time);
    }

    private void TargetNextWayPoint()
    {
        Transform _currentPoint = _prevWayPoint;
        _prevWayPoint = _nextWayPoint;
        _nextWayPoint = _currentPoint;

        _currentTime = 0;

        float distanceToWaypoint = Vector3.Distance(_prevWayPoint.position, _nextWayPoint.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_startPoint.position, _pointSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_endPoint.position, _pointSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_offset + transform.position, _size);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        other.transform.SetParent(transform);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        other.transform.SetParent(null);
    //}

    private void MyCustomCollider()
    {
        Collider[] hitcolliders = Physics.OverlapBox(_offset + transform.position, _size * 0.5f, Quaternion.Euler(Vector3.zero));

        foreach (Collider collider in hitcolliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.transform.SetParent(transform);
                _player = collider;
                return;
            }
        }

        if(_player != null)
        {
            _player.transform.SetParent(null);
            _player = null;
        }
        //for(int i = 0; i < hitcolliders.Length; i++)
        //{
        //    if (hitcolliders[i].CompareTag("Player"))
        //        hitcolliders[i].transform.SetParent(transform);
        //}
    }
}