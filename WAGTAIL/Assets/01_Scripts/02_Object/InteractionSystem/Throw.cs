using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;
    [SerializeField] GameObject _playerInteractionPoint;
    Vector3 _nomalInteractionPoint;
    public string InteractionPrompt => _promt;


    public bool Interact(Interactor interactor)
    {
        if(_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle)
        {
            Pickup();
            // isCarry를 isThrow로 바꿔줘야함
            interactor.player.isCarry = true;
            
            return true;
        }

        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            Throwing();
            interactor.player.isCarry = false;
            return true;
        }

        return false;
    }

    public void Pickup()
    {
        GetComponent<Rigidbody>().useGravity = false;

        transform.SetParent(_playerEquipPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        // interactionPoint를 머리위로 옮김
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
    }

    public void Throwing()
    {
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;

        _playerEquipPoint.transform.DetachChildren();

        GetComponent<Rigidbody>().useGravity = true;



        // 포물선을 그리며 던진 후
    }
}
