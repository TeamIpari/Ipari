using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    public GameUIType Type;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UIManager.GetInstance().ActiveGameUI(Type, true);
        }

    }
}
