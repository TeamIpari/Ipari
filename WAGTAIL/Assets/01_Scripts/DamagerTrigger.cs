using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.iOS;


public class DamagerTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ;
        }
    }
}
