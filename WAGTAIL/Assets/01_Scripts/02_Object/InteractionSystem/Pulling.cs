using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulling : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    public string InteractionPrompt => _promt;

    public bool Interact(Interactor interactor)
    {


        return true;
    }
}
