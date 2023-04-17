using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour, IInteractable
{
    [Header("Interactable Setting")]
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;
    [SerializeField] GameObject _playerInteractionPoint;

    [Header("Throw Setting")]
    [SerializeField] float _force = 1.0f;
    [SerializeField] float _yForce = 1.0f;
    [Range(0, 1)]
    [SerializeField] float _yAngle = 0.45f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;
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

        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            Throwing(interactor);
            interactor.player.isCarry = false;
            return true;
        }

        return false;
    }

    public void Pickup()
    {
        // Object�� Player�� �Ӹ� ������ �����̴°� ����
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().isKinematic = false;

        // Object�� Player�� �Ӹ� ���� �ű�
        transform.SetParent(_playerEquipPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        // interactionPoint�� �Ӹ� ���� �ű�
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
    }


    public void Throwing(Interactor interactor)
    {

        // interactionPoint�� Position�� �ʱ���·� �ǵ���
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;

        // Object ������ Ǯ����
        _playerEquipPoint.transform.DetachChildren();

        // �Ӹ� ������ �����̴°� �����ϱ� ���� �͵� ����
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().useGravity = true;

        // ���� ��Ĵ�� ����
        _playerForwardTransform = interactor.player.transform.forward;
        _playerForwardTransform.x *= _force;
        _playerForwardTransform.y = _yForce * _yAngle;
        _playerForwardTransform.z *= _force;

        GetComponent<Rigidbody>().AddForce(_playerForwardTransform );
    }
}
