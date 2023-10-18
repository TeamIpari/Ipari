using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; set; }

    public Vector3 InteractPopupOffset { get; set; }

    public bool Interact(GameObject interactor);

    public bool AnimEvent();
}
