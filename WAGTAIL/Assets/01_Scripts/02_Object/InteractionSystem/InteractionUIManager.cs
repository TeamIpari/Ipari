using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InteractionUIManager : MonoBehaviour
{
    private bool _isActive;
    private Animator _animator;
    private Player _player;
    private static readonly int Fadein = Animator.StringToHash("fadein");
    private static readonly int Fadeout = Animator.StringToHash("fadeout");

    private void Start()
    {
        _player = Player.Instance;
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
                _animator.SetTrigger(Fadeout);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (!_isActive)
            {
                _isActive = true;
                if (_animator.speed > 0f)
                {
                    _animator.SetTrigger(Fadein);
                }
                _animator.speed = 1.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (_isActive)
            {
                _isActive = false;
                _animator.SetTrigger(Fadeout);
            }
        }
    }
}
