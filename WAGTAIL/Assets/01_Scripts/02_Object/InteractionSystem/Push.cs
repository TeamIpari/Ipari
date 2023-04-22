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
            // player가 object를 미는 방향으로 보게끔 조절
            // TODO : 더 깔끔하게 해보기
            _playerPos = interactor.gameObject.transform.position;
            _hAxis = _playerPos.x - gameObject.transform.position.x;
            _vAxis = _playerPos.z - gameObject.transform.position.z;
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
