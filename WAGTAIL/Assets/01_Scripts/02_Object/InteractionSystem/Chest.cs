using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] private float _pos;

    Vector3 _position;
    Vector3 _playerPos;

    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        _playerPos = interactor.gameObject.transform.position;
        // 예외 처리 하기
        _position = new Vector3(_playerPos.x + _pos, _playerPos.y, _playerPos.z);
        Debug.Log("Opening chest!");
        return true;
    }
}
