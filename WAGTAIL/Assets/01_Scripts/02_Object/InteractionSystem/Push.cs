using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Push : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;

    private float _vAxis;
    private float _hAxis;

    Vector3 _playerPos;
    public Vector3 dir;

    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        if (interactor.player.movementSM.currentState == interactor.player.idle)
        {
            interactor.player.isPush = true;
            
            _playerPos = interactor.gameObject.transform.position;
            var position = gameObject.transform.position;
            _hAxis = _playerPos.x - position.x;
            _vAxis = _playerPos.z - position.z;
            dir = new Vector3(_hAxis, 0, _vAxis).normalized;
            dir.x = -Mathf.Round(dir.x);
            dir.z = -Mathf.Round(dir.z);
            interactor.transform.LookAt(interactor.transform.position + dir);

            transform.SetParent(_playerEquipPoint.transform, true);
            return true;
        }

        else if (interactor.player.movementSM.currentState == interactor.player.push)
        {
            interactor.player.isPush = false;
            _playerEquipPoint.transform.DetachChildren();
            return true;
        }

        return false;
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}
