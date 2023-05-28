using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public Animation Anim;

    private void Start()
    {
        Anim = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
