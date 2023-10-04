using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("interactable"))
        //    other.GetComponent<ThrowObject>().SetAutoTarget(this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        //if(other.CompareTag("interactable"))
        //    other.GetComponent<ThrowObject>().SetAutoTarget();
    }
}
