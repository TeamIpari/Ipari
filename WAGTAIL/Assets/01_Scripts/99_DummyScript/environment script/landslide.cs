using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class landslide : MonoBehaviour, IEnvironment
{
    public BrokenDoorManager Door;
    bool is_trigger = false;
    public float time_Count = 0;

    public string InteractionPrompt => throw new System.NotImplementedException();

    public bool Interact()
    {

        //Door.Boom();
        if (!is_trigger)
        {
            Invoke("Boom", time_Count);
            is_trigger = true;
        }
        return false;
    }

    public void Boom()
    {
        Door.Boom();
    }
}
