using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;

    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        // ���� ó�� �ϱ�

        Debug.Log("Open Door!");
        return true;
    }
}
