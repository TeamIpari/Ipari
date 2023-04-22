using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt; // Interact 가능한 범위에 있을 때 출력해줄 문구
    //======================================================================================
    [SerializeField] GameObject _playerEquipPoint; // Pickup을 위한 변수

    public string InteractionPrompt => _promt;

    private void Start()
    {
        _playerEquipPoint = Player.Instance.EquipPoint.gameObject;  
    }

    public bool Interact(Interactor interactor)
    {
        // TODO : Player가 nomal상태인지 확인해주는거 구현해야됨
        // EquitPoint에 자식이 있는걸로 확인할지 Player State로 관리할지 고민중

        // 들기
        if (_playerEquipPoint.transform.childCount == 0 && 
            interactor.player.movementSM.currentState == interactor.player.idle) // Player 손에 오브젝트가 있는지 확인
        {
            // TODO : 애니메이션 넣으면 추후 로직 수정해야함
            Pickup();
            interactor.player.isCarry = true;
            Debug.Log("Pick Up Object!");
            return true;
        }

        // 내려놓기
        else if (_playerEquipPoint.transform.GetChild(0).name == gameObject.name)
        {
            // TODO : 애니메이션 넣으면 추후 로직 수정해야함
            Drop();
            interactor.player.isCarry = false;
            Debug.Log("Drop Object!");
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

    public void Drop()
    {
        // TODO : Player State 바꿔줘야함
        // TODO : 바닥에 붙게끔 내려놓아야함
        _playerEquipPoint.transform.DetachChildren();
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}