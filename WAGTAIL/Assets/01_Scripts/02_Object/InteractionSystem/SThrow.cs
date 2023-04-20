using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class SThrow : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => throw new System.NotImplementedException();
    [SerializeField] GameObject _playerEquipPoint;
    [SerializeField] GameObject _playerInteractionPoint;

    [SerializeField] GameObject _playerLeftHand;
    [SerializeField] GameObject _playerRightHand;

    // 시작 지점. (어지간해선 플레이어)
    Transform startPos;
    // 끝 지점. (타겟)
    // A to B 지점으로 미리 지정을 해 놓을 것인가?
    public Transform endPos;
    // 꺾이는 지점. 
    Transform heightPos;


    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;

    public bool Interact(Interactor interactor)
    {
        if(interactor.player.movementSM.currentState == interactor.player.idle)
        {
            //interactor.player.isSmallThrow = true;
            interactor.player.isCarry = true;
            Pickup();

            return true;
        }
        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            //if(interactor.)
            Throwing(interactor);
            //interactor.player.isSmallThrow = false;
            interactor.player.isCarry = false;
        }


        return false;
    }

    public void Pickup()
    {
        // 
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;

        // 
        transform.SetParent(_playerRightHand.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // interactionPoint를 손으로 옮김.
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;

    }


    public void Throwing(Interactor interactor)
    {
        // interactionPoint의 Position을 초기상태로 되돌림
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;
        // 던지기 스타트
        Debug.Log("나, 던진다!");

    }
}
