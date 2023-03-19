using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt; // Interact 가능한 범위에 있을 때 출력해줄 문구
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
