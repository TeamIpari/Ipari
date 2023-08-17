using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour, IEnviroment
{
    public GameObject Target;

    public bool Updown = false;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        IsHit = true;
        if (Target != null)
        {
            Target.AddComponent<BrokenPlatform>().HideOnly(Updown);
        }

        return false;
    }
}
