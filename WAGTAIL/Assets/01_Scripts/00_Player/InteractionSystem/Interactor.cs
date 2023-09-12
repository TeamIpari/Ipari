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
    
    private Collider _currentInteractable;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        interaction();
    } 

    // (구현해야함) 가장 가까운 collider를 읽어내서 IInteractable을 상속받은 클래스가 있다면 상호작용을 한다.
    private void interaction()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders,
    _interactableMask);

        if (_numFound > 0)
        {
            var interactable = _colliders[0].GetComponent<IInteractable>();

            if (interactable != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                interactable.Interact(this.gameObject);
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
