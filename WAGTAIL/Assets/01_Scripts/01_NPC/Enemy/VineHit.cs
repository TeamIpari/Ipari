using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineHit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Player?{other.gameObject.name}");
    }
}
