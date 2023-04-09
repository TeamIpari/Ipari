using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulling : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    //======================================================================================
    [SerializeField] GameObject _playerEquipPoint;  // Pickup을 위한 변수;

    //======================================================================================
    // Distory 이후 작동을 위한 함수
    [SerializeField] GameObject _object;
    
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        // 들기
        if (_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle) // Player 손에 오브젝트가 있는지 확인
        {
            // TODO : 애니메이션 넣으면 추후 로직 수정해야함
            Pickup();
            interactor.player.isPull = true;
            Debug.Log("Pick Up Object!");
            return true;
        }

        // 내려놓기
        else if (_playerEquipPoint.transform.GetChild(0).name == gameObject.name)
        {
            // TODO : 애니메이션 넣으면 추후 로직 수정해야함
            Drop();
            interactor.player.isPull = false;
            Debug.Log("Drop Object!");
            return true;
        }

        return true;
    }

    public void Pickup()
    {
        transform.SetParent(_playerEquipPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        gameObject.GetComponent<Node>().GetNode();
    }

    public void Drop()
    {
        _playerEquipPoint.transform.DetachChildren();
        gameObject.GetComponent<Node>().GetNode();
        gameObject.GetComponent<Node>().SetNode();
    }

    // 추가한 코드
    public void OnDestroy()
    {
        _object.GetComponent<IInteractable>().Interact(null);
    }
}
