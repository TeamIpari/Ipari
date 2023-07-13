using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class basket : MonoBehaviour
{
    public int TargetCount;
    //[SerializeField] private int CurCount;
    public TextMeshProUGUI CountBase;
    public GameObject target;


    private void Start()
    {
        target.SetActive(false); 
        ShowText();
    }

    void ShowText()
    {
        CountBase.text = (TargetCount + 1).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            try
            {
                Throw _throw = other.GetComponent<Throw>();
                if(_throw.PhysicsCheck)
                {
                    if (TargetCount >= 0)
                    {
                        _throw.gameObject.layer = 0;
                        //_throw.enabled = false;
                        Debug.Log(other.name);
                        TargetCount--;
                    }
                    if (TargetCount == -1)
                        target.SetActive(true);
                    ShowText();
                }
            }
            catch
            {
                Debug.Log("Not Throw Object");
            }
        }
    }
}
