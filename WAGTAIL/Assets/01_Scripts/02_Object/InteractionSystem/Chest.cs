using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;

    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        // ���� ó�� �ϱ�

        Debug.Log("Opening chest!");
        return true;
    }
}
