using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnviroment 
{
    public string EnviromentPrompt { get; }
    public bool _hit { get; set; }

    public bool Interact();
}
