using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt; // Interact ������ ������ ���� �� ������� ����
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {
        if(interactor.player.movementSM.currentState == interactor.player.idle)
        {
            interactor.player.isClimbing = true;
            return true;
        }

        return false;
    }
}
