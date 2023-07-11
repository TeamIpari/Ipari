using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventController : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => throw new System.NotImplementedException();

    public UnityEvent InteractableEvent;
    // Start is called before the first frame update
    void Start()
    {

    }

    public bool Interact(Interactor interactor)
    {
        if(InteractableEvent.GetPersistentEventCount() > 0)
            InteractableEvent.Invoke();

        return true;
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}
