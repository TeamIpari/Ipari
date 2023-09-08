using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotatePlatform : MonoBehaviour, IEnviroment
{
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public void ExecutionFunction(float time)
    {
        Debug.Log($"Not Have Function");  
    }

    public bool Interact()
    {


        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame  
    void Update()
    {
        transform.Rotate(Vector3.up * 1.0f);
    }
}
