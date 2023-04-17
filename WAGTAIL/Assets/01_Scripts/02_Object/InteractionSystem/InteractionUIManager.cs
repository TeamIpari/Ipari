using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionUIManager : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 1.0f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private int _numFound;
    [SerializeField] private GameObject _player; 
    private readonly Collider[] _colliders = new Collider[1];
    private bool _isActive;
    private Animator _animator;

    //private static int testAniHash = Animator.StringToHash("fadein");


    private void Start()
    {
        _isActive = false;
        _animator = GetComponentInChildren<Animator>();
        _animator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        interactionUI();
    }

    private void interactionUI()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, 
            _colliders,_interactableMask);

        if(_numFound > 0 && !_isActive)
        {
            _isActive = true;
            if (_animator.speed > 0f)
            {
                _animator.SetTrigger("fadein");
            }
            _animator.speed = 1.0f;
        }

        else if(_numFound == 0 && _isActive)
        {
            _isActive = false;
            _animator.SetTrigger("fadeout");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
