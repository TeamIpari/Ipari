using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _time;
    [SerializeField] private float[] _respawnTime;


    public float _currentTime;
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
        RestartCheckPoint();

        _currentTime += Time.deltaTime;

        if (_currentTime >= _time)
        {
            _currentTime = _time;
            //TargetNextWayPoint();
        }

        transform.position = Vector3.Lerp(_prevWayPoint.position, _nextWayPoint.position, _currentTime / _time);
    }

    public void RestartCheckPoint()
    {
        if (Player.Instance.isDead)
        {
            GetComponent<CinemachineVirtualCamera>().Priority = 9;
            _currentTime = _respawnTime[GameManager.GetInstance().num];
        }
        else
        {
            GetComponent<CinemachineVirtualCamera>().Priority = 11;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_startPoint.position, _pointSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_endPoint.position, _pointSize);
    }
    /*
private void TargetNextWayPoint()
{
    Transform _currentPoint = _prevWayPoint;
    _prevWayPoint = _nextWayPoint;
    _nextWayPoint = _currentPoint;

    _currentTime = 0;

    float distanceToWaypoint = Vector3.Distance(_prevWayPoint.position, _nextWayPoint.position);
}*/
}
