using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollision : MonoBehaviour
{
    public GameObject Door_1;
    public GameObject Door_2;

    public float _openSpeed = 1.0f;

    [SerializeField]
    private bool b_Open = false;

    private void Start()
    {
        b_Open = false;
    }

    private void Update()
    {
        
    }

    private void OpenDoor()
    {
        Door_1.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            OpenDoor();
            b_Open = true;
        }
    }
}
