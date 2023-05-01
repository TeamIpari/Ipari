using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class lendslide : MonoBehaviour, IEnviroment
{
    bool ishit = false;

    public BrokenDoorManager Door;
    public float _brokenTime = 2.0f;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool _hit { get { return ishit; } set { ishit = value; } }

    public bool Interact()
    {
        ishit = true;
        StartCoroutine(Boom());
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !ishit)
        {
            ishit = true;
            StartCoroutine(Boom());
        }
    }


    IEnumerator Boom ()
    {
        yield return new WaitForSeconds(_brokenTime);
        Door.Boom();

    }
}
