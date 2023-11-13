using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUI;

    private Animator _animator;
    private static readonly int Fadeout = Animator.StringToHash("fadeout");

    // Start is called before the first frame update
    private void Start()
    {
        _animator = tutorialUI.GetComponent<Animator>();
        tutorialUI.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && tutorialUI.activeSelf == false)
        {
            tutorialUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && tutorialUI.activeSelf == true)
        {
            _animator.SetTrigger(Fadeout);
        }
    }
}
