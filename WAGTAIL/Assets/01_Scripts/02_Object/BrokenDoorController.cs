using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDoorController : MonoBehaviour
{
    private Rigidbody _rb;
    Vector3 _velocity;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Throw(float _destroyTime, float _force, GameObject _player)
    {
        // player의 방향으로 날리기 위한 계산
        _velocity = _player.transform.position - this.transform.position;
        _velocity = _velocity.normalized;
        _velocity *= _force;

        _rb.AddForce(_velocity);
    }
}