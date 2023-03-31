using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    // interactionPoint
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    // playerState
    [HideInInspector] public Player player; 

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        interaction();
    } 

    private void interaction()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders,
    _interactableMask);

        if (_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>();

            if (interactable != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                interactable.Interact(this);
                player.currentInteractable = _colliders[0].gameObject;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
