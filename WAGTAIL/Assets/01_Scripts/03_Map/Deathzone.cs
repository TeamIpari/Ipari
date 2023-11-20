using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathzone : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("π‚¿∫ DeathZone" + gameObject.name);
            other.GetComponent<Player>().isDead = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("π‚¿∫ DeathZone" + gameObject.name);
            other.gameObject.GetComponent<Player>().isDead = true;
        }
    }
}
