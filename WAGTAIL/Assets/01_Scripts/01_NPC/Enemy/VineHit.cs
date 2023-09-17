using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineHit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.GetComponent<Player>().isDead = true;
    }
}
