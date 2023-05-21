using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private GameObject _jump;
    [SerializeField] private GameObject _move;
    [SerializeField] private GameObject _interactable;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _interactable.SetActive(true);
        }

    }
}
