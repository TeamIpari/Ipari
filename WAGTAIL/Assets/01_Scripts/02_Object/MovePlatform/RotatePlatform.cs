using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlatform : MonoBehaviour, IEnviroment
{
    public string EnviromentPrompt => throw new System.NotImplementedException();

    bool _ishit;
    public bool _hit { get { return _ishit; } set { _ishit = value; } }

    public bool Interact()
    {

        //Debug.Log("AA");

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
