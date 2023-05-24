using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour, IEnviroment
{
    public GameObject target;

    public bool updown = false;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    bool ishit = false;
    public bool _hit { get { return ishit; } set { ishit = value; } }

    public bool Interact()
    {
        ishit = true;
        if (target != null)
        {
            target.AddComponent<BrokenPlatform>().HideOnly(updown);
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
