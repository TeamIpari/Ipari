using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnviroment 
{
    public string EnviromentPrompt { get; }
    public bool IsHit { get; set; }

    public bool Interact();

    public void ExecutionFunction(float time);

}
