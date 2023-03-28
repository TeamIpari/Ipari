using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        if(_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle)
        {
            Pickup();
            // isCarry�� isThrow�� �ٲ������
            interactor.player.isCarry = true;
            return true;
        }

        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.idle)
        {
            Throwing();
            interactor.player.isCarry = false;
            return true;
        }

        return false;
    }

    public void Pickup()
    {
        transform.SetParent(_playerEquipPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void Throwing()
    {
        // �������� �׸��� ���� ��

        _playerEquipPoint.transform.DetachChildren();
    }
}
