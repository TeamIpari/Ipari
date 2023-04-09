using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulling : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    //======================================================================================
    [SerializeField] GameObject _playerEquipPoint;  // Pickup�� ���� ����;

    //======================================================================================
    // Distory ���� �۵��� ���� �Լ�
    [SerializeField] GameObject _object;
    
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        // ���
        if (_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle) // Player �տ� ������Ʈ�� �ִ��� Ȯ��
        {
            // TODO : �ִϸ��̼� ������ ���� ���� �����ؾ���
            Pickup();
            interactor.player.isPull = true;
            Debug.Log("Pick Up Object!");
            return true;
        }

        // ��������
        else if (_playerEquipPoint.transform.GetChild(0).name == gameObject.name)
        {
            // TODO : �ִϸ��̼� ������ ���� ���� �����ؾ���
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

    // �߰��� �ڵ�
    public void OnDestroy()
    {
        _object.GetComponent<IInteractable>().Interact(null);
    }
}
