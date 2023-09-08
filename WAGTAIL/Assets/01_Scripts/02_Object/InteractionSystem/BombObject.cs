using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BombObject : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float force;
    [SerializeField] private float explosionRange;

    private const float PushTime = 0.075f;
    private readonly Collider[] _colliders = new Collider[1];
    
    private float _currentTime;
    private CharacterController _cc;
    private Vector3 _vDir;
    private bool _isStart;
    

    // Start is called before the first frame update
    private void Start()
    {
        _currentTime = 0;
        _isStart = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            StartExplosion();
        }
    }
    
    
    private void FixedUpdate()
    {
        if (_isStart) MovePlayer();
    }
    
    public void StartExplosion()
    {
        var numFound = Physics.OverlapSphereNonAlloc(transform.position, explosionRange, _colliders, layerMask);
        if (numFound == 0) return;
        
        _cc = _colliders[0].GetComponent<CharacterController>();
        CalcDirectionVector(_colliders[0].transform.position, transform.position);
        _currentTime = 0f;
        _isStart = true;
    }
    
    private void CalcDirectionVector(Vector3 targetPos, Vector3 currentPos)
    {
        _vDir = (targetPos - currentPos).normalized;
        _vDir.y = 0;
    }

    private void MovePlayer()
    {
        if (_currentTime <= PushTime)
        {
            _currentTime += Time.deltaTime;
            _cc.Move((_vDir * (_currentTime * force)));
        }
        
        else
        {
            _isStart = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    
}
