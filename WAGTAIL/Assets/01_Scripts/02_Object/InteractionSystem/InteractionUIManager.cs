using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionUIManager : MonoBehaviour
{
    private bool _isActive;
    private Animator _animator;
    [SerializeField] private Player _player;

    private void Start()
    {
        _isActive = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.speed = 0f;
    }

    private void Update()
    {
        if( _player.isCarry || _player.isPull || _player.isClimbing || _player.isPush)
        {
            if(_isActive)
            {
                _isActive = false;
                _animator.SetTrigger("fadeout");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!_isActive)
            {
                _isActive = true;
                if (_animator.speed > 0f)
                {
                    _animator.SetTrigger("fadein");
                }
                _animator.speed = 1.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (_isActive)
            {
                _isActive = false;
                _animator.SetTrigger("fadeout");
            }
        }
    }

}
