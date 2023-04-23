using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt; // Interact ������ ������ ���� �� ������� ����
    //======================================================================================
    [SerializeField] GameObject _playerEquipPoint; // Pickup�� ���� ����

    public string InteractionPrompt => _promt;

    private void Start()
    {
        _playerEquipPoint = Player.Instance.EquipPoint.gameObject;  
    }

    public bool Interact(Interactor interactor)
    {
        // TODO : Player�� nomal�������� Ȯ�����ִ°� �����ؾߵ�
        // EquitPoint�� �ڽ��� �ִ°ɷ� Ȯ������ Player State�� �������� �����

        // ���
        if (_playerEquipPoint.transform.childCount == 0 && 
            interactor.player.movementSM.currentState == interactor.player.idle) // Player �տ� ������Ʈ�� �ִ��� Ȯ��
        {
            // TODO : �ִϸ��̼� ������ ���� ���� �����ؾ���
            Pickup();
            interactor.player.isCarry = true;
            Debug.Log("Pick Up Object!");
            return true;
        }

        // ��������
        else if (_playerEquipPoint.transform.GetChild(0).name == gameObject.name)
        {
            // TODO : �ִϸ��̼� ������ ���� ���� �����ؾ���
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
        // TODO : Player State �ٲ������
        // TODO : �ٴڿ� �ٰԲ� �������ƾ���
        _playerEquipPoint.transform.DetachChildren();
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}